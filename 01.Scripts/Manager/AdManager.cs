using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEditor;

public enum RVType
{
    MoneyFever,
    AutoFever,
    SpeedFever
}

public class AdManager : MonoBehaviour
{
    public static AdManager instance;

    public GameObject isShowTimer;
    public Text isShowTimerText;
    public Text afterISRewardText;
    public int isShowTimerInt = 5;
    public int isSec = 3;
    public bool isShowing = false;
    public bool enableIS = true;

    [Space]

    public bool moneyFever = false;
    public float moneyFeverDuration = 30f;
    public float moenyFeverMultifly = 2f;
    public RvSideUI _moneyFeverBtn;

    [Space]

    public bool autoFever = false;
    public float autoFeverDuration = 30f;
    public RvSideUI _autoFeverBtn;

    [Space]

    public bool speedFever = false;
    public float speedFeverDuration = 30f;
    public RvSideUI _speedFeverBtn;

    [Space]

    public MoneyOfferSideUI moneyOffer;

    [Space]

    public Transform IRMoneyStartPoint;
    public Transform IRMoneyEndPoint;
    // public UIAttractorCustom IRMoneyParticle;

    private Coroutine isCor = null;

    private bool rvDelay = false;

    public int isCount = 0;

    public int GetIsReward(int add = 0) => ((isCount + 1 + add) * 90);

    public bool GetFeverActive(RVType type)
    {
        switch (type)
        {
            case RVType.MoneyFever:
                return moneyFever;

            case RVType.AutoFever:
                return autoFever;

            case RVType.SpeedFever:
                return speedFever;
        }

        return false;
    }

    private void Awake()
    {
        instance = this;

        isCount = ES3.KeyExists("ISCount") ? ES3.Load<int>("ISCount") : 0;

        if (ES3.KeyExists("EnableIS"))
            ES3.Save<bool>("EnableIS", true);
    }

    private void Start()
    {
        MondayOFF.AdsManager.OnAfterInterstitial -= AfterIs;
        MondayOFF.AdsManager.OnAfterInterstitial += AfterIs;

        void AfterIs()
        {
            isShowTimerInt = isSec;
            if (isShowTimerText != null)
                isShowTimerText.text = "AD BREAK IN " + isShowTimerInt + " SEC";
            if (isShowTimer != null)
                isShowTimer.SetActive(false);
            isShowing = false;

            SaveManager.instance.AddMoney(GetIsReward(-1));

            // print(GetIsReward(-1));

            if (!ShopUI.shopUIActive)
            {
                MainManager.instance.GenerateUIParticleAttractor(IRMoneyEndPoint, IRMoneyStartPoint);
            }

            if ((ES3.KeyExists("ISCount") ? ES3.Load<int>("ISCount") : 0) == 2)
            {
                //StartBoostRvShowing()
            }

            if (!ES3.KeyExists("FirstRV"))
            {
                ES3.Save<bool>("FirstRV", true);

                EventManager.instance.CustomEvent(AnalyticsType.GAME, "First IS", true, true);
            }
        }
        MondayOFF.AdsManager.OnAfterRewarded -= StopIsCoruition;

        MondayOFF.AdsManager.OnAfterRewarded += StopIsCoruition;

        if (!ES3.KeyExists("MoneyOffer"))
        {
            ES3.Save<bool>("MoneyOffer", true);
            this.TaskDelay(67, () => moneyOffer.Show());
        }
        else
        {
            this.TaskDelay(37, () => moneyOffer.Show());
        }
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // MainManager.instance.GenerateUIParticleAttractor(IRMoneyEndPoint, IRMoneyStartPoint);

        }
    }
#endif

    //부스트 RV 들 보여주기 시작
    public void StartBoostRvShowing()
    {
        MainManager.instance.TaskDelay(3, () => { AdManager.instance._speedFeverBtn.Show(); });
        MainManager.instance.TaskDelay(30f, () => AdManager.instance._autoFeverBtn.Show());
        // MainManager.instance.TaskDelay(20f, () => { AdManager.instance.moneyFeverBtn.GetComponentInParent<RvSideUI>(true).Show(); });

        AdManager.instance.enableIS = false;

        if (!ES3.KeyExists("speedFeverReady"))
        {
            ES3.Save<bool>("speedFeverReady", true);
        }

        if (!ES3.KeyExists("autoFeverReady"))
        {
            ES3.Save<bool>("autoFeverReady", true);
        }
    }

    public void OnClickRVbtn(RVType type)
    {
        // EventManager.instance.CustomEvent(AnalyticsType.RV, "-" + type, true, true);

        switch (type)
        {
            case RVType.MoneyFever:
                if (moneyFever)
                    return;

                AdManager.instance.RV(() =>
                        {
                            MoneyFever();
                        }, "-" + type);

                break;

            case RVType.AutoFever:
                if (autoFever)
                    return;

                AdManager.instance.RV(() =>
                                        {
                                            AutoFever();
                                        }, "-" + type);

                break;

            case RVType.SpeedFever:
                if (speedFever)
                    return;

                AdManager.instance.RV(() =>
                                        {
                                            SpeedFever();
                                        }, "-" + type);

                break;
        }
    }

    public void OnClickFreeChance(RVType type)
    {
        switch (type)
        {
            case RVType.MoneyFever:
                if (moneyFever)
                    return;

                MoneyFever();
                break;

            case RVType.AutoFever:
                if (autoFever)
                    return;

                AutoFever();
                break;

            case RVType.SpeedFever:
                if (speedFever)
                    return;

                SpeedFever();
                break;
        }
    }

    public void MoneyFever(bool freeChance = false)
    {
        moneyFever = true;

        _moneyFeverBtn.ActiveFever(moneyFeverDuration);
    }

    public void AutoFever(bool freeChance = false)
    {
        autoFever = true;
        _autoFeverBtn.ActiveFever(autoFeverDuration);
    }

    public void SpeedFever(bool freeChance = false)
    {
        speedFever = true;
        _speedFeverBtn.ActiveFever(speedFeverDuration);
    }

    public void DeActiveFever(RVType type)
    {
        switch (type)
        {
            case RVType.MoneyFever:
                moneyFever = false;
                break;

            case RVType.AutoFever:
                autoFever = false;
                break;

            case RVType.SpeedFever:
                speedFever = false;
                break;
        }
    }
#if UNITY_EDITOR

    [MenuItem("Custome/ForceIS")]
    static void ForceIS()
    {
        MondayOFF.AdsManager.SetIsTime(Time.realtimeSinceStartup - 100);
        MondayOFF.AdsManager.ShowInterstitial();
    }
    #endif

    public void ShowInterstitial()
    {

        // Debug.LogError(!isShowing);
        // Debug.LogError(MondayOFF.AdsManager.IsInterstitialReady());
        // Debug.LogError(MondayOFF.AdsManager.GetTimeUntilNextInterstitial() <= 0);
        // Debug.LogError(StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>().Where((n) => n.Level >= 3).Count() > 0);
        // Debug.LogError(!MainManager.instance.isReadyOnRV);
        // Debug.LogError((ES3.KeyExists("EnableIS") ? ES3.Load<bool>("EnableIS") : false));
        // Debug.LogError(enableIS);
        // Debug.LogError(!MainManager.instance._ITR_MODE);


        if (!isShowing && MondayOFF.AdsManager.IsInterstitialReady() && MondayOFF.AdsManager.GetTimeUntilNextInterstitial() <= 0
        && StageManager.instance.currentClock.GetComponentsInChildren<ClockPointer>().Where((n) => n.Level >= 3).Count() > 0
        && !MainManager.instance.isReadyOnRV && (ES3.KeyExists("EnableIS") ? ES3.Load<bool>("EnableIS") : false) && enableIS && !MainManager.instance._ITR_MODE
        && !IapManager.instance.starterPack && !MondayOFF.NoAds.IsNoAds)
        {
            isShowTimer.SetActive(true);
            isShowing = true;

            isCor = StartCoroutine(Count());

            afterISRewardText.text = "+ " + GetIsReward().ToString();

            IEnumerator Count()
            {
                while (isShowTimerInt >= 0)
                {
                    yield return new WaitForSeconds(1f);

                    isShowTimerInt -= 1;
                    isShowTimerText.text = "AD BREAK IN " + isShowTimerInt + " SEC";

                    if (isShowTimerInt <= 0)
                    {
                        if (MondayOFF.AdsManager.IsInterstitialReady() && !rvDelay)
                            MondayOFF.AdsManager.ShowInterstitial();

                        isShowTimer.SetActive(false);

                        isCount += 1;

                        ES3.Save<int>("ISCount", isCount);
                        if (isCor != null)
                            StopCoroutine(isCor);
                        isCor = null;

                        if ((isCount % 3 == 0) && !MondayOFF.NoAds.IsNoAds && !IapManager.instance.starterPack)
                        {
                            this.TaskDelay(1f, () =>
                            {
                                IapManager.instance.GenerateIapPopup("UI/iapOffer_noAdsPack", "clockcrusher_noads");
                            });
                        }
                    }
                }
            }

            //             this.TaskWhile(1, 0, () =>
            // {
            //     isShowTimerInt -= 1;
            //     isShowTimerText.text = "AD BREAK IN 5 SEC";

            //     if (isShowTimerInt <= 0)
            //         AdManager.instance.ShowInterstitial();

            // }
            //             , () => isShowTimerInt <= 0);

        }
    }

    public void StopIsCoruition()
    {
        if (isCor != null)
            StopCoroutine(isCor);
        isCor = null;

        isShowTimerInt = isSec;
        if (isShowTimerText != null)
            isShowTimerText.text = "AD BREAK IN " + isShowTimerInt + " SEC";
        if (isShowTimer != null)
            isShowTimer.SetActive(false);
        isShowing = false;
    }


    public void RV(System.Action onAfterReward, string tag, System.Action onWatchedAds = null, System.Action onUseTicket = null)
    {
        if (SaveManager.instance.rvTicket > 0)
        {
            onUseTicket?.Invoke();
            SaveManager.instance.UseRVTicket(1);

            onAfterReward?.Invoke();

            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Use RV Ticket - " + tag, true, true);

            StopIsCoruition();
            rvDelay = true;

            this.TaskDelay(5, () => rvDelay = false);

            if (MondayOFF.AdsManager.GetTimeUntilNextInterstitial() < 30)
                MondayOFF.AdsManager.SetIsTime(Time.realtimeSinceStartup - 30);
        }
        else
        {
            onWatchedAds?.Invoke();
            MondayOFF.AdsManager.ShowRewarded(() =>
            {
                onAfterReward.Invoke();
                EventManager.instance.CustomEvent(AnalyticsType.RV, tag, true, true);
            });
        }
    }
}
