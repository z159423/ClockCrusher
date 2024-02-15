using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System.Linq;

public class Clock : MonoBehaviour
{
    [SerializeField] ClockNum[] ClockNumbers;
    [Button("Find Numbers")]
    public void FindNumbers() => ClockNumbers = GetComponentsInChildren<ClockNum>();
    public ClockNum[] GetClockNums() => ClockNumbers;
    public ClockNum[] aliveNum;

    public float[] clockSpawnRotates;

    public int maxPointerCount = 10;

    public bool stop = true;

    public GameObject clockModel2;
    public MeshRenderer meshRenderer;

    [Space]

    public Mesh clockMeshType1;
    public Material[] clockMattype1;
    public Mesh clockMeshType2;
    public Material[] clockMattype2;

    [Space]

    public GameObject destoryPartObject;

    public System.Action onAfterStageStart;


    //
    private bool wiggle = false;

    private void Start()
    {
        if (!ES3.KeyExists("ClockPointers"))
            MainManager.instance.SpawnPointer(-30, true, 1);

        if (MainManager.instance._ITR_MODE)
            ChangeMesh();

        UpdateAliveClock();
    }


#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (clockModel2 != null)
            {
                clockModel2.SetActive(meshRenderer.enabled);
                meshRenderer.enabled = !meshRenderer.enabled;
            }
        }
    }
#endif

    public void Init()
    {
        // Debug.LogError(gameObject.name);

        foreach (var num in ClockNumbers)
            num.Load();


    }

    [Button("ForceFindClockNumbers")]
    void FindClockNumbers()
    {
        ClockNumbers = GetComponentsInChildren<ClockNum>();
    }


    public void ClockWiggle()
    {
        // transform.DOShakeScale(0.15f, 0.05f, 1).OnComplete(() => transform.localScale = Vector3.one);
        if (wiggle)
            return;

        wiggle = true;

        transform.transform.DOScale(1.012f, 0.15f).SetEase(Ease.OutBack)
            .OnComplete(() => { transform.transform.DOScale(1f, 0.15f).OnComplete(() => wiggle = false); });
    }

    public void OnDestroyNum()
    {
        if (ClockNumbers.Where((n) => n.die).Count() == ClockNumbers.Length)
        {
            this.TaskDelay(3, () => StageManager.instance.StageClear());
        }

        if (StageManager.instance.currentClock.GetComponentsInChildren<ClockNum>(true).Where((n) => n.die).Count() >= 1)
        {
            if (!ES3.KeyExists("moneyFeverReady"))
            {
                ES3.Save<bool>("moneyFeverReady", true);
                this.TaskDelay(10f, () => AdManager.instance._moneyFeverBtn.Show());
            }
        }

        if (StageManager.instance.currentClock.GetComponentsInChildren<ClockNum>().Count() == 1)
        {
            MainManager.instance.StartLastFever();
        }

        UpdateAliveClock();
    }

    public void UpdateUI()
    {
        foreach (var num in ClockNumbers)
        {
            num.UpdateUI();
        }
    }

    public void ChangeMesh()
    {
        switch (ITRManager.instance.clockMatType)
        {
            case clockMatType.type1:
                if (clockMeshType1 != null)
                    GetComponentInChildren<MeshFilter>().mesh = clockMeshType1;
                if (clockMattype1.Length > 0)
                    GetComponentInChildren<MeshRenderer>().materials = clockMattype1;
                break;

            case clockMatType.type2:
                if (clockMeshType2 != null)
                    GetComponentInChildren<MeshFilter>().mesh = clockMeshType2;
                if (clockMattype2.Length > 0)
                    GetComponentInChildren<MeshRenderer>().materials = clockMattype2;
                break;
        }

        switch (ITRManager.instance.backgroundType)
        {
            case BackgroundType.def:
                gameObject.layer = LayerMask.NameToLayer("Default");
                break;

            case BackgroundType.magenta:
                gameObject.layer = LayerMask.NameToLayer("UI");

                break;
        }
    }

    public void ForceChangeValue(int num, int value)
    {
        ClockNumbers[num].ForceChangeValue(value);
    }

    public void ChangeClockNumInputField()
    {
        for (int i = 0; i < ClockNumbers.Length; i++)
        {
            Cheat.instance.ChangeClockNumHpField(i, (int)ClockNumbers[i].currentHp);
        }
    }

    public void ClockSaveReset()
    {
        foreach (var num in ClockNumbers)
        {
            num.SaveReset();
        }
    }

    public void UpdateAliveClock()
    {
        if (gameObject != null)
            aliveNum = ClockNumbers.Where((n) => n.gameObject.activeSelf).ToArray();
    }
}
