using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using DG.Tweening;

public enum NumbericUnit
{
    None, K, M, B, T
}

[System.Serializable]
public class Number
{
    public GameObject obj;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
}

public class ClockNum : SaveableObject
{
    public static float numberBetweenDist = 0.08f;
    [SerializeField] GameObject[] numberObjects;
    [SerializeField] Number[] numbers;



    [SerializeField] Number dot;
    [SerializeField] MeshRenderer meshRenderer;


    public float hp;
    public float currentHp;

    private Vector3 baseScale = Vector3.one;

    public float KillReward;

    public Stopper currentStopper;

    private bool particleReady = true;

    [ReadOnly]
    public bool die = false;

    private bool enableTween = true;

    public bool hideNum = false;
    public bool goldNum = false;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Start()
    {
        ChangeValue(currentHp);
    }

    public void UpdateUI()
    {
        ChangeValue(currentHp);
    }

    public void ChangeValue(float value)
    {
        // currentHp = Mathf.FloorToInt(value);

        int objCount = 0;

        NumbericUnit numbericUnit;
        short shortNum = 0;
        short shortNum2 = 0;

        bool _decimal = false;

        //단위 계산
        if (value > 1000000000)
        {

            if ((short)(value / 1000000000) < 10)
            {
                shortNum = (short)(value / 100000000);
                _decimal = true;
            }
            else
                shortNum = (short)(value / 1000000000);


            numbericUnit = NumbericUnit.B;

            ChangeNumberMesh(shortNum, numbericUnit, _decimal);
        }
        else if (value > 1000000)
        {
            if ((short)(value / 1000000) < 10)
            {
                shortNum = (short)(value / 100000);
                _decimal = true;
            }
            else
                shortNum = (short)(value / 1000000);



            numbericUnit = NumbericUnit.M;

            ChangeNumberMesh(shortNum, numbericUnit, _decimal);
        }
        else if (value > 1000)
        {
            if ((short)(value / 1000) < 10)
            {
                shortNum = (short)(value / 100);
                _decimal = true;
            }
            else
                shortNum = (short)(value / 1000);


            numbericUnit = NumbericUnit.K;

            ChangeNumberMesh(shortNum, numbericUnit, _decimal);
        }
        else if (value > 0)
        {
            shortNum = (short)value;
            numbericUnit = NumbericUnit.None;

            ChangeNumberMesh(shortNum, numbericUnit, _decimal);
        }

        void ChangeNumberMesh(short _shortNum, NumbericUnit _numbericUnit, bool _decimal2)
        {
            if (hideNum)
            {
                dot.obj.SetActive(false);

                for (int i = 0; i < 3; i++)
                {
                    numbers[i].meshRenderer.enabled = true;
                    numbers[i].meshFilter.mesh = MainManager.instance.hideNum;
                    numbers[i].meshCollider.sharedMesh = numbers[i].meshFilter.sharedMesh;
                    numbers[i].meshCollider.enabled = true;
                    numbers[i].meshRenderer.transform.localPosition = Vector3.zero;

                    numbers[i].meshRenderer.materials = goldNum ? MainManager.instance.goldMat : MainManager.instance.defMat;
                }

                numbers[3].obj.SetActive(!hideNum);
            }
            else
            {
                numbers.ToList().ForEach((n) => { n.meshRenderer.enabled = false; n.meshCollider.enabled = false; });

                objCount = 0;

                foreach (char digitChar in _shortNum.ToString())
                {
                    var digit = int.Parse(digitChar.ToString());
                    // print(digit + " " + digitChar + " " + numberObjects[objCount]);

                    numbers[objCount].meshRenderer.enabled = true;
                    numbers[objCount].meshFilter.mesh = MainManager.instance.GetNumberObject(digit);
                    numbers[objCount].meshRenderer.materials = goldNum ? MainManager.instance.goldMat : MainManager.instance.defMat;
                    objCount++;
                }

                List<float> extra = new List<float>();

                if (_decimal2)
                {
                    dot.obj.SetActive(true);
                    extra.Add(-0.025f);
                }
                else
                    dot.obj.SetActive(false);

                dot.meshRenderer.materials = goldNum ? MainManager.instance.goldMat : MainManager.instance.defMat;

                numbers[3].meshRenderer.enabled = true;
                numbers[3].meshFilter.mesh = MainManager.instance.GetNumbericUnit(_numbericUnit);
                numbers[3].meshRenderer.materials = goldNum ? MainManager.instance.goldMat : MainManager.instance.defMat;

                // numberObjects[3].GetComponentInChildren<MeshCollider>(true).isTrigger

                var find = numbers.Where((n) => n.meshRenderer.enabled).Select((n) => n.obj.transform).ToArray();
                ArrangeNumberText(find, extra);

                foreach (var num in numbers)
                {
                    num.meshCollider.sharedMesh = num.meshFilter.sharedMesh;
                    num.meshCollider.enabled = true;
                }
            }
        }
    }

    public void ArrangeNumberText(Transform[] nums, List<float> extra)
    {
        float middleValue = ((numberBetweenDist * nums.Length) * 0.5f);
        for (int i = 0; i < nums.Length; i++)
        {
            nums[i].transform.localPosition = new Vector3(((numberBetweenDist * (i + 1)) - middleValue) + (extra.Count > i ? extra[i] : 0), nums[i].transform.localPosition.y, nums[i].transform.localPosition.z);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp = Mathf.Clamp(currentHp -= damage, 0, int.MaxValue);

        if (!ShopUI.shopUIActive)
            AdManager.instance.ShowInterstitial();

        ChangeValue(currentHp);

        for (int i = 0; i < numbers.Length; i++)
        {
            if (enableTween)
            {
                var tarns = numbers[i].meshRenderer.transform;
                tarns.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack)
                            .OnComplete(() =>
                                tarns.DOScale(1f, 0.2f).OnComplete(() => enableTween = true)
                           );
            }

            if (particleReady)
            {
                if (numbers[i].meshRenderer.enabled)
                {
                    var particle = MainManager.instance.PopParticle(goldNum ? "Pooling/Particle/HitParticle Gold" : "Pooling/Particle/HitParticle", Vector3.one, Quaternion.identity, null, 2f);
                    var p = particle.GetComponent<ParticleSystem>();
                    var shape = p.shape;
                    shape.meshRenderer = numbers[i].meshRenderer;
                }
            }
        }
        particleReady = false;
        MainManager.instance.TaskDelay(0.1f, () => particleReady = true);

        enableTween = false;

        if (MainManager.instance._ITR_MODE)
            StageManager.instance.currentClock.ChangeClockNumInputField();

        if (currentHp <= 0)
            Die();

        // Save();
    }

    public void ForceChangeValue(int value)
    {
        currentHp = value;

        ChangeValue(currentHp);

        if (currentHp <= 0)
            Die();

        Save();

        // Debug.LogError("forceChanged " + value);
    }

    [Button]
    public void Die()
    {
        if (!ShopUI.shopUIActive)
            MainManager.GenerateUIParticleAttractorFromWorldSpace(MainManager.instance.moneyIcon, transform);

        SaveManager.instance.AddMoney(KillReward);

        die = true;
        gameObject.SetActive(false);

        currentHp = 0;

        StageManager.instance.currentClock.OnDestroyNum();

        // if (currentStopper != null)
        //     currentStopper.Show();

        QuestManager.instance.OnProgressQuest(QuestType.Destory1Number);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Destory Num - [" + StageManager.instance.currentStage + " - " + hp + "]", true, true);

        ES3.Save<int>("DestroyNumCount", (ES3.KeyExists("DestroyNumCount") ? ES3.Load<int>("DestroyNumCount") + 1 : 1));
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Destroy Num Count - " + ES3.Load<int>("DestroyNumCount"), true, true);

        MainManager.instance.GenerateMoneyText(KillReward, transform.position + new Vector3(Random.Range(-0.04f, 0.04f), Random.Range(-0.04f, 0.04f) + 0.35f, -0.2f), path: "MoneyParticleText_Die");

        if (!ES3.KeyExists("PauseTutorial"))
        {
            this.TaskDelay(10f, () => Tutorial.insatnce.StartPauseTutorial());
        }
    }


    public override void Save()
    {
        base.Save();

        PlayerPrefs.SetFloat(Guid, currentHp);
        // ES3.Save<float>(Guid, currentHp);

        // print(Guid + " " + currentHp);
    }

    public void Load()
    {
        // currentHp = ES3.KeyExists(Guid) ? ES3.Load<float>(Guid) : hp;
        currentHp = PlayerPrefs.HasKey(Guid) ? PlayerPrefs.GetFloat(Guid) : hp;

        if (currentHp <= 0)
        {
            gameObject.SetActive(false);
            die = true;

            if (currentStopper != null)
            {
                currentStopper.Show();
            }
        }

        // Debug.LogError(Guid + " " + (ES3.KeyExists(Guid) ? ES3.Load<float>(Guid) : hp));

        ChangeValue(currentHp);
    }


    [Button("ForceChange")]
    public void ForceChangeNumber()
    {
        currentHp = hp;
        ChangeValue(currentHp);
    }

    public void SaveReset()
    {
        currentHp = hp;
        Save();
    }

    [Button("Genenrate Money Particle")]
    public void GenerateMoneyParticle()
    {
        MainManager.GenerateUIParticleAttractorFromWorldSpace(MainManager.instance.moneyIcon, transform);
    }
}

