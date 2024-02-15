using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public class ShopUI : MonoBehaviour
{
    [TitleGroup("Package")][SerializeField] public GameObject packageSet;
    [TitleGroup("Package")][SerializeField] public GameObject packageTitle;
    [TitleGroup("Package")][SerializeField] public GameObject starterPack;
    [TitleGroup("Package")][SerializeField] public GameObject AntiqueSkinPack;
    [TitleGroup("Package")][SerializeField] public GameObject NoadsPack;
    [TitleGroup("Package")][SerializeField] public GameObject moneyPack;


    [TitleGroup("Upgrade")][SerializeField] public GameObject upgradeSet;
    [TitleGroup("Upgrade")][SerializeField] public GameObject upgradeTitle;
    [TitleGroup("Upgrade")][SerializeField] private GameObject lv2StickForever;
    [TitleGroup("Upgrade")][SerializeField] private GameObject infinityPause;


    [TitleGroup("rvTicket")][SerializeField] public GameObject ticketsSet;
    [TitleGroup("rvTicket")][SerializeField] public GameObject ticketsTitle;
    [TitleGroup("rvTicket")][SerializeField] private GameObject rvTicketFreeBtn;
    [TitleGroup("rvTicket")][SerializeField] public GameObject rvTicket10Btn;
    [TitleGroup("rvTicket")][SerializeField] public GameObject rvTicket20Btn;
    [TitleGroup("rvTicket")][SerializeField] public GameObject rvTicket50Btn;



    [TitleGroup("dailyRvTicket")][SerializeField] private GameObject dailyRVTicketAvaliableParent;
    [TitleGroup("dailyRvTicket")][SerializeField] private Text dailyRVTicketAvaliablCountText;
    [TitleGroup("dailyRvTicket")][SerializeField] private GameObject dailyRVTicketRemainTimeParent;
    [TitleGroup("dailyRvTicket")][SerializeField] private Text dailyRVTicketRemainTimeText;

    [TitleGroup("particles")][SerializeField] private Transform rvTicketEndParent;



    const int dailyRVTicketFreeMaxCount = 5;


    public Transform moneyIcon;
    public Transform RvTicketIcon;

    private GameObject focus;

    private TaskUtil.WhileTaskMethod dailyRVTask = null;

    private bool useable = true;

    public static bool shopUIActive = false;

    private void Start()
    {
        MainManager.instance.shopUI = this;

        dailyRVTask = this.TaskWhile(1f, 0, () => UpdateDailyRVUI());

        UpdateUI();

        if (Tutorial.insatnce.rvTicketTutorial)
        {
            rvTicketFreeBtn.SetActive(true);
            if(Tutorial.insatnce.currentTutorial != null)
            Tutorial.insatnce.currentTutorial?.SetActive(false);
            focus = HightlightFocus.GenerateMask(rvTicketFreeBtn.transform, rvTicketFreeBtn.transform.position, rvTicketFreeBtn.transform, 1);
            this.TaskDelay(0.1f, () => MoveScrollViewToTargetPosition(0));
            GetComponentInChildren<ScrollRect>().vertical = false;
        }
        shopUIActive = true;
    }



    private void OnDestroy()
    {
        if (dailyRVTask != null)
            dailyRVTask.Kill();
        MainManager.instance.shopUI = null;

        shopUIActive = false;
    }

    public void UpdateUI()
    {
        if (ES3.KeyExists("iap_starterPack") ? ES3.Load<bool>("iap_starterPack") : false)
        {
            NoadsPack.SetActive(false);
            starterPack.SetActive(false);
        }

        if (MondayOFF.NoAds.IsNoAds)
        {
            NoadsPack.SetActive(false);
        }

        if (ES3.KeyExists("iap_antiqueSkinPack") ? ES3.Load<bool>("iap_antiqueSkinPack") : false)
        {
            AntiqueSkinPack.SetActive(false);
        }

        if (ES3.KeyExists("iap_moneypack") ? ES3.Load<bool>("iap_moneypack") : false)
        {
            moneyPack.SetActive(false);
        }

        if (ES3.KeyExists("lv2stickforever") ? ES3.Load<bool>("lv2stickforever") : false)
        {
            lv2StickForever.SetActive(false);
        }

        if (ES3.KeyExists("iap_freePause") ? ES3.Load<bool>("iap_freePause") : false)
        {
            infinityPause.SetActive(false);
        }

        packageTitle.SetActive(CountActiveChildren(packageSet.transform) > 0);
        upgradeTitle.SetActive(CountActiveChildren(upgradeSet.transform) > 0);
        ticketsTitle.SetActive(CountActiveChildren(ticketsSet.transform) > 0);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInChildren<UnityEngine.UI.ScrollRect>().content);
    }

    public static int CountActiveChildren(Transform parent)
    {
        int activeChildCount = 0;

        // 모든 자식에 대한 반복
        foreach (Transform child in parent)
        {
            // 자식이 활성화되어 있는지 확인
            if (child.gameObject.activeSelf)
            {
                activeChildCount++;
            }
        }

        return activeChildCount;
    }

    public void OnClickPurchaseBtn(string id)
    {
        EventManager.instance.CustomEvent(AnalyticsType.UI, "On Click Iap Purchase Btn - " + id, true, true);
        MondayOFF.IAPManager.PurchaseProduct(id);
    }

    public void TryPurchaseNoAds()
    {
        MondayOFF.NoAds.Purchase();
    }

    public void OnClickRVTicketFreeBtn()
    {
        if (!useable)
            return;

        SaveManager.instance.AddRvTicket(1);


        Tutorial.insatnce.TaskDelay(1.65f, () => Hide());

        if (Tutorial.insatnce.rvTicketTutorial)
        {
            if (focus != null)
                Destroy(focus);

            useable = false;

            Tutorial.insatnce.EndRVTicketTutorial();
        }

        MainManager.instance.rvTicketText.SetActive(true);

        this.TaskDelay(0.1f, () =>
                                        {
                                            MainManager.instance.GenerateUIParticleAttractor(rvTicketEndParent, dailyRVTicketAvaliableParent.transform, new ParticleSystem.Burst(0, 1), Resources.Load<Material>("UIParticle_RVTicket"), transform);

                                        });

    }

    public void Hide()
    {
        if (this != null)
            Destroy(gameObject);
    }

    public float targetNormalizedPosition;

    // 예를 들어 버튼이나 다른 이벤트에서 이 메서드를 호출하여 스크롤 뷰를 이동시킬 수 있습니다.
    [Button("MoveScrollViewToTargetPosition")]
    public void MoveScrollViewToTargetPosition(float value)
    {
        if (GetComponentInChildren<UnityEngine.UI.ScrollRect>() != null)
        {
            // 유효한 normalizedPosition 값은 0에서 1 사이입니다.
            value = Mathf.Clamp01(value);

            // 스크롤 뷰의 normalizedPosition을 설정하여 이동합니다.
            GetComponentInChildren<UnityEngine.UI.ScrollRect>().normalizedPosition = new Vector2(GetComponentInChildren<UnityEngine.UI.ScrollRect>().normalizedPosition.x, value);

        }
    }

    public void OnClickDailyRVTicketBtn()
    {
        if (SaveManager.instance.IsTimeLimitRVReady(SaveManager.instance.dailyFreeRvTicketTime))
        {
            EventManager.instance.CustomEvent(AnalyticsType.UI, "On Click Free RV Ticket", true, true);

            MondayOFF.AdsManager.ShowRewarded(() =>
            {
                ES3.Save<int>("DailyRvTicketCount", (ES3.KeyExists("DailyRvTicketCount") ? ES3.Load<int>("DailyRvTicketCount") : dailyRVTicketFreeMaxCount) - 1);

                if ((ES3.KeyExists("DailyRvTicketCount") ? ES3.Load<int>("DailyRvTicketCount") : dailyRVTicketFreeMaxCount) <= 0)
                {
                    SaveManager.instance.dailyFreeRvTicketTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    ES3.Save<string>("dailyRvTicketTime", SaveManager.instance.dailyFreeRvTicketTime);
                }

                SaveManager.instance.AddRvTicket(1);
                UpdateDailyRVUI();
                EventManager.instance.CustomEvent(AnalyticsType.RV, "Rv Ticket 1", true, true);

                this.TaskDelay(0.1f, () =>
                                        {
                                            MainManager.instance.GenerateUIParticleAttractor(rvTicketEndParent, dailyRVTicketAvaliableParent.transform, new ParticleSystem.Burst(0, 1), Resources.Load<Material>("UIParticle_RVTicket"), transform);

                                        });
            });
        }
    }

    public void UpdateDailyRVUI()
    {
        if (SaveManager.instance.IsTimeLimitRVReady(SaveManager.instance.dailyFreeRvTicketTime))
        {
            if ((ES3.KeyExists("DailyRvTicketCount") ? ES3.Load<int>("DailyRvTicketCount") : dailyRVTicketFreeMaxCount) == 0)
                ES3.Save<int>("DailyRvTicketCount", dailyRVTicketFreeMaxCount);

            dailyRVTicketAvaliableParent.SetActive(true);
            dailyRVTicketRemainTimeParent.SetActive(false);
        }
        else
        {
            dailyRVTicketAvaliableParent.SetActive(false);
            dailyRVTicketRemainTimeParent.SetActive(true);

            var lefttime = (int)SaveManager.instance.GetLeftTime(SaveManager.instance.dailyFreeRvTicketTime);

            dailyRVTicketRemainTimeText.text = SaveManager.GetFormatedStringFromSecond(lefttime);
        }

        dailyRVTicketAvaliablCountText.text = (ES3.KeyExists("DailyRvTicketCount") ? ES3.Load<int>("DailyRvTicketCount") : dailyRVTicketFreeMaxCount) + "/" + dailyRVTicketFreeMaxCount;
    }

}
