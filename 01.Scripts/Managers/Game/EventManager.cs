using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MondayOFF;
using System;


public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private int playtime = 0;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        if (ES3.KeyExists("playtime"))
            playtime = ES3.Load<int>("playtime");
        else
            CustomEvent(AnalyticsType.PLAYTIME, " - " + playtime.ToString(), true, true);

        this.TaskWhile(30, 0, () =>
        {
            playtime += 30;
            ES3.Save<int>("playtime", playtime);
            CustomEvent(AnalyticsType.PLAYTIME, " - " + playtime.ToString(), true, true);
        });

        EnterGame();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("FLAG_TYPE", "PAUSE");
#if UNITY_ANDROID
            dic.Add("OS_TYPE", "AOS");
#endif
#if UNITY_IOS
            dic.Add("OS_TYPE", "IOS");
#endif
            EventTracker.LogEvent($"GAME_FLAG", dic);
            TimeEvent("FLAG_TYPE", "PAUSE");
        }
    }

    public void CustomEvent(AnalyticsType type, string additionInfo, bool timeEvent = false, bool stageNum = false)
    {
        var dic = new Dictionary<string, object>();
        dic.Add("FLAG_TYPE", $"{type} - {additionInfo}");
#if UNITY_ANDROID
        dic.Add("OS_TYPE", "AOS");
#endif
#if UNITY_IOS
        dic.Add("OS_TYPE", "IOS");
#endif

        if (timeEvent)
        {
            dic.Add("TIME", playtime.ToString());
            // TimeEvent("FLAG_TYPE", $"{type} - {additionInfo}");
        }

        if (stageNum)
        {
            if (StageManager.instance != null)
                dic.Add("STAGENUM", StageManager.instance.currentStage.ToString());
            // StageNum("FLAG_TYPE", $"{type} - {additionInfo}");
        }
#if UNITY_EDITOR
        Debug.Log("<color=blue>[Firebase]</color> Custome Event - " + $"{type} - {additionInfo}");
#endif
        EventTracker.LogEvent($"GAME_FLAG", dic);
    }

    void TimeEvent(string paramName, string value)
    {
        var dic = new Dictionary<string, object>();
        dic.Add(paramName, value);
        dic.Add("TIME", playtime.ToString());
#if UNITY_ANDROID
        dic.Add("OS_TYPE", "AOS");
#endif
#if UNITY_IOS
        dic.Add("OS_TYPE", "IOS");
#endif

        EventTracker.LogEvent($"TIME_TRACE", dic);
    }

    void StageNum(string paramName, string value)
    {
        var dic = new Dictionary<string, object>();
        dic.Add(paramName, value);

        dic.Add("STAGENUM", StageManager.instance.currentStage.ToString());
#if UNITY_ANDROID
        dic.Add("OS_TYPE", "AOS");
#endif
#if UNITY_IOS
        dic.Add("OS_TYPE", "IOS");
#endif

        EventTracker.LogEvent($"STAGE_TRACE", dic);
    }


    public static string FirstEnterDateTimeString
    {
        get
        {
            return PlayerPrefs.GetString("FirstEnterDateTimeString", "NoData");
        }
        set
        {
            PlayerPrefs.SetString("FirstEnterDateTimeString", value);
        }
    }

    public static int TodayRetentionCount
    {
        get
        {
            return PlayerPrefs.GetInt("TodayRetentionCount", 0);
        }
        set
        {
            PlayerPrefs.SetInt("TodayRetentionCount", value);
        }
    }

    public static int NextDayRetentionCount
    {
        get
        {
            return PlayerPrefs.GetInt("NextDayRetentionCount", 0);
        }
        set
        {
            PlayerPrefs.SetInt("NextDayRetentionCount", value);
        }
    }

    public static void EnterGame()
    {
        if (FirstEnterDateTimeString == "NoData")
        {
            Debug.LogWarning("FirstEnter");
            FirstEnterDateTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            TodayRetentionCount = 0;
            NextDayRetentionCount = 0;
        }
        else
        {
            int nextDayRetentionCount = (int)(((int)(DateTime.Now - Convert.ToDateTime(FirstEnterDateTimeString)).TotalHours) / 24f);// 며칠 차 접속이니?

            if (NextDayRetentionCount != nextDayRetentionCount)
            {

                //새로운 날인듯
                NextDayRetentionCount = nextDayRetentionCount;
                TodayRetentionCount = 0;
            }
            else
            {
                //당일 재접속
                TodayRetentionCount++;
            }
        }

        Debug.LogWarning("RetentionData" + NextDayRetentionCount + "/" + TodayRetentionCount);
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Retention - " + NextDayRetentionCount + " - " + TodayRetentionCount);
    }

    public static DateTime GetRecentDateTime()
    {
        if (PlayerPrefs.GetString("RecentEnterGameDateString", "firstEnter") == "firstEnter")
        {
            return DateTime.Now;
        }
        else
        {
            return Convert.ToDateTime(PlayerPrefs.GetString("RecentEnterGameDateString", "firstEnter"));
        }
    }

    public static void SetRecentEnterDateString(string st)
    {
        PlayerPrefs.SetString("RecentEnterGameDateString", st);
    }
}


public enum AnalyticsType
{
    GAME,
    UI,
    IAP,
    RV,
    ADS,
    AB_TEST,
    TEST,
    PLAYTIME
}
