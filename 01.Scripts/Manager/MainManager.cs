using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using MoreMountains.NiceVibrations;
using DG.Tweening;
using System;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public enum UpgradeType
{
    AddStick,
    Speed,
    Income,
    Merge
}

[System.Serializable]
public class Upgrade
{
    public UpgradeType type;

    public int maxLevel;
    public int currentLevel;
    public int[] cost;

    public void TryUpgrade()
    {
        // if (currentLevel < maxLevel)
        currentLevel++;
    }

    public int GetCurrentCost()
    {
        return cost[Mathf.Clamp(currentLevel, 0, cost.Length - 1)];
    }

    public bool CheckIsMax()
    {
        if (currentLevel >= maxLevel)
            return true;
        else
            return false;
    }
}

public class MainManager : MonoBehaviour, IUpdater
{
    public static MainManager instance;

    public List<ClockPointer> pointerList = new List<ClockPointer>();

    public Mesh[] numberMeshs;
    public Mesh[] numbericUnits;
    public Mesh hideNum;
    public Material[] defMat;
    public Material[] goldMat;
    public Mesh[] dot;

    [SerializeField] private bool interstitialReady;
    [SerializeField] private float interstitialTime;


    public float clockHandRotateSpeed = 1f;

    public float clickSpeedBonus = 1f;
    public float clickSpeedBonusDuration = 0.5f;
    public bool touchSpeedUp = false;
    public TaskUtil.DelayTaskMethod touchSpeedUpDelay = null;

    public bool enableRotate = true;

    public float GetRotateSpeed() => clockHandRotateSpeed + (touchSpeedUp || AdManager.instance.autoFever ? clickSpeedBonus : 0f) + UPGRADE_SPEED_VALUE[FindUpgrade(UpgradeType.Speed).currentLevel];
    public float GetRotateSpeed2(Upgrade upgrade) => clockHandRotateSpeed + (touchSpeedUp || AdManager.instance.autoFever ? clickSpeedBonus : 0f) + UPGRADE_SPEED_VALUE[upgrade.currentLevel];

    [SerializeField] private GameObject mergeSlot;
    [SerializeField] private AnimationCurve mergeAnimationCurve;
    [SerializeField] public Transform UI;
    [SerializeField] public Transform playUI;
    [SerializeField] public Transform particleUI;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public Camera UICamera;
    [SerializeField] public Transform moneyIcon;
    [SerializeField] public Transform rvTicketIcon;
    [SerializeField] public Transform particleParent;
    [SerializeField] public GameObject tapToSpeedUp;
    [SerializeField] public PauseBtn pauseBtn;
    [SerializeField] public StickCollectionBtn stickCollectionBtn;
    [SerializeField] public ShopUI shopUI;
    [SerializeField] public GameObject rvTicketText;
    [SerializeField] public Text rpmText;
    [SerializeField] public RvSideUI[] rvSideUIs;
    [SerializeField] public MoneyOfferSideUI moneyOfferSideUI;


    public PointerSkinType pointerSkinType = PointerSkinType.Default;

    [Title("업그레이드")]
    [Space]

    public Upgrade[] UPGRADES;
    public Upgrade FindUpgrade(UpgradeType type) => UPGRADES.Where((n) => n.type == type).First();
    public Upgrade addStick;
    public Upgrade speed;
    public Upgrade income;
    public Upgrade merge;


    public int GetPointerMaxLevel() => Resources.LoadAll<GameObject>("Pointer").OrderByDescending((n) => n.GetComponent<ClockPointer>().Level).First().GetComponent<ClockPointer>().Level;

    public float[] UPGRADE_SPEED_VALUE = { .05f, .1f, .15f, .2f, .25f, .30f, .35f, .40f, .45f, .5f, .55f, .6f, .65f, .7f, .75f, .8f, .9f, .95f, 1f };
    public float[] UPGRADE_INCOME_VAlUE = { 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2f, 2.1f, 2.2f, 2.3f, 2.4f, 2.5f, 2.6f, 2.7f, 2.8f, 2.9f, 3f };

    public AnimationCurve mergeEase;

    public System.Action onChangePointer;

    private bool isMerge = false;
    public bool isReadyOnRV = false;
    public bool onDragPointer = false;
    public bool tutorial1 = false;

    public bool pause = false;
    public bool rvPause = false;


    public bool PopupEnable = true;
    public bool _ITR_MODE = false;

    public bool showMoneyGainText = true;
    public bool UpgardeRVActive = false;

    [Button("Force")]
    private void Awake()
    {
        if (!ES3.KeyExists("ClearZero"))
        {
            MondayOFF.EventTracker.ClearStage(0);
            ES3.Save<bool>("ClearZero", true);
        }

        instance = this;

        var addstick = FindUpgrade(UpgradeType.AddStick);
        addstick.currentLevel = ES3.KeyExists("UPGRADE_ADDSTICK") ? ES3.Load<int>("UPGRADE_ADDSTICK") : 0;
        addstick.maxLevel = addstick.cost.Length - 1;
        this.addStick = addstick;

        var speed = FindUpgrade(UpgradeType.Speed);
        speed.currentLevel = ES3.KeyExists("UPGRADE_SPEED") ? ES3.Load<int>("UPGRADE_SPEED") : 0;
        this.speed = speed;

        speed.maxLevel = UPGRADE_SPEED_VALUE.Length - 1;

        var income = FindUpgrade(UpgradeType.Income);
        income.currentLevel = ES3.KeyExists("UPGRADE_INCOME") ? ES3.Load<int>("UPGRADE_INCOME") : 0;
        income.maxLevel = UPGRADE_INCOME_VAlUE.Length - 1;
        this.income = income;

        var merge = FindUpgrade(UpgradeType.Merge);
        merge.currentLevel = ES3.KeyExists("UPGRADE_MERGE") ? ES3.Load<int>("UPGRADE_MERGE") : 0;
        merge.maxLevel = merge.cost.Length - 1;
        this.merge = merge;

        // onChangePointer += UpdateUI_Merge;
    }

    public void ResetUpgrade()
    {
        var addstick = FindUpgrade(UpgradeType.AddStick);
        addstick.currentLevel = 0;
        ES3.Save<int>("UPGRADE_ADDSTICK", addstick.currentLevel);

        var speed = FindUpgrade(UpgradeType.Speed);
        speed.currentLevel = 0;
        ES3.Save<int>("UPGRADE_SPEED", speed.currentLevel);

        var income = FindUpgrade(UpgradeType.Income);
        income.currentLevel = 0;
        ES3.Save<int>("UPGRADE_INCOME", income.currentLevel);

        var merge = FindUpgrade(UpgradeType.Merge);
        merge.currentLevel = 0;
        ES3.Save<int>("UPGRADE_MERGE", merge.currentLevel);

        SaveManager.instance.onChangeMoneyEvent.Invoke();
    }

#if UNITY_EDITOR
    [MenuItem("Custome/Quick Start")]
    static void QuickStart()
    {
        EditorSceneManager.OpenScene("Assets/00.Scenes/MondayOFFSplashScene.unity");
        EditorApplication.isPlaying = true;
    }
#endif

#if UNITY_EDITOR
    [MenuItem("Custome/Reset And Quick Start")]
    static void ResetAndQuickStart()
    {
        ES3.DeleteFile();
        PlayerPrefs.DeleteAll();
        EditorSceneManager.OpenScene("Assets/00.Scenes/MondayOFFSplashScene.unity");
        EditorApplication.isPlaying = true;
    }
#endif



    private void Start()
    {
        // this.TaskWaitUntil(() => onChangePointer.Invoke(), () => StageManager.instance.currentClock != null);

        if (ES3.KeyExists("MergeTutorial"))
        {
            pauseBtn.gameObject.SetActive(true);
        }

        if (ES3.KeyExists("PointerSkinType"))
            pointerSkinType = ES3.Load<PointerSkinType>("PointerSkinType");

        UpgardeRVActive = PlayerPrefs.HasKey("UpgardeRVActive");

        rvTicketText.SetActive(ES3.KeyExists("enableShop"));

        // this.TaskDelay(3f, () => throw new System.Exception("test 1"));

        this.TaskWaitUntil(() => UpdateUI_Merge(), () => StageManager.instance != null);

        if (_ITR_MODE)
            switch (ITRManager.instance.backgroundType)
            {
                case BackgroundType.def:
                    mainCamera.clearFlags = CameraClearFlags.Skybox;

                    break;

                case BackgroundType.magenta:
                    mainCamera.clearFlags = CameraClearFlags.SolidColor;
                    mainCamera.backgroundColor = Color.magenta;
                    break;
            }

        Updater.Instance.AddUpdater(this);

        mainCamera.rect = new Rect(0, 0, 1f, 1f);
    }

    public void FrameUpdate()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        if (Input.GetKeyDown(KeyCode.Z))
        {
            MondayOFF.AdsManager.ShowInterstitial();
        }

        interstitialReady = MondayOFF.AdsManager.IsInterstitialReady();
        interstitialTime = MondayOFF.AdsManager.GetTimeUntilNextInterstitial();

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Time.timeScale = Time.timeScale + 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Time.timeScale = Time.timeScale - 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && !Cheat.instance.whileEdit)
        {
            SaveManager.instance.AddMoney(1000000);
        }
        else if (Input.GetKeyDown(KeyCode.M) && !Cheat.instance.whileEdit)
        {
            ES3.DeleteFile();
            PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !Cheat.instance.whileEdit)
        {
            Cheat.instance.OnChangeSelectedPointerLevel(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && !Cheat.instance.whileEdit)
        {
            Cheat.instance.OnChangeSelectedPointerLevel(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && !Cheat.instance.whileEdit)
        {
            Cheat.instance.OnChangeSelectedPointerLevel(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && !Cheat.instance.whileEdit)
        {
            Cheat.instance.OnChangeSelectedPointerLevel(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) && !Cheat.instance.whileEdit)
        {
            Cheat.instance.OnChangeSelectedPointerLevel(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) && !Cheat.instance.whileEdit)
        {
            Cheat.instance.OnChangeSelectedPointerLevel(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) && !Cheat.instance.whileEdit)
        {
            Cheat.instance.OnChangeSelectedPointerLevel(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9) && !Cheat.instance.whileEdit)
        {
            Cheat.instance.OnChangeSelectedPointerLevel(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0) && !Cheat.instance.whileEdit)
        {
            Cheat.instance.OnChangeSelectedPointerLevel(9);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            var upgrade = FindUpgrade(UpgradeType.AddStick);
            upgrade.TryUpgrade();
            SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: 1);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            var upgrade = FindUpgrade(UpgradeType.AddStick);
            upgrade.TryUpgrade();

            SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: 2);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            var upgrade = FindUpgrade(UpgradeType.AddStick);
            upgrade.TryUpgrade();

            SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: 3);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            var upgrade = FindUpgrade(UpgradeType.AddStick);
            upgrade.TryUpgrade();

            SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: 4);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            var upgrade = FindUpgrade(UpgradeType.AddStick);
            upgrade.TryUpgrade();
            SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: 5);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            var upgrade = FindUpgrade(UpgradeType.AddStick);
            upgrade.TryUpgrade();

            SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: 6);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            var upgrade = FindUpgrade(UpgradeType.AddStick);
            upgrade.TryUpgrade();

            SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: 7);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            var upgrade = FindUpgrade(UpgradeType.AddStick);
            upgrade.TryUpgrade();

            SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: 8);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            var upgrade = FindUpgrade(UpgradeType.AddStick);
            upgrade.TryUpgrade();
            SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: 9);
        }
        else if (Input.GetKeyDown(KeyCode.J) && !Cheat.instance.whileEdit)
        {
            StageManager.instance.StageClear();
        }
        else if (Input.GetKeyDown(KeyCode.H) && !Cheat.instance.whileEdit)
        {
            SaveManager.instance.RemovePointer(pointerList[pointerList.Count - 1]);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            StartMergeTutorial();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            AdManager.instance.speedFever = !AdManager.instance.speedFever;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            pause = !pause;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            SaveManager.instance.AddRvTicket(10);
        }
#endif


        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 포인터의 위치에서 화면으로 광선을 쏴서 충돌 검사
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 광선이 어떤 물체와 충돌했는지 확인
            if (Physics.Raycast(ray, out hit))
            {
                // 충돌한 물체의 태그 또는 다른 식별자를 확인하여 특정 오브젝트에 대한 동작 수행
                if (hit.collider.CompareTag("Stopper"))
                {
                    hit.collider.GetComponent<Stopper>().OnClick();
                }
            }
        }

        float damage = 0;

        for (int i = 0; i < pointerList.Count; i++)
            damage += pointerList[i].damage;

        rpmText.text = "$" + FormatNumber(damage * (clockHandRotateSpeed + UPGRADE_SPEED_VALUE[speed.currentLevel]
             * (MainManager.instance.lastFever ? 5f : 1)
              * (AdManager.instance.speedFever ? 3f : 1f)
               * (MainManager.instance.onDragPointer ? 0.25f : 1)
                * (IapManager.instance.starterPack ? 3 : 1))) + " / SEC";
    }

    public Mesh GetNumberObject(int number)
    {
        if (number >= 0 && number <= 9)
            return numberMeshs[number];

        Debug.LogError("해당되는 숫자가 없습니다. " + number);
        return null;
    }

    public Mesh GetNumbericUnit(NumbericUnit unit)
    {
        switch (unit)
        {
            case NumbericUnit.K:
                return numbericUnits[0];

            case NumbericUnit.M:
                return numbericUnits[1];

            case NumbericUnit.B:
                return numbericUnits[2];

            case NumbericUnit.T:
                return numbericUnits[3];

            case NumbericUnit.None:
                return null;

            default:
                Debug.LogError("해당되는 숫자 단위가 없습니다. " + unit);
                return null;
        }
    }

    public void PadTouchEvent()
    {
        MMVibrationManager.Haptic(HapticTypes.LightImpact);

        if (AdManager.instance.autoFever && tapToSpeedUp.gameObject.activeSelf == false)
            return;

        if (touchSpeedUpDelay != null)
        {
            touchSpeedUpDelay.Kill();
            touchSpeedUpDelay = null;
        }

        touchSpeedUp = true;

        if (StageManager.instance.currentClock != null)
            foreach (var pointer in StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>())
                pointer.StartTrail();


        touchSpeedUpDelay = this.TaskDelay(clickSpeedBonusDuration, () =>
        {
            touchSpeedUp = false;
            touchSpeedUpDelay = null;

            if (StageManager.instance.currentClock != null)
                foreach (var pointer in StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>())
                    pointer.StopTrail();
        });

        if (StageManager.instance.currentClock.stop)
            StageManager.instance.currentClock.stop = false;

        tapToSpeedUp.gameObject.SetActive(false);

        if (ES3.KeyExists("EnableQuest") ? !ES3.Load<bool>("EnableQuest") : true)
        {
            ES3.Save<bool>("EnableQuest", true);
            QuestManager.instance.questUIBox.ShowAnimation();
        }
    }

    public void Upgrade_AddStick()
    {
        var upgrade = FindUpgrade(UpgradeType.AddStick);
        SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: IapManager.instance.lv2StickForever ? 2 : 1);

        upgrade.TryUpgrade();
        ES3.Save<int>("UPGRADE_ADDSTICK", upgrade.currentLevel);

        UpdateUI_Merge();

        Tutorial.insatnce.AddStickTutorialEnd();

        QuestManager.instance.OnProgressQuest(QuestType.AddNSticks);

        UpgradeCount();
    }

    public void Upgarde_Speed()
    {
        var upgrade = FindUpgrade(UpgradeType.Speed);
        upgrade.TryUpgrade();
        ES3.Save<int>("UPGRADE_SPEED", upgrade.currentLevel);

        QuestManager.instance.OnProgressQuest(QuestType.UpgradeSpeedNTimes);

        UpgradeCount();
    }

    public void Upgrade_Income()
    {
        var upgrade = FindUpgrade(UpgradeType.Income);
        upgrade.TryUpgrade();
        ES3.Save<int>("UPGRADE_INCOME", upgrade.currentLevel);

        QuestManager.instance.OnProgressQuest(QuestType.UpgradeIncome1Times);

        UpgradeCount();
    }

    public void UpgradeCount()
    {
        ES3.Save<int>("Upgrade Count", ES3.KeyExists("Upgrade Count") ? ES3.Load<int>("Upgrade Count") + 1 : 1);

        MondayOFF.EventTracker.TryStage(ES3.Load<int>("Upgrade Count"));
        MondayOFF.EventTracker.ClearStage(ES3.Load<int>("Upgrade Count"));
        EventManager.instance.CustomEvent(AnalyticsType.GAME, $"Upgrade Count - {(ES3.Load<int>("Upgrade Count"))}", true, true);
    }

    public GameObject SpawnPointer(int level = 1, bool affectQuest = true)
    {
        var upgrade = FindUpgrade(UpgradeType.AddStick);
        upgrade.TryUpgrade();
        return SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: level, affectQuest: affectQuest);
    }

    public GameObject SpawnPointer(float rotation, bool save = true, int level = 1, bool randomRotate = true, bool affectQuest = true)
    {
        var pointer = Instantiate(Resources.LoadAll<GameObject>("Pointer").Where((n) => n.GetComponent<ClockPointer>().Level == level).First(), StageManager.instance.currentClock.transform);
        pointer.transform.rotation = Quaternion.Euler(0, 0, randomRotate ? rotation + UnityEngine.Random.Range(-10f, 10f) : rotation);

        PopParticle("Pooling/Particle/Particle_Merge 1", new Vector3(0, 0, 0), Quaternion.Euler(0, 0, pointer.transform.rotation.eulerAngles.z));

        pointerList.Add(pointer.GetComponent<ClockPointer>());

        Guid newGuid = Guid.NewGuid();

        pointer.GetComponentInChildren<ClockPointer>().guid = newGuid.ToString();

        if (save)
            SaveManager.instance.clockPointers.Add(new ClockPointerSaveData() { guid = newGuid.ToString(), level = pointer.GetComponent<ClockPointer>().Level, spawnRotate = rotation });

        ES3.Save<List<ClockPointerSaveData>>("ClockPointers", SaveManager.instance.clockPointers);

        if (affectQuest)
            if (QuestManager.instance.currentQuest.type == QuestType.GetLvNStick)
                if (QuestManager.instance.currentQuest.goalRequest == level)
                    QuestManager.instance.OnProgressQuest(QuestType.GetLvNStick);

        if (level != 1)
        {
            var unlocked = ES3.KeyExists("UnlockedStick") ? ES3.Load<List<int>>("UnlockedStick") : new List<int>();
            if (!unlocked.Contains(level))
            {
                unlocked.Add(level);
                ES3.Save<List<int>>("UnlockedStick", unlocked);

                ES3.Save<bool>("NewStickInCollection", true);
                stickCollectionBtn.UpdateDot();
            }
        }

        UpdateUI_Merge();
        // onChangePointer.Invoke();
        return pointer;
    }

    public void OnClickMergeRV()
    {
        if (isMerge)
            return;

        var find = StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>().OrderBy((n) => n.Level);

        for (int i = 0; i < find.Count(); i++)
        {
            var find2 = StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>().Where((n) => n.Level == find.ToArray()[i].Level).ToArray();
            if (find2.Count() >= 3)
            {
                AdManager.instance.RV(() =>
                {


                    MergePointer(true);
                    // EventManager.instance.CustomEvent(AnalyticsType.RV, " Merge");
                }, " Merge");

                return;
            }
        }
    }

    public void MergePointer(bool RV = false)
    {
        var upgrade = FindUpgrade(UpgradeType.Merge);
        if ((GetCost(upgrade) > SaveManager.instance.money || isMerge) && !RV)
            return;

        var find = StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>().OrderBy((n) => n.Level);

        for (int i = 0; i < find.Count(); i++)
        {
            var find2 = StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>().Where((n) => n.Level == find.ToArray()[i].Level).ToArray();
            if (find2.Count() >= 3)
            {
                //만약 마지막 초침밖에 없을시
                if (find2[0].Level == Resources.LoadAll<GameObject>("Pointer").OrderByDescending((n) => n.GetComponent<ClockPointer>().Level).First().GetComponent<ClockPointer>().Level)
                    return;

                int nextLevel = find.ToArray()[i].Level + 1;

                Managers.Sound.Play("Sound/Pop");
                isMerge = true;

                if (!RV)
                    SaveManager.instance.UseMoney(GetCost(upgrade));

                find2[0].Stop = true;
                find2[1].Stop = true;
                find2[2].Stop = true;

                var rotationPos = StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length];
                // var rotationPos = FindClosestNumber(find2[0].transform.localRotation.z, StageManager.instance.currentClock.clockSpawnRotates);

                EventManager.instance.CustomEvent(AnalyticsType.GAME, " UPGRADE_MERGE");

                if (ES3.KeyExists("EnableQuest"))
                    this.TaskDelay(1.2f, () => QuestManager.instance.OnProgressQuest(QuestType.MergeNTimes));
                Tutorial.insatnce.mergeStickTutorialEnd();

                UpgradeCount();

                find2[0].StartTrail();
                find2[1].StartTrail();
                find2[2].StartTrail();

                find2[0].HighlightThisPointerON();
                find2[1].HighlightThisPointerON();
                find2[2].HighlightThisPointerON();

                if (_ITR_MODE)
                {
                    switch (ITRManager.instance.mergeType)
                    {
                        case MergeType.type1:
                            AutoMerge();
                            break;

                        case MergeType.type2:
                            AutoMerge(0.45f, 0);
                            break;
                    }
                }
                else
                {
                    AutoMerge();
                }

                void AutoMerge(float mergeSpeed = 1.2f, float _rotationOffset = 360)
                {
                    find2[0].transform.DOLocalRotate(new Vector3(0, 0, rotationPos + _rotationOffset), mergeSpeed, RotateMode.FastBeyond360).SetEase(mergeAnimationCurve);
                    find2[1].transform.DOLocalRotate(new Vector3(0, 0, rotationPos + _rotationOffset), mergeSpeed, RotateMode.FastBeyond360).SetEase(mergeAnimationCurve);
                    find2[2].transform.DOLocalRotate(new Vector3(0, 0, rotationPos - _rotationOffset), mergeSpeed, RotateMode.FastBeyond360).SetEase(mergeAnimationCurve).OnComplete(() =>
                    {
                        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);

                        SaveManager.instance.RemovePointer(find2[0]);
                        SaveManager.instance.RemovePointer(find2[1]);
                        SaveManager.instance.RemovePointer(find2[2]);

                        var newPointer = SpawnPointer(rotationPos, true, nextLevel, randomRotate: false);
                        newPointer.GetComponent<ClockPointer>().Stop = true;

                        newPointer.transform.localRotation = Quaternion.Euler(0, 0, rotationPos);
                        newPointer.GetComponent<ClockPointer>().Stop = false;

                        var upgrade = FindUpgrade(UpgradeType.Merge);
                        upgrade.TryUpgrade();
                        ES3.Save<int>("UPGRADE_MERGE", upgrade.currentLevel);

                        // if (QuestManager.instance.activeQuestList.Where((n) => n.questType == QuestType.GetLvNStick).Count() > 0)
                        // {
                        //     if (QuestManager.instance.FindQuest(QuestType.GetLvNStick).goalRequest[QuestManager.instance.FindQuestSaveData(QuestType.GetLvNStick).currentLevel] == nextLevel)
                        //         QuestManager.instance.OnProgressQuest(QuestType.GetLvNStick);
                        // }

                        Managers.Sound.Play("Sound/merge", volume: 0.5f);

                        this.TaskDelay(0.01f, () => UpdateUI_Merge());

                        if (!PlayerPrefs.HasKey("UpgardeRVActive"))
                        {
                            PlayerPrefs.SetInt("UpgardeRVActive", 1);
                            UpgardeRVActive = true;
                        }

                        // if (!ES3.KeyExists("UpgardeRVActive"))
                        //     ES3.Save<bool>("UpgardeRVActive", true);

                        isMerge = false;

                        if (!ES3.KeyExists("MergeStick_" + nextLevel) && PopupEnable)
                        {

                            ES3.Save<bool>("MergeStick_" + nextLevel, true);
                            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Reach On New Stick - " + nextLevel, true, true);

                            if (!ES3.KeyExists("StickCollectionEnable"))
                            {
                                stickCollectionBtn.ShowAnimation();
                                ES3.Save<bool>("StickCollectionEnable", true);
                            }

                            isReadyOnRV = true;

                            this.TaskDelay(1f, () =>
                                {
                                    var popUp = Instantiate(Resources.Load<GameObject>("UI/NewStickPopUp"), UI);
                                    popUp.GetComponentInChildren<NewStickUnlockedPopUp>().Init(nextLevel, () =>
                                    {
                                        if (nextLevel == 3)
                                        {
                                            // this.TaskDelay(10f, () => { AdManager.instance.moneyFeverBtn.GetComponentInParent<RvSideUI>(true).Show(); });
                                            // this.TaskDelay(20f, () => AdManager.instance.autoFeverBtn.GetComponentInParent<RvSideUI>(true).Show());
                                        }
                                    });
                                });

                            if (nextLevel == 3 && !ES3.KeyExists("IAP_ShowiapOffer_StarterPack") && false)
                            {
                                ES3.Save<bool>("IAP_ShowiapOffer_StarterPack", true);
                                this.TaskDelay(5f, () =>
                                {
                                    IapManager.instance.GenerateIapPopup("UI/iapOffer_StarterPack", IapManager.iap_starterPack);
                                });
                            }

                            //새로운 랩의 초침을 얻었을경우
                        }
                    });
                }

                return;
            }

        }
    }

    public void MergePointerInDrag(ClockPointer pointer1, ClockPointer pointer2)
    {
        var rotationPos = pointer1.transform.rotation.eulerAngles.z;
        var nextLevel = pointer1.Level + 1;

        // PopParticle("Pooling/Particle/Particle_Merge", new Vector3(0, 0, 0), Quaternion.Euler(0, 0, rotationPos));
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);

        Managers.Sound.Play("Sound/Pop");

        // Destroy(pointer1.gameObject);
        // Destroy(pointer2.gameObject);
        SaveManager.instance.RemovePointer(pointer1);
        SaveManager.instance.RemovePointer(pointer2);

        var newPointer = SpawnPointer(rotationPos, true, nextLevel, randomRotate: false);
        newPointer.GetComponent<ClockPointer>().Stop = true;

        newPointer.transform.localRotation = Quaternion.Euler(0, 0, rotationPos);
        newPointer.GetComponent<ClockPointer>().Stop = false;

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Merge Stick - " + nextLevel, true, true);

        if (!PlayerPrefs.HasKey("UpgardeRVActive"))
        {
            PlayerPrefs.SetInt("UpgardeRVActive", 1);
            UpgardeRVActive = true;
        }

        // if (!ES3.KeyExists("UpgardeRVActive"))
        //     ES3.Save<bool>("UpgardeRVActive", true);

        if (!ES3.KeyExists("MergeStick_" + nextLevel))
        {

            ES3.Save<bool>("MergeStick_" + nextLevel, true);
            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Reach On New Stick - " + nextLevel, true, true);

            isReadyOnRV = true;

            this.TaskDelay(1f, () =>
                {

                    var popUp = Instantiate(Resources.Load<GameObject>("UI/NewStickPopUp"), UI);
                    popUp.GetComponentInChildren<NewStickUnlockedPopUp>().Init(nextLevel, () =>
                    {
                        if (nextLevel == 3)
                        {
                            this.TaskDelay(10f, () => { AdManager.instance._moneyFeverBtn.Show(); });
                            this.TaskDelay(20f, () => AdManager.instance._autoFeverBtn.Show());
                        }
                    });
                });

            if (nextLevel == 3)
            {
                AdManager.instance.enableIS = false;

                if (!ES3.KeyExists("moneyFeverReady"))
                {
                    ES3.Save<bool>("moneyFeverReady", true);
                }

                if (!ES3.KeyExists("autoFeverReady"))
                {
                    ES3.Save<bool>("autoFeverReady", true);
                }
            }
        }
    }

    public void UpdateUI_Merge()
    {
        var find = StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>().OrderBy((n) => n.Level);

        for (int i = 0; i < find.Count(); i++)
        {
            var find2 = StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>().Where((n) => n.Level == find.ToArray()[i].Level).ToArray();

            // Debug.LogError(Resources.LoadAll<GameObject>("Pointer").OrderByDescending((n) => n.GetComponent<ClockPointer>().Level).ToArray()[0].GetComponent<ClockPointer>().Level);

            if (find2.Count() >= 3 && find2[0].Level != Resources.LoadAll<GameObject>("Pointer").OrderByDescending((n) => n.GetComponent<ClockPointer>().Level).ToArray()[0].GetComponent<ClockPointer>().Level)
            {
                mergeSlot.SetActive(true);
                // Debug.LogError(find2.Count() + " " + find2[0].gameObject.name + " " + find2[1].gameObject.name);

                // Debug.LogError("TREASD");


                return;
            }
        }

        mergeSlot.SetActive(false);
    }

    //파티클 오브젝트풀로 생성
    public Poolable PopParticle(string path, Vector3 pos, Quaternion rotation, Transform parent = null, float pushDelay = 5f)
    {
        var particle = Managers.Pool.Pop(Resources.Load<GameObject>(path), parent == null ? particleParent : parent);
        particle.transform.position = pos;
        particle.transform.localRotation = rotation;

        this.TaskDelay(pushDelay, () => Managers.Pool.Push(particle));

        return particle;
    }

    //돈 메쉬 파티클 생성
    public void GenerateMoneyMesh(float value, Vector3 pos)
    {
        var money = Managers.Pool.Pop(Resources.Load<GameObject>("MoneyDropMesh"), particleParent);
        money.transform.position = pos;
        money.transform.localScale = new Vector3(1.5f, 2f, 1.5f);
        money.transform.DOScale(Vector3.zero, 1.5f);

        var rb = money.GetComponent<Rigidbody>();
        float randomForceX = UnityEngine.Random.Range(-1f, 1f);
        float randomForceY = UnityEngine.Random.Range(3, 5f);
        rb.AddForce(new Vector3(randomForceX, randomForceY, 0f), ForceMode.Impulse);
        // X축에 랜덤한 회전을 추가

        float randomRotationSpeed = UnityEngine.Random.Range(-10, 10);
        // rb.AddTorque(new Vector3(0f, 0f, randomRotationSpeed), ForceMode.Impulse);

        money.GetComponent<MoneyDropMesh>().ChangeValue(Mathf.FloorToInt(value));

        this.TaskDelay(5f, () => Managers.Pool.Push(money.GetComponent<Poolable>()));
    }

    //돈 추가 파티클 UI
    public void GenerateMoneyText(float value, Vector3 pos, string path = "MoneyParticleText")
    {
        var money = Managers.Pool.Pop(Resources.Load<GameObject>(path), particleParent);
        money.transform.position = pos;
        money.GetComponentInChildren<MoneyPopUpText>().Init(value);
    }

    public void GeneratePointerRVPopUp()
    {

    }

    public static float FindClosestNumber(float target, float[] numbers)
    {
        if (numbers == null || numbers.Length == 0)
        {
            throw new System.ArgumentException("The array is null or empty.");
        }

        float closestNumber = numbers[0];
        float minDifference = Mathf.Abs(target - closestNumber);

        for (int i = 1; i < numbers.Length; i++)
        {
            float currentDifference = Mathf.Abs(target - numbers[i]);

            if (currentDifference < minDifference)
            {
                minDifference = currentDifference;
                closestNumber = numbers[i];
            }
        }

        return closestNumber;
    }

    public bool lastFever = false;
    public GameObject lastFeverGameob;

    public void StartLastFever()
    {
        lastFeverGameob.SetActive(true);
        lastFever = true;

        this.TaskDelay(3f, () => lastFeverGameob.SetActive(false));

        foreach (var pointer in StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>())
        {
            pointer.SetTrailLength();
        }
    }

    public void EndLastFever()
    {
        lastFeverGameob.SetActive(false);
        lastFever = false;

        foreach (var pointer in StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>())
        {
            pointer.SetTrailLength();
        }
    }


    public void GenerateUIParticleAttractor(Transform end, Transform start, UnityEngine.Events.UnityAction onAttract = null, System.Action OnCompleteParticle = null)
    {
        var attractor = Managers.Pool.Pop(Resources.Load<GameObject>("Particles/UIAttractor"), UI);

        attractor.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

        attractor.GetComponent<UIAttractorCustom>().InitParticle(Resources.Load<Material>("UIParticle_Money"), 1, new ParticleSystem.Burst(0, 20));
        attractor.GetComponent<UIAttractorCustom>().StartEmit(end, start, onAttract, OnCompleteParticle);

        this.TaskDelay(5f, () => { if (attractor != null) Managers.Pool.Push(attractor); });
    }

    public void GenerateUIParticleAttractor(Transform end, Transform start, ParticleSystem.Burst burst, Material material, Transform parent, UnityEngine.Events.UnityAction onAttract = null, System.Action OnCompleteParticle = null)
    {
        var attractor = Managers.Pool.Pop(Resources.Load<GameObject>("Particles/UIAttractor"), parent);

        attractor.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

        attractor.GetComponent<UIAttractorCustom>().InitParticle(material, 1, burst);
        attractor.GetComponent<UIAttractorCustom>().StartEmit(end, start, onAttract, OnCompleteParticle);

        this.TaskDelay(5f, () => { if (attractor != null) Managers.Pool.Push(attractor); });
    }

    public static void GenerateUIParticleAttractorFromWorldSpace(Transform end, Transform start, UnityEngine.Events.UnityAction onAttract = null, System.Action OnCompleteParticle = null)
    {
        var attractor = Managers.Pool.Pop(Resources.Load<GameObject>("Particles/UIAttractor_NumKill"), MainManager.instance.UI);

        Vector3 screenPosition = MainManager.instance.mainCamera.WorldToScreenPoint(start.position);

        var viewPort = MainManager.instance.mainCamera.WorldToViewportPoint(start.position);

        // 스크린 좌표를 Canvas 상의 UI 좌표로 변환
        RectTransform canvasRect = MainManager.instance.UI.GetComponent<RectTransform>(); // 여기에는 해당 Canvas의 RectTransform을 넣어야 합니다.
        // Vector2 uiPosition;
        Vector3 uiPosition2;

        // RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, MainManager.instance.mainCamera, out uiPosition);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, screenPosition, MainManager.instance.UICamera, out uiPosition2);

        // 여기에서 uiPosition을 사용하여 UI 요소의 위치를 업데이트할 수 있습니다.
        // 예를 들면 RectTransform.anchoredPosition을 사용할 수 있습니다.
        // 예시:
        // uiElementRectTransform.anchoredPosition = uiPosition;

        attractor.GetComponent<UIAttractorCustom>().InitParticle(Resources.Load<Material>("UIParticle_Money"), 1, new ParticleSystem.Burst(0, 20));
        attractor.GetComponent<UIAttractorCustom>().StartEmit(end, uiPosition2, onAttract, OnCompleteParticle);

        // Debug.LogError(uiPosition2);

        MainManager.instance.TaskDelay(5f, () => { if (attractor != null) Managers.Pool.Push(attractor); });

        // Debug.LogError(start.gameObject);
        // Debug.LogError(screenPosition);
        // Debug.LogError(uiPosition2);
    }

    public void NewStickPopUp()
    {

    }

    public static string FormatNumber(double number)
    {
        string suffix = "";
        double formattedNumber = number;

        if (number < 1000)
            return ((int)number).ToString();

        if (number >= 1e3 && number < 1e6)
        {
            suffix = "K";
            formattedNumber = number / 1e3;
        }
        else if (number >= 1e6 && number < 1e9)
        {
            suffix = "M";
            formattedNumber = number / 1e6;
        }
        else if (number >= 1e9 && number < 1e12)
        {
            suffix = "B";
            formattedNumber = number / 1e9;
        }
        else if (number >= 1e12)
        {
            suffix = "T";
            formattedNumber = number / 1e12;
        }

        // 소수점 이하 두 자리까지 표시
        string formattedString = string.Format("{0:0.0}{1}", formattedNumber, suffix);

        return formattedString;
    }


    [FoldoutGroup("MergeTutorial")] public Transform tutoralFinger;
    [FoldoutGroup("MergeTutorial")] public Vector3 offset;
    [FoldoutGroup("MergeTutorial")] public GameObject mergetheStickText;



    public void StartMergeTutorial()
    {
        ES3.Save<bool>("MergeTutorial", true);

        tutorial1 = true;
        mergetheStickText.SetActive(true);

        Vector2 startPos = RectTransformUtility.WorldToScreenPoint(mainCamera, (pointerList[0].transform.up * 0.5f) + offset);
        Vector2 endPos = RectTransformUtility.WorldToScreenPoint(mainCamera, (pointerList[1].transform.up * 0.5f) + offset);

        pauseBtn.btnImage.sprite = pauseBtn.playImage;

        // Vector2 resultStart;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(UI.GetComponent<RectTransform>(), startPos, UICamera, out resultStart);
        // Vector2 resultEnd;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(UI.GetComponent<RectTransform>(), endPos, UICamera, out resultEnd);


        print(startPos + " " + endPos);

        tutoralFinger.GetComponent<RectTransform>().anchoredPosition = startPos - new Vector2(Screen.width / 2f, Screen.height / 2f);
        tutoralFinger.gameObject.SetActive(true);

        tutoralFinger.GetComponent<RectTransform>().DOAnchorPos(endPos - new Vector2(Screen.width / 2f, Screen.height / 2f), 1).SetLoops(-1, LoopType.Restart);
    }

    public void CompleteMergeTutorial()
    {
        tutorial1 = false;
        mergetheStickText.SetActive(false);
        tutoralFinger.gameObject.SetActive(false);

        pauseBtn.btnImage.sprite = pauseBtn.puaseImage;

        pauseBtn.gameObject.SetActive(true);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Complte Merge Tutorial", true, true);
    }

    public void StartPointerDeagTutorial()
    {

    }


    //머지 가능한 초침들만 하이라이트
    public void Focus_SelectedStickAndMergeableStick(int level)
    {
        var find = pointerList.Where((n) => n.Level != level).ToList();

        for (int i = 0; i < find.Count; i++)
        {
            find[i]._renderer.materials = find[i].GetCurrectMaterial(true);
        }


        // pointerList.Where((n) => n.Level != level).ToList().ForEach((n) => n.GetComponent<MeshRenderer>().material = Resources.Load<Material>("T_Main Trans"));
    }

    public void Unfocus_SelectedStickAndMergeableStick()
    {
        for (int i = 0; i < pointerList.Count; i++)
        {
            pointerList[i]._renderer.materials = pointerList[i].GetCurrectMaterial(false);
        }

        // pointerList.ForEach((n) => n.GetComponent<MeshRenderer>().material = Resources.Load<Material>("T_Main"));
    }

    public void Focus_OnlySelectedStick(ClockPointer stick)
    {
        var find = pointerList.Where((n) => n != stick).ToList();
        for (int i = 0; i < find.Count; i++)
        {
            find[i]._renderer.materials = find[i].GetCurrectMaterial(true);
        }
        // pointerList.Where((n) => n != stick).ToList().ForEach((n) => n.GetComponent<MeshRenderer>().material = Resources.Load<Material>("T_Main Trans"));
    }

    public void ForceMerge()
    {
        MergePointer();
    }

    public void ForceAddStick()
    {
        var upgrade = FindUpgrade(UpgradeType.AddStick);
        SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[upgrade.currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: Cheat.instance.selectedSpawnPointerLevel);

        upgrade.TryUpgrade();
        ES3.Save<int>("UPGRADE_ADDSTICK", upgrade.currentLevel);

        UpdateUI_Merge();
    }

    [Button("ITR_MODE", ButtonSizes.Large)]
    public void ITR_MODE()
    {
        PopupEnable = false;
        _ITR_MODE = true;
        Tutorial.tutorialEnable = false;
        Debug.LogError(Tutorial.tutorialEnable);
    }


    [Button("Build_Mode", ButtonSizes.Large)]
    public void Build_Mode()
    {
        PopupEnable = true;
        _ITR_MODE = false;
        Tutorial.tutorialEnable = true;
        Debug.LogError(Tutorial.tutorialEnable);
    }

    [SerializeField] private Transform moveMotionImage;

    public void GenerateImageMotion(Image image, Transform startPos, Transform endPos)
    {
        moveMotionImage.gameObject.SetActive(true);

        moveMotionImage.GetComponent<Image>().sprite = image.sprite;
        moveMotionImage.GetComponent<RectTransform>().sizeDelta = image.GetComponent<RectTransform>().sizeDelta;
        moveMotionImage.localScale = image.transform.localScale;

        moveMotionImage.transform.position = startPos.position;

        moveMotionImage.DOScale(Vector3.zero, 1f);
        moveMotionImage.transform.DOMove(endPos.position, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            moveMotionImage.gameObject.SetActive(false);
        });
    }

    public void ShowShopUI(float view = 1)
    {
        var ui = Instantiate(Resources.Load<GameObject>("UI/SHopUI"), UI);
        if (!Tutorial.insatnce.rvTicketTutorial)
            this.TaskDelay(0.1f, () => ui.GetComponent<ShopUI>().MoveScrollViewToTargetPosition(view));
    }

    public void Cheat_SpawnStick(int level) => SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[0], true, level, true);

    public int GetCost(Upgrade upgrade)
    {
        // Debug.LogError((float)upgrade.cost[upgrade.currentLevel] + " " + (StageManager.instance.currentStage == 0 ? 1 : (1f + (StageManager.instance.currentStage * 0.5f))));

        if (upgrade.currentLevel >= upgrade.cost.Length - 1)
            return upgrade.cost[upgrade.cost.Length - 1];
        else
            return (int)((float)upgrade.cost[upgrade.currentLevel] * (StageManager.instance.currentStage == 0 ? 1 : (1f + (StageManager.instance.currentStage * 0.5f))));
    }
}
