using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;
using UnityEditor;
using Unity.VisualScripting;

public enum QuestType
{
    AddNSticks,
    MergeNTimes,
    UpgradeSpeedNTimes,
    UpgradeIncome1Times,
    TapNTimes,
    GetLvNStick,
    MoveLvNStickToAnotherArea,
    Destory1Number,
    DestoryClock
}

[System.Serializable]
public class Quest
{
    public QuestType questType;

    public int[] goalRequest;

    public int GetMaxRequestLevel() => goalRequest.Length - 1;

    public int priority = 0;
    // public int[] ClearReward;
}

[System.Serializable]
public class _Quest
{
    public QuestType type;
    public int goalRequest;
    // public int reward;

    public List<QuestReward> questRewardList = new List<QuestReward>();
}

public enum questRewardType
{
    money,
    rvTicket
}

[System.Serializable]
public class QuestReward
{
    public questRewardType rewardType;
    public int value;
}

[System.Serializable]
public class _QuestSaveData
{
    public QuestType type;
    public int currentLevel;
    public int currentProgress;

    public bool complete = false;

    public void ImproveProgress()
    {
        if (complete)
            return;

        currentProgress++;

        CheckComplete();
    }

    [Button]
    public void ForceComplete()
    {
        CompleteQuest();
    }

    public void CheckComplete()
    {
        if (QuestManager.instance._questSaveData.currentLevel >= QuestManager.instance._questList.Count)
        {
            //모든 퀘스트 완료 퀘스트 UI 숨기기
            QuestManager.instance.questBox.SetActive(false);
        }
        else
        if (currentProgress >= QuestManager.instance._questList[currentLevel].goalRequest)
            CompleteQuest();
    }

    public void CompleteQuest()
    {
        complete = true;
        QuestManager.instance.CompleteQuest();
    }

    public void AfterClameReward()
    {
        complete = false;

        // if (QuestManager.instance.FindQuest(type).GetMaxRequestLevel() >= currentLevel)
        currentLevel++;
        currentProgress = 0;
    }

    public void ResetQuest()
    {
        currentLevel = 0;
        currentProgress = 0;
    }
}

public class QuestManager : MonoBehaviour
{
    const string questReward = "https://docs.google.com/spreadsheets/d/1J0GjFQgmBfoDdVmSXLzVQBLNvKKzg1uZDw28Ygwls-Y/edit?hl=ko#gid=0";

    [HideInInspector] public List<Quest> questList = new List<Quest>();
    [HideInInspector] public List<Quest> activeQuestList = new List<Quest>();
    [HideInInspector] public List<QuestSaveData> questSaveDatas = new List<QuestSaveData>();

    public Quest FindQuest(QuestType type) => questList.Find((n) => n.questType == type);
    public QuestSaveData FindQuestSaveData(QuestType type) => questSaveDatas.Find((n) => n.type == type);
    public Quest FindActiveQuest(QuestType type) => questList.Find((n) => n.questType == type);

    [HideInInspector][FoldoutGroup("Quest v1")] public Transform parent;
    [HideInInspector][FoldoutGroup("Quest v1")] public QuestSideUI questSideUI;
    [HideInInspector][FoldoutGroup("Quest v1")] public QuestUI questUI;

    [Space]
    [HideInInspector][FoldoutGroup("Quest v1")] public GameObject selectedQuestParent;
    [HideInInspector][FoldoutGroup("Quest v1")] public QuestType selectedQuest;
    [HideInInspector][FoldoutGroup("Quest v1")] public Text shortProgressText;
    [HideInInspector][FoldoutGroup("Quest v1")] public Image shortProgressFill;

    [Space]
    [HideInInspector][FoldoutGroup("Quest v1")] public QuestType completeSideBoxQuest = (QuestType)(-1);
    [HideInInspector][FoldoutGroup("Quest v1")] public GameObject sideBox;
    [HideInInspector][FoldoutGroup("Quest v1")] public Transform sideBoxClameBtn;
    [HideInInspector][FoldoutGroup("Quest v1")] public Text sideBoxText;

    [Space]
    [FoldoutGroup("Quest v2")] public GameObject questBox;
    [FoldoutGroup("Quest v2")] public Text questText;
    [FoldoutGroup("Quest v2")] public Text rewardValueText;
    [FoldoutGroup("Quest v2")] public Image rewardMoneyImage;
    [FoldoutGroup("Quest v2")] public Image rewardTicketImage;
    [FoldoutGroup("Quest v2")] public Image progressFill;
    [FoldoutGroup("Quest v2")] public Text completeMoneyText;
    [FoldoutGroup("Quest v2")] public GameObject completeBox;
    [FoldoutGroup("Quest v2")] public QuestUIBox questUIBox;


    [Space]
    [FoldoutGroup("Quest v2")] public List<_Quest> _questList;
    [FoldoutGroup("Quest v2")] public _Quest currentQuest;
    [FoldoutGroup("Quest v2")] public _QuestSaveData _questSaveData;




    private QuestType lastQuestType = QuestType.Destory1Number;


    // public int[] Quest_AddStick_Request;
    // public int[] Quest_AddStick_Reward;

    // public int[] Quest_Merge_Request;
    // public int[] Quest_Merge_Reward;

    // public int[] Quest_UpgradeSpeed_Request;
    // public int[] Quest_UpgradeSpeed_Reward;

    // public int[] Quest_UpgradeIncome_Request;
    // public int[] Quest_UpgradeIncome_Reward;

    // public int[] Quest_Tap_Request;
    // public int[] Quest_Tap_Reward;

    // public int[] Quest_GetStick_Request;
    // public int[] Quest_GetStick_Reward;

    // public int[] Quest_MoveStick_Request;
    // public int[] Quest_MoveStick_Reward;

    // public int[] Quest_Destroy_Request;
    // public int[] Quest_Destroy_Reward;

    public static QuestManager instance;

    private void Awake()
    {
        instance = this;

        questSaveDatas = ES3.KeyExists("QuestSaveData") ? ES3.Load<List<QuestSaveData>>("QuestSaveData") : questSaveDatas;

        if (!ES3.KeyExists("QuestCurrentSaveData"))
        {
            currentQuest = _questList[0];
            _questSaveData.type = currentQuest.type;
            ES3.Save<_QuestSaveData>("QuestCurrentSaveData", _questSaveData);
        }
        else
        {
            _questSaveData = ES3.Load<_QuestSaveData>("QuestCurrentSaveData");
            _questSaveData.CheckComplete();
            FindNextQuest();
        }

        UpdateUI();
        UpdateQuestProgress();

        this.TaskWaitUntil(() =>
        {
            GenerateActiveQuest();
            if (ES3.KeyExists("EnableQuest"))
                ES3.Save<bool>("", true);

            SelectShortProgressUI();
            FindNextSideBoxQuest();
        }, () => StageManager.instance != null);
    }

    private void Start()
    {

    }

    [Button]
    public void ForceRandomProgress()
    {
        OnProgressQuest(activeQuestList.First().questType);
    }


    public void GenerateActiveQuest(bool reset = false)
    {
        if (reset)
            activeQuestList.Clear();

        int count = activeQuestList.Count;
        foreach (var quest in questList.Where((n) => FindQuestSaveData(n.questType).currentLevel <= FindQuest(n.questType).GetMaxRequestLevel())
        .OrderBy((n) => GetQuestReward(n.questType)))
        {
            if (count == 4)
                break;

            if (!activeQuestList.Contains(quest))
            {
                if (quest.questType == QuestType.MoveLvNStickToAnotherArea)
                {
                    if (ES3.KeyExists("PauseTutorial"))
                    {
                        activeQuestList.Add(quest);
                        count++;
                    }
                    else
                    {

                    }
                }
                else
                {
                    // Debug.LogError(quest.questType + " " + FindQuestSaveData(quest.questType).currentLevel + " " + FindQuest(quest.questType).GetMaxRequestLevel());
                    activeQuestList.Add(quest);
                    count++;
                }
            }
        }
    }

    // public void OnProgressQuest(QuestType type)
    // {
    //     if (ES3.KeyExists("EnableQuest") ? !ES3.Load<bool>("EnableQuest") : true)
    //         return;

    //     var find = FindQuestSaveData(type);

    //     if (!activeQuestList.Contains(FindQuest(type)))
    //         return;

    //     if (type == QuestType.GetLvNStick)
    //         find.ForceComplete();
    //     else
    //         find.ImproveProgress();

    //     if (type != QuestType.TapNTimes)
    //         SaveQuest();

    //     UpdateQuestUI();
    //     UpdateShortProgressUI(type);
    // }

    public void SaveQuest()
    {
        ES3.Save<List<QuestSaveData>>("QuestSaveData", questSaveDatas);
    }

    public void CompleteQuest(QuestType type)
    {
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "QuestComplete - " + type + "_" + FindQuestSaveData(type).currentLevel, true, true);

        if (completeSideBoxQuest == (QuestType)(-1))
        {
            completeSideBoxQuest = type;
            UpdateSideBoxUI();
        }
    }

    public void UpdateSideBoxUI()
    {
        sideBox.SetActive(true);
        sideBoxText.text = GetQuestDesctiption(completeSideBoxQuest);
    }

    public void OnClickClameBtn(QuestType type)
    {
        ClameReward(type);

        if (questUI != null)
            questUI.UpdateUI();
    }

    public void OnClickSideBoxClameBtn()
    {
        ClameReward(completeSideBoxQuest);
        MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.moneyIcon.transform, sideBoxClameBtn);
    }

    private void ClameReward(QuestType type)
    {
        var save = FindQuestSaveData(type);
        var find = FindQuest(type);

        ES3.Save<int>("ClameQuestRewardCount", ES3.KeyExists("ClameQuestRewardCount") ? ES3.Load<int>("ClameQuestRewardCount") + 1 : 1);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, $"ClameQuestReward - {type}_{FindQuestSaveData(type).currentLevel}", true, true);
        EventManager.instance.CustomEvent(AnalyticsType.GAME, $"Clame Quest Reward Count - {(ES3.Load<int>("ClameQuestRewardCount"))}", true, true);

        SaveManager.instance.AddMoney(GetQuestReward(type));

        activeQuestList.Remove(find);
        save.AfterClameReward();
        GenerateActiveQuest();

        if (selectedQuest == type)
            SelectShortProgressUI();

        if (type == completeSideBoxQuest)
            FindNextSideBoxQuest();

        lastQuestType = type;

        SaveQuest();
    }

    public void FindNextSideBoxQuest()
    {
        completeSideBoxQuest = (QuestType)(-1);

        var find = activeQuestList.Where((n) => FindQuestSaveData(n.questType).complete);
        if (find.Count() > 0)
        {
            completeSideBoxQuest = find.First().questType;

            UpdateQuestUI();
            UpdateSideBoxUI();
        }
        else
        {
            sideBox.SetActive(false);
        }
    }

    public void ResetQuest()
    {
        questSaveDatas.ForEach((n) => n.ResetQuest());

        GenerateActiveQuest();
        SelectShortProgressUI();
        FindNextSideBoxQuest();
    }

    //Quest UI
    public void ShowQuestUI()
    {
        var questUI = Instantiate(Resources.Load<GameObject>("UI/Quest UI"), parent).GetComponentInChildren<QuestUI>();

        questUI.Init();

        this.questUI = questUI;
    }

    public void HideQuestUI()
    {
        questUI = null;
    }

    public void UpdateQuestUI()
    {
        if (questUI != null)
            questUI.UpdateUI();
    }

    public void SelectShortProgressUI()
    {
        if (activeQuestList.Count() == 0)
        {
            selectedQuestParent.SetActive(false);
            return;
        }
        else
        {
            selectedQuestParent.SetActive(true);
        }

        var find1 = activeQuestList.Where((n) => FindQuest(n.questType).goalRequest[FindQuestSaveData(n.questType).currentLevel] >= 2 && n.questType != lastQuestType);

        QuestType find2;

        if (find1.Count() == 0)
        {
            find2 = activeQuestList.OrderBy((n) => FindQuestSaveData(n.questType).currentLevel).First().questType;
        }
        else
        {
            find2 = find1.OrderBy((n) => FindQuestSaveData(n.questType).currentLevel).First().questType;
            // Debug.LogError(find1.Count() + " " + find2);
        }

        selectedQuest = activeQuestList.Where((n) => GetQuestReward(find2) == GetQuestReward(n.questType)).OrderBy((n) => FindQuest(n.questType).priority).First().questType;

        UpdateShortProgressUI(selectedQuest);
        // selectedQuest = questUI.GetComponentsInChildren<QuestSlot>().OrderBy((n) => FindQuestSaveData(n._GetType).currentLevel).First()._GetType;
    }

    public void UpdateShortProgressUI(QuestType type)
    {
        if (type == selectedQuest)
        {
            var saveData = FindQuestSaveData(type);
            var find = FindQuest(type);

            shortProgressText.text = GetQuestDesctiption(type);

            shortProgressFill.fillAmount = (float)saveData.currentProgress / (float)find.goalRequest[saveData.currentLevel];
        }
    }

    public string GetQuestDesctiption(QuestType type)
    {
        var saveData = FindQuestSaveData(type);
        var find = FindQuest(type);

        switch (type)
        {
            case QuestType.AddNSticks:
                return "Add " + find.goalRequest[saveData.currentLevel] + " Stick";

            case QuestType.MergeNTimes:
                return "Merge " + find.goalRequest[saveData.currentLevel] + " Stick";

            case QuestType.UpgradeSpeedNTimes:
                return "Upgrade Speed " + find.goalRequest[saveData.currentLevel] + " Times";

            case QuestType.UpgradeIncome1Times:
                return "Upgrade Income " + find.goalRequest[saveData.currentLevel] + " Times";

            case QuestType.TapNTimes:
                return "Tap " + find.goalRequest[saveData.currentLevel] + " Times";

            case QuestType.GetLvNStick:
                return "Get lv." + find.goalRequest[saveData.currentLevel] + " Stick";

            case QuestType.MoveLvNStickToAnotherArea:
                return "Move " + find.goalRequest[saveData.currentLevel] + " Sticks";

            case QuestType.Destory1Number:
                return "Break " + find.goalRequest[saveData.currentLevel] + " Numers";

            default:
                return null;
        }
    }



    public int GetQuestReward(QuestType type)
    {
        var saveData = FindQuestSaveData(type);
        var find = FindQuest(type);

        int n = find.goalRequest[saveData.currentLevel];
        int m = saveData.currentLevel;
        int l = StageManager.instance.currentStage + 1;

        int result = 0;

        switch (type)
        {
            case QuestType.AddNSticks:
                result = (int)((200 + (Mathf.Pow(n - 1, 2)) * (m + 1) * 100)) * l;
                break;

            case QuestType.MergeNTimes:
                result = (int)((200 + (Mathf.Pow(n - 1, 2)) * (m + 1) * 200)) * l;
                break;

            case QuestType.UpgradeSpeedNTimes:
                result = (int)((200 + (Mathf.Pow(n - 1, 2)) * (m + 1) * 300)) * l;
                break;

            case QuestType.UpgradeIncome1Times:
                result = (int)((200 + Mathf.Pow(m, 2) * 100)) * l;
                break;

            case QuestType.TapNTimes:
                result = (int)((300 + (Mathf.Pow(m - 1, 2)) * 100)) * l;
                break;

            case QuestType.GetLvNStick:
                result = (int)(300 + ((Mathf.Pow(n - 1, 2)) * (100 * (n - 1)))) * l; //300+{(N-1)^2}*{100*(N-1)}
                break;

            case QuestType.MoveLvNStickToAnotherArea:
                result = (int)(100 + (n - 1) * 100) * l;
                break;

            case QuestType.Destory1Number:
                result = (int)(500 + (Mathf.Pow(m, 2) * 200)) * l;
                break;

            default:
                return 0;
        }

        return (int)(result * 0.5f);
    }


    [Button("Force Progress")]
    public void OnProgressQuest(QuestType type)
    {
        if (ES3.KeyExists("EnableQuest") ? !ES3.Load<bool>("EnableQuest") : true)
            return;

        if (currentQuest.type != type)
            return;

        if (type == QuestType.GetLvNStick)
            _questSaveData.ForceComplete();
        else
            _questSaveData.ImproveProgress();

        if (type != QuestType.TapNTimes)
            _SaveQuest();

        UpdateUI();
        UpdateQuestProgress();
    }

    public void UpdateQuestProgress()
    {
        progressFill.fillAmount = (float)_questSaveData.currentProgress / (float)currentQuest.goalRequest;
    }

    [Button("Force Complete")]
    public void CompleteQuest()
    {
        if (currentQuest == null)
            return;

        if (!ES3.KeyExists("QuestTutorial"))
            Tutorial.insatnce.StartQuestTutorial();

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Quest - Complete Quest", true, true);
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Quest - Complete Quest - " + _questSaveData.currentLevel, true, true);

        completeBox.gameObject.SetActive(true);
    }
#if UNITY_EDITOR
    [MenuItem("Custome/ClameReward")]
    public static void addTicket()
    {
        MainManager.instance.rvTicketText.gameObject.SetActive(true);

        MainManager.instance.TaskDelay(0.1f, () =>
        {
            MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.rvTicketIcon.transform
                                        , QuestManager.instance.questBox.transform
                                        , new ParticleSystem.Burst(0, 1)
                                        , Resources.Load<Material>("UIParticle_RVTicket")
                                        , QuestManager.instance.questBox.transform);
        });

    }
#endif

    public void ClameReward()
    {
        if (Tutorial.insatnce.questTutorial)
            Tutorial.insatnce.EndQuestTutorial();

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Quest - ClameReward", true, true);
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Quest - ClameReward - " + _questSaveData.currentLevel, true, true);

        if (_questSaveData.currentLevel == 2)
        {
            AdManager.instance.StartBoostRvShowing();
            // AdManager.instance.StartBoostRvShowing();
        }

        // if (_questSaveData.currentLevel == 3)
        // {
        //     this.TaskDelay(5, () => Tutorial.insatnce.StartRVTicketTutorial());


        //     // AdManager.instance.StartBoostRvShowing();
        // }

        foreach (var reward in currentQuest.questRewardList)
        {
            switch (reward.rewardType)
            {
                case questRewardType.money:
                    SaveManager.instance.AddMoney(reward.value);
                    MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.moneyIcon.transform, questBox.transform);
                    break;

                case questRewardType.rvTicket:
                    SaveManager.instance.AddRvTicket(reward.value);

                    MainManager.instance.rvTicketText.gameObject.SetActive(true);
                    MainManager.instance.TaskDelay(0.1f, () =>
                            {
                                MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.rvTicketIcon.transform
            , questBox.transform
            , new ParticleSystem.Burst(0, reward.value)
            , Resources.Load<Material>("UIParticle_RVTicket")
            , questBox.transform);
                            });
                    break;

            }
        }

        _questSaveData.AfterClameReward();
        FindNextQuest();

        completeBox.SetActive(false);

        UpdateUI();
        UpdateQuestProgress();

    }

    void FindNextQuest()
    {
        if (_questSaveData.currentLevel >= _questList.Count)
        {
            //모든 퀘스트 완료 퀘스트 UI 숨기기

            questBox.SetActive(false);
        }
        else
        {
            InitCurrentQuest(_questList[_questSaveData.currentLevel]);

            _questSaveData.type = currentQuest.type;
        }

        _SaveQuest();
    }

    public void SkipQuest(QuestType questType)
    {
        var find = _questList.Where((n) => _questList.IndexOf(n) > _questSaveData.currentLevel && n.type == questType);

        // Debug.LogError(_questSaveData.currentLevel);
        // Debug.LogError(find.Count());

        if (find.Count() > 0)
        {
            InitCurrentQuest(_questList[_questList.IndexOf(find.First()) + 1]);
            _questSaveData.currentProgress = 0;
            _questSaveData.type = currentQuest.type;
            _questSaveData.currentLevel = _questList.IndexOf(find.First()) + 1;
            _SaveQuest();
            UpdateUI();
            UpdateQuestProgress();
        }
    }

    void InitCurrentQuest(_Quest saveData)
    {
        currentQuest.type = saveData.type;
        currentQuest.goalRequest = saveData.goalRequest;
        currentQuest.questRewardList = saveData.questRewardList;
    }

    public void UpdateUI()
    {
        questText.text = GetQuestDesctiption(currentQuest);

        if (currentQuest.questRewardList.Count > 0)
        {
            switch (currentQuest.questRewardList[0].rewardType)
            {
                case questRewardType.money:
                    rewardMoneyImage.gameObject.SetActive(true);
                    rewardTicketImage.gameObject.SetActive(false);
                    questUIBox.moneyIcon.gameObject.SetActive(true);
                    questUIBox.rvTicketIcon.gameObject.SetActive(false);
                    break;

                case questRewardType.rvTicket:
                    rewardMoneyImage.gameObject.SetActive(false);
                    rewardTicketImage.gameObject.SetActive(true);
                    questUIBox.moneyIcon.gameObject.SetActive(false);
                    questUIBox.rvTicketIcon.gameObject.SetActive(true);
                    break;
            }
            completeMoneyText.text = "+" + currentQuest.questRewardList[0].value.ToString();
            questUIBox.rewardText.text = "+" + currentQuest.questRewardList[0].value.ToString();
        }
    }

    public void _SaveQuest()
    {
        ES3.Save<_QuestSaveData>("QuestCurrentSaveData", _questSaveData);
    }

    public string GetQuestDesctiption(_Quest quest)
    {
        switch (quest.type)
        {
            case QuestType.AddNSticks:
                return "Add " + quest.goalRequest + " Stick";

            case QuestType.MergeNTimes:
                return "Merge " + quest.goalRequest + " Times";

            case QuestType.UpgradeSpeedNTimes:
                return "Upgrade Speed X" + quest.goalRequest;

            case QuestType.UpgradeIncome1Times:
                return "Upgrade Income X" + quest.goalRequest;

            case QuestType.TapNTimes:
                return "Tap " + quest.goalRequest + " Times";

            case QuestType.GetLvNStick:
                return "Get " + Resources.LoadAll<StickObjects>("Pointer").Where((n) => n.level == quest.goalRequest).First()._name + " Stick";

            case QuestType.MoveLvNStickToAnotherArea:
                return "Move " + quest.goalRequest + " Sticks";

            case QuestType.Destory1Number:
                return "Break " + quest.goalRequest + " Numer";

            case QuestType.DestoryClock:
                return "Break the clock!";

            default:
                return null;
        }
    }


    [Button("Load Quest Datas", ButtonSizes.Large)]
    public void LoadCSV_Gear_Online()
    {
        StartCoroutine(LoadCSV_Gear_OnlineRoutine());
    }

    public IEnumerator LoadCSV_Gear_OnlineRoutine()
    {
        UnityWebRequest www = UnityWebRequest.Get(questReward);
        yield return www.SendWebRequest();

        string csvString = www.downloadHandler.text;
        Debug.LogWarning(csvString);

        CSVReader.Read(ConvertStringToTextAsset(csvString));

        var list = CSVReader.Read(ConvertStringToTextAsset(csvString));


        // Quest_AddStick_Reward = new int[list.Count];
        // Quest_Merge_Reward = new int[list.Count];
        // Quest_UpgradeSpeed_Reward = new int[list.Count];
        // Quest_UpgradeIncome_Reward = new int[list.Count];
        // Quest_Tap_Reward = new int[list.Count];
        // Quest_GetStick_Reward = new int[list.Count];
        // Quest_MoveStick_Reward = new int[list.Count];
        // Quest_Destroy_Reward = new int[list.Count];

        for (int i = 0; i < list.Count; i++)
        {
            Dictionary<string, object> rewards = list[i];

            foreach (var test in rewards)
            {
                print(test.Key + " " + test.Value);
            }

            // Quest_AddStick_Reward[i] = (int)list[i]["Add N Sticks"];
            // Quest_Merge_Reward[i] = (int)list[i]["Merge N times"];
            // Quest_UpgradeSpeed_Reward[i] = (int)list[i]["Upgrade Speed N times"];
            // Quest_UpgradeIncome_Reward[i] = (int)list[i]["Upgrade Income 1 times"];
            // Quest_Tap_Reward[i] = (int)list[i]["Tap N times"];
            // Quest_GetStick_Reward[i] = (int)list[i]["Get lvN Stick"];
            // Quest_MoveStick_Reward[i] = (int)list[i]["Move lvN Stick to another area"];
            // Quest_Destroy_Reward[i] = (int)list[i]["Destroy 1 Number"];
        }
    }

    public TextAsset ConvertStringToTextAsset(string st)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(st);

        TextAsset textAsset = new TextAsset(System.Text.Encoding.UTF8.GetString(bytes));

        Debug.Log("Converted TextAsset: " + textAsset.text);
        return textAsset;
    }

    public void UIOnOff()
    {
        questUIBox.gameObject.SetActive(!questUIBox.gameObject.activeSelf);
    }
}
