using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

[System.Serializable]
public class Stage
{
    public GameObject stagePrefab;
}

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    public Stage[] stages;

    public int currentStage;
    public Clock currentClock;

    [SerializeField] private CanvasGroup playUI;
    [SerializeField] private CanvasGroup clearUI;

    public List<ClockPointer> GetCurrentClockPointerList() => currentClock.GetComponentsInChildren<ClockPointer>().ToList();

    public int lastHighestPointerLevel;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentStage = ES3.KeyExists("currentStage") ? ES3.Load<int>("currentStage") : 0;
        LoadStage(currentStage);

        //저장되어있는 초침 소환하기
        SaveManager.instance.clockPointers = ES3.KeyExists("ClockPointers") ? ES3.Load<List<ClockPointerSaveData>>("ClockPointers") : new List<ClockPointerSaveData>();

        for (int i = 0; i < SaveManager.instance.clockPointers.Count; i++)
        {
            var pointer = Instantiate(Resources.LoadAll<GameObject>("Pointer").Where((n) => n.GetComponent<ClockPointer>().Level == SaveManager.instance.clockPointers[i].level).First(), StageManager.instance.currentClock.transform);
            pointer.transform.rotation = Quaternion.Euler(0, 0, SaveManager.instance.clockPointers[i].lastRotation);
            pointer.GetComponentInChildren<ClockPointer>().guid = SaveManager.instance.clockPointers[i].guid;

            MainManager.instance.pointerList.Add(pointer.GetComponent<ClockPointer>());
        }

        currentClock?.onAfterStageStart?.Invoke();
    }

    public void LoadStage(int stage)
    {
        currentClock = Instantiate(stages[stage % stages.Length].stagePrefab).GetComponent<Clock>();

        currentClock.Init();
        currentClock.UpdateUI();

        if (StageManager.instance.currentClock.GetComponentsInChildren<ClockNum>().Count() == 1)
        {
            MainManager.instance.StartLastFever();
        }
    }

    public void HideStage()
    {
        if (currentClock != null)
            Destroy(currentClock.gameObject);
    }

    public void StageClear()
    {
        //===================Old===============================//
        // playUI.gameObject.SetActive(false);
        // clearUI.gameObject.SetActive(true);
        // clearUI.DOFade(1, 1f);

        // MondayOFF.EventTracker.ClearStage(currentStage + 1);
        // EventManager.instance.CustomEvent(AnalyticsType.GAME, "stageClear - " + currentStage);

        // if (currentStage < stages.Length - 1)
        //     currentStage++;
        // else
        //     currentStage = 0;

        // ES3.Save<int>("currentStage", currentStage);

        // MainManager.instance.EndLastFever();

        // SaveManager.instance.ClearMoney();
        //===================Old===============================//

        MondayOFF.EventTracker.ClearStage(currentStage + 1);

        if (QuestManager.instance.currentQuest.type != QuestType.DestoryClock)
            QuestManager.instance.SkipQuest(QuestType.DestoryClock);

        // if (currentStage < stages.Length - 1)
        currentStage++;
        // else
        //     currentStage = 0;

        ES3.Save<int>("currentStage", currentStage);
        MainManager.instance.EndLastFever();
        // SaveManager.instance.ClearMoney();

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "stageClear - " + currentStage);
        lastHighestPointerLevel = MainManager.instance.pointerList.OrderBy((n) => n.Level).First().Level;

        GameObject parts;

        playUI.gameObject.SetActive(false);

        this.TaskDelay(0.5f, () =>
        {
            SaveManager.instance.ResetPointer();
            MainManager.instance.ResetUpgrade();
            MainManager.instance.pointerList.Clear();
            currentClock.ClockSaveReset();

            if (currentClock != null)
                HideStage();

            parts = Instantiate(currentClock.destoryPartObject);
            parts.GetComponent<ClockDummyExplosion>().ExplostionParts();

            this.TaskDelay(5f, () => Destroy(parts));
        });

        this.TaskDelay(1.5f, () =>
        {
            ES3.Save<List<ClockPointerSaveData>>("ClockPointers", SaveManager.instance.clockPointers);

            currentClock = Instantiate(stages[currentStage % stages.Length].stagePrefab, new Vector3(7, 0, 0), Quaternion.identity).GetComponent<Clock>();

            currentClock.transform.DOMoveX(0, 1f).OnComplete(() =>
            {
                currentClock.Init();
                currentClock.UpdateUI();
                MainManager.instance.SpawnPointer(level: IapManager.instance.lv2StickForever ? 2 : 1, false);

                MainManager.instance.ResetUpgrade();

                playUI.gameObject.SetActive(true);
                MainManager.instance.tapToSpeedUp.SetActive(true);

                currentClock?.onAfterStageStart?.Invoke();

                for (int i = 0; i < MainManager.instance.rvSideUIs.Length; i++)
                {
                    MainManager.instance.rvSideUIs[i].Start();
                    MainManager.instance.moneyOfferSideUI.Show();
                }

            });

            QuestManager.instance.OnProgressQuest(QuestType.DestoryClock);

        });
    }

    public void NextStage()
    {
        if (currentClock != null)
            HideStage();

        SaveManager.instance.clockPointers.Clear();

        clearUI.alpha = 0;
        playUI.gameObject.SetActive(true);
        clearUI.gameObject.SetActive(false);

        SaveManager.instance.ResetPointer();
        MainManager.instance.ResetUpgrade();
        MainManager.instance.pointerList.Clear();

        QuestManager.instance.ResetQuest();

        LoadStage(currentStage);

        MainManager.instance.tapToSpeedUp.SetActive(true);
    }
}
