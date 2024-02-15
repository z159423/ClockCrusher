using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.Linq;
using System;


public enum SaveTextType
{
    Money,
    RVTicket
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [FoldoutGroup("Money")] public System.Action onChangeMoneyEvent;
    [FoldoutGroup("Money")] public List<Text> moneyTextList = new List<Text>();
    [FoldoutGroup("Money")] public float money;

    [FoldoutGroup("RV Ticket")] public string dailyFreeRvTicketTime = "2000-01-01 01:01:01";
    [FoldoutGroup("RV Ticket")] public System.Action onChangeRVTicketEvent;
    [FoldoutGroup("RV Ticket")] public List<Text> rvTicketTextList = new List<Text>();
    [FoldoutGroup("RV Ticket")] public List<RVBtnUI> rvBtnUIList = new List<RVBtnUI>();
    [FoldoutGroup("RV Ticket")] public int rvTicket;

    public List<ClockPointerSaveData> clockPointers = new List<ClockPointerSaveData>();



    [Button("ForceInit")]
    private void Awake()
    {
        instance = this;

        onChangeMoneyEvent += OnChangeMoney;
        onChangeRVTicketEvent += OnChangeRvTicket;

        if (!ES3.KeyExists("ClearZero"))
        {
            MondayOFF.EventTracker.ClearStage(0);
            ES3.Save("ClearZero", 1);
        }

        if (ES3.KeyExists("dailyRvTicketTime"))
            dailyFreeRvTicketTime = ES3.Load<string>("dailyRvTicketTime");
    }

    private void Start()
    {
        LoadMoney();
        LoadRvTicket();

        this.TaskWhile(1f, 10, SavePointerRotation);
        this.TaskWhile(1f, 1.5f, SaveClockNums);

        if (addstick == null)
            addstick = MainManager.instance.FindUpgrade(UpgradeType.AddStick);

        if (mergeStick == null)
            mergeStick = MainManager.instance.FindUpgrade(UpgradeType.Merge);
    }

    public void AddText(SaveTextType type, Text text)
    {
        switch (type)
        {
            case SaveTextType.Money:
                moneyTextList.Add(text);
                onChangeMoneyEvent.Invoke();
                break;

            case SaveTextType.RVTicket:
                rvTicketTextList.Add(text);
                onChangeRVTicketEvent.Invoke();
                break;

            default:
                Debug.LogError("해당되는 타입이 없습니다.");
                break;
        }
    }

    public void RemoveText(SaveTextType type, Text text)
    {
        switch (type)
        {
            case SaveTextType.Money:
                moneyTextList.Remove(text);
                break;

            case SaveTextType.RVTicket:
                rvTicketTextList.Remove(text);
                break;

            default:
                Debug.LogError("해당되는 타입이 없습니다.");
                break;
        }
    }

    public void SaveClockNums()
    {
        if (StageManager.instance.currentClock != null)
            for (int i = 0; i < StageManager.instance.currentClock.GetClockNums().Length; i++)
                StageManager.instance.currentClock.GetClockNums()[i].Save();
    }

    //==========================Money=================================

    Upgrade addstick = null;
    Upgrade mergeStick = null;

    public void AddMoney(float value)
    {
        money += value;

        if (!PlayerPrefs.HasKey("AddStickTutorial"))
        {
            // if (!ES3.KeyExists("AddStickTutorial"))
            // {
            if (money >= addstick.GetCurrentCost())
            {
                Tutorial.insatnce.AddStickTutorialStart();
                PlayerPrefs.SetInt("AddStickTutorial", 1);
                PlayerPrefs.Save();
            }
        }

        if (!PlayerPrefs.HasKey("mergeStickTutorial"))
        {
            // if (!ES3.KeyExists("mergeStickTutorial"))
            // {
            if (mergeStick.GetCurrentCost() <= money)
            {

                var find = MainManager.instance.pointerList.OrderBy((n) => n.Level);

                for (int i = 0; i < find.Count(); i++)
                {
                    var find2 = MainManager.instance.pointerList.Where((n) => n.Level == find.ToArray()[i].Level).ToArray();

                    if (find2.Count() >= 3)
                    {
                        Tutorial.insatnce.mergeStickTutorialStart();
                        PlayerPrefs.SetInt("mergeStickTutorial", 1);
                        PlayerPrefs.Save();
                        break;
                    }
                }
            }
        }

        // SaveMoney();
        onChangeMoneyEvent.Invoke();
    }

    public void UseMoney(float value)
    {
        if (money - value < 0)
        {
            // Debug.LogError("돈이 부족합니다!");
            return;
        }

        money -= value;

        SaveMoney();
        onChangeMoneyEvent.Invoke();
    }

    public void SaveMoney()
    {
        // ES3.Save<float>("Money", money);
        PlayerPrefs.SetFloat("Money", money);
    }

    public void LoadMoney()
    {
        // money = ES3.KeyExists("Money") ? ES3.Load<float>("Money") : 0;
        money = PlayerPrefs.HasKey("Money") ? PlayerPrefs.GetFloat("Money") : 0;

        onChangeMoneyEvent.Invoke();
    }

    public void ClearMoney()
    {
        money = 0;

        SaveMoney();
        onChangeMoneyEvent.Invoke();
    }

    public void OnChangeMoney()
    {
        // moneyTextList.ForEach((n) => n.text = MainManager.FormatNumber(Mathf.FloorToInt(money)).ToString());

        for (int i = 0; i < moneyTextList.Count; i++)
        {
            moneyTextList[i].text = MainManager.FormatNumber(Mathf.FloorToInt(money)).ToString();
            // moneyTextList[i].text = money.ToString();
        }
    }

    //==========================RV Ticket=================================

    public void AddRvTicket(int value)
    {
        rvTicket += value;
        SaveRvTicket();
    }

    public bool UseRVTicket(int value)
    {
        if (rvTicket <= 0)
            return false;

        rvTicket -= value;

        rvTicket = Mathf.Clamp(rvTicket, 0, int.MaxValue);
        SaveRvTicket();
        return true;
    }

    public void SaveRvTicket()
    {
        ES3.Save<int>("RVTicket", rvTicket);

        onChangeRVTicketEvent?.Invoke();
        MainManager.instance.rvTicketText.SetActive(true);
    }

    public void LoadRvTicket()
    {
        rvTicket = ES3.KeyExists("RVTicket") ? ES3.Load<int>("RVTicket") : 0;

        onChangeRVTicketEvent?.Invoke();
    }

    public void ClearRvTicket()
    {
        rvTicket = 0;
        SaveRvTicket();
    }

    public void OnChangeRvTicket()
    {
        rvTicketTextList.ForEach((n) => n.text = rvTicket.ToString());
        rvBtnUIList.ForEach((n) => n.UpdateUI());
    }

    public void AddRvBtn(RVBtnUI btn)
    {
        rvBtnUIList.Add(btn);
    }

    public void RemoveRvBtn(RVBtnUI btn)
    {
        rvBtnUIList.Remove(btn);
    }







    public void RemovePointer(ClockPointer pointer)
    {
        var find = clockPointers.Where((n) => n.guid == pointer.guid);

        if (find.Count() > 0)
        {
            MainManager.instance.pointerList.Remove(pointer);
            clockPointers.Remove(find.First());
        }

        Destroy(pointer.gameObject);

        ES3.Save<List<ClockPointerSaveData>>("ClockPointers", clockPointers);
    }

    public void ResetPointer()
    {
        clockPointers.Clear();
        ES3.Save<List<ClockPointerSaveData>>("ClockPointers", clockPointers);

        ES3.DeleteKey("ClockPointers");
    }

    public void SavePointerRotation()
    {
        if (StageManager.instance.currentClock != null)
        {
            foreach (var pointer in StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>())
            {
                var find = clockPointers.Where((n) => n.guid == pointer.guid);
                if (find.Count() > 0)
                {
                    find.First().lastRotation = pointer.transform.localRotation.eulerAngles.z;
                }
            }

            ES3.Save<List<ClockPointerSaveData>>("ClockPointers", clockPointers);
        }
    }

    public bool IsTimeLimitRVReady(string time)
    {
        DateTime dateTime;
        double timeDiff;
        if (DateTime.TryParse(time, out dateTime))
        {
            // 정상적으로 파싱된 경우
            timeDiff = GetTimeDiff(DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;

            if (timeDiff < 86400f)
                return false;
            else
                return true;
        }
        else
        {
            // 파싱에 실패한 경우
            return false;
        }
    }

    public static System.TimeSpan GetTimeDiff(System.DateTime time)
    {
        System.TimeSpan timeDiff = System.DateTime.Now - time;

        return timeDiff;
    }

    public double GetLeftTime(string time)
    {
        return 28800 - GetTimeDiff(DateTime.ParseExact(time, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
    }

    public static string GetFormatedStringFromSecond(int second)
    {
        int hours = second / 3600;
        int minutes = (second % 3600) / 60;
        int remainingSeconds = second % 60;

        // 문자열 형식으로 변환
        string formattedTime = $"{hours:D2}:{minutes:D2}:{remainingSeconds:D2}";

        return formattedTime;
    }
}
