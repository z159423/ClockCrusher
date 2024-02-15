using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class IapManager : MonoBehaviour
{

    public const string iap_starterPack = "clockcrusher_starterpack";
    public const string iap_noAds = "clockcrusher_noads";
    public const string iap_antiqueSkinPack = "clockcrusher_antiqueskinpack";
    public const string iap_moneyPack = "clockcrusher_moneypack";

    public const string iap_freePause = "clockcrusher_freepause";

    public const string iap_lv2StickForever = "clockcrusher_lv2stickforever";

    public const string iap_rvTicket10 = "clockcrusher_rvticket10";
    public const string iap_rvTicket20 = "clockcrusher_rvticket20_";
    public const string iap_rvTicket50 = "clockcrusher_rvticket50";


    public static IapManager instance;
    public GameObject noAdsBtn;
    public GameObject defaultLV2StickIapSideBtn;
    public GameObject iapSideBtn;


    [FoldoutGroup("참조")] public GameObject iapLoadingScreen;


    public bool lv2StickForever = false;
    public bool starterPack = false;
    public bool antiqueSkinPack = false;
    public bool freePause = false;
    public bool moneyPack = false;

    private IapPopUp currentPopup = null;

    public System.Action<string> onSuccessPurchase = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        lv2StickForever = ES3.KeyExists("lv2stickforever") ? ES3.Load<bool>("lv2stickforever") : false;
        freePause = ES3.KeyExists("iap_freePause") ? ES3.Load<bool>("iap_freePause") : false;
        starterPack = ES3.KeyExists("iap_starterPack") ? ES3.Load<bool>("iap_starterPack") : false;
        antiqueSkinPack = ES3.KeyExists("iap_antiqueSkinPack") ? ES3.Load<bool>("iap_antiqueSkinPack") : false;

        if (antiqueSkinPack)
        {
            MainManager.instance.pointerSkinType = PointerSkinType.Antique;
            MainManager.instance.pointerList.ForEach((n) => n.ChangeSkin());

            MainManager.instance.stickCollectionBtn.UpdateIcon();
        }

        if (starterPack)
        {
            MondayOFF.AdsManager.HideBanner();
            MondayOFF.AdsManager.DisableInterstitial();
        }

        MainManager.instance.pauseBtn.UpdateUI();

        if (noAdsBtn != null)
            MondayOFF.NoAds.OnNoAds += () => noAdsBtn.SetActive(false);

        MondayOFF.AdsManager.OnAfterInterstitial += () =>
        {
            ES3.Save<bool>("EnableNoAdsBtn", true);
            EnableNoAdsBtn();
        };

        MondayOFF.NoAds.OnNoAds += () =>
        {
            EventManager.instance.CustomEvent(AnalyticsType.IAP, "Noads Active", true, true);

            SaveManager.instance.AddMoney(500);
            SaveManager.instance.AddRvTicket(10);

            ES3.Save<bool>("enableShop", true);

            if (MainManager.instance.shopUI != null)
            {
                MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.moneyIcon, MainManager.instance.shopUI.NoadsPack.transform);
                MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.RvTicketIcon, MainManager.instance.shopUI.NoadsPack.transform, new ParticleSystem.Burst(0, 20), Resources.Load<Material>("UIParticle_RVTicket"), MainManager.instance.shopUI.transform);
                MainManager.instance.shopUI.UpdateUI();
            }
        };



        MondayOFF.IAPManager.RegisterProduct(iap_starterPack, () =>
    {
        // ES3.Save<bool>("NoAds", true);

        starterPack = true;
        SaveManager.instance.AddRvTicket(20);

        EventManager.instance.CustomEvent(AnalyticsType.IAP, " Reward IAP - " + iap_starterPack, true, true);

        ES3.Save<bool>("iap_starterPack", true);
        ES3.Save<bool>("enableShop", true);

        MondayOFF.AdsManager.HideBanner();
        MondayOFF.AdsManager.DisableInterstitial();

        if (MainManager.instance.shopUI != null)
        {
            MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.moneyIcon, MainManager.instance.shopUI.starterPack.transform);
            MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.RvTicketIcon, MainManager.instance.shopUI.starterPack.transform, new ParticleSystem.Burst(0, 20), Resources.Load<Material>("UIParticle_RVTicket"), MainManager.instance.shopUI.transform);
            MainManager.instance.shopUI.UpdateUI();
        }

        if (currentPopup != null)
        {
            Destroy(currentPopup.gameObject);
        }

        iapSideBtn.SetActive(false);
    });

        MondayOFF.IAPManager.RegisterProduct(iap_antiqueSkinPack, () =>
        {
            // ES3.Save<bool>("NoAds", true);

            ES3.Save<bool>("enableShop", true);

            antiqueSkinPack = true;
            MainManager.instance.pointerSkinType = PointerSkinType.Antique;

            SaveManager.instance.AddMoney(500);
            SaveManager.instance.AddRvTicket(20);
            EventManager.instance.CustomEvent(AnalyticsType.IAP, " Reward IAP - " + iap_antiqueSkinPack, true, true);
            ES3.Save<bool>("iap_antiqueSkinPack", true);

            MainManager.instance.pointerList.ForEach((n) => n.ChangeSkin());

            if (MainManager.instance.shopUI != null)
            {
                MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.moneyIcon, MainManager.instance.shopUI.AntiqueSkinPack.transform);
                MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.RvTicketIcon, MainManager.instance.shopUI.AntiqueSkinPack.transform, new ParticleSystem.Burst(0, 20), Resources.Load<Material>("UIParticle_RVTicket"), MainManager.instance.shopUI.transform);
                MainManager.instance.shopUI.UpdateUI();
            }

            MainManager.instance.stickCollectionBtn.UpdateIcon();

        });

        MondayOFF.IAPManager.RegisterProduct(iap_moneyPack, () =>
        {
            // ES3.Save<bool>("NoAds", true);

            ES3.Save<bool>("enableShop", true);

            moneyPack = true;

            SaveManager.instance.AddMoney(500);
            SaveManager.instance.AddRvTicket(20);
            EventManager.instance.CustomEvent(AnalyticsType.IAP, " Reward IAP - " + iap_moneyPack, true, true);
            ES3.Save<bool>("iap_moneypack", true);

            if (MainManager.instance.shopUI != null)
            {
                MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.moneyIcon, MainManager.instance.shopUI.AntiqueSkinPack.transform);
                MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.RvTicketIcon, MainManager.instance.shopUI.AntiqueSkinPack.transform, new ParticleSystem.Burst(0, 20), Resources.Load<Material>("UIParticle_RVTicket"), MainManager.instance.shopUI.transform);
                MainManager.instance.shopUI.UpdateUI();
            }
        });

        MondayOFF.IAPManager.RegisterProduct(iap_lv2StickForever, () =>
        {
            lv2StickForever = true;

            EventManager.instance.CustomEvent(AnalyticsType.IAP, " Reward IAP - " + iap_lv2StickForever, true, true);
            ES3.Save<bool>("lv2stickforever", true);
            defaultLV2StickIapSideBtn.SetActive(false);

            if (MainManager.instance.shopUI != null)
                MainManager.instance.shopUI.UpdateUI();
        });

        MondayOFF.IAPManager.RegisterProduct(iap_freePause, () =>
                {
                    freePause = true;

                    EventManager.instance.CustomEvent(AnalyticsType.IAP, " Reward IAP - " + iap_freePause, true, true);
                    ES3.Save<bool>("iap_freePause", true);

                    if (MainManager.instance.shopUI != null)
                        MainManager.instance.shopUI.UpdateUI();

                    MainManager.instance.pauseBtn.UpdateUI();
                });

        MondayOFF.IAPManager.RegisterProduct(iap_rvTicket10, () =>
       {
           EventManager.instance.CustomEvent(AnalyticsType.IAP, " Reward IAP - " + iap_rvTicket10, true, true);

           SaveManager.instance.AddRvTicket(10);

           if (MainManager.instance.shopUI != null)
           {
               MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.RvTicketIcon, MainManager.instance.shopUI.rvTicket10Btn.transform, new ParticleSystem.Burst(0, 10), Resources.Load<Material>("UIParticle_RVTicket"), MainManager.instance.shopUI.transform);
               MainManager.instance.shopUI.UpdateUI();
           }
       });

        MondayOFF.IAPManager.RegisterProduct(iap_rvTicket20, () =>
               {
                   EventManager.instance.CustomEvent(AnalyticsType.IAP, " Reward IAP - " + iap_rvTicket20, true, true);

                   SaveManager.instance.AddRvTicket(20);


                   if (MainManager.instance.shopUI != null)
                   {
                       MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.RvTicketIcon, MainManager.instance.shopUI.rvTicket20Btn.transform, new ParticleSystem.Burst(0, 20), Resources.Load<Material>("UIParticle_RVTicket"), MainManager.instance.shopUI.transform);
                       MainManager.instance.shopUI.UpdateUI();
                   }
               });

        MondayOFF.IAPManager.RegisterProduct(iap_rvTicket50, () =>
        {
            EventManager.instance.CustomEvent(AnalyticsType.IAP, " Reward IAP - " + iap_rvTicket50, true, true);

            SaveManager.instance.AddRvTicket(50);


            if (MainManager.instance.shopUI != null)
            {
                MainManager.instance.GenerateUIParticleAttractor(MainManager.instance.shopUI.RvTicketIcon, MainManager.instance.shopUI.rvTicket50Btn.transform, new ParticleSystem.Burst(0, 25), Resources.Load<Material>("UIParticle_RVTicket"), MainManager.instance.shopUI.transform);
                MainManager.instance.shopUI.UpdateUI();
            }
        });

        // if (ES3.KeyExists("IAP_ShowLv2StickForeverPopup") && (ES3.KeyExists("lv2stickforever") ? !ES3.Load<bool>("lv2stickforever") : true))
        // {
        //     defaultLV2StickIapSideBtn.SetActive(true);
        //     this.TaskDelay(10f, () =>
        //     {
        //         GenerateIapPopup("UI/iapOffer_lv2StickForever");
        //     });
        // }
        // else
        //     defaultLV2StickIapSideBtn.SetActive(false);

        if (ES3.KeyExists("IAP_ShowiapOffer_StarterPack") && (ES3.KeyExists("iap_starterPack") ? !ES3.Load<bool>("iap_starterPack") : true))
        {
            iapSideBtn.SetActive(true);
            iapSideBtn.GetComponent<sideIapBtn>().Init(null, "UI/iapOffer_StarterPack", iap_starterPack);
            // this.TaskDelay(10f, () =>
            // {
            //     GenerateIapPopup("UI/iapOffer_StarterPack", iap_starterPack);
            // });
        }
        else
            iapSideBtn.SetActive(false);

        if (ES3.KeyExists("iap_starterPack") ? ES3.Load<bool>("iap_starterPack") : false)
        {
            MondayOFF.AdsManager.HideBanner();
            MondayOFF.AdsManager.DisableInterstitial();

            noAdsBtn.SetActive(false);
        }

        MondayOFF.IAPManager.OnBeforePurchase += () =>
        {
            if (iapLoadingScreen != null)
                Destroy(iapLoadingScreen);
            iapLoadingScreen = Instantiate(Resources.Load<GameObject>("UI/Loading"), null);
        };

        MondayOFF.IAPManager.OnAfterPurchase += (result) =>
        {
            if (iapLoadingScreen != null)
                Destroy(iapLoadingScreen);
        };

        MondayOFF.IAPManager.OnAfterPurchaseWithProductId += (result, id) =>
        {
            onSuccessPurchase?.Invoke(id);
        };

    }

    public void OnClickNoAdsBtn()
    {
        var pop = GenerateIapPopup("UI/iapOffer_noAdsPack", IapManager.iap_noAds, onCompletePurcahse: () => noAdsBtn.SetActive(false), showNoThanks: true);
        pop.GetComponentInChildren<IapPopUp>().ChangeTitle("Sales Offer");
        // MondayOFF.NoAds.Purchase();
    }

    public void EnableNoAdsBtn()
    {
        if (ES3.KeyExists("EnableNoAdsBtn") && !MondayOFF.NoAds.IsNoAds)
        {
            if (noAdsBtn != null)
                noAdsBtn.SetActive(true);
        }
        else
        {
            if (noAdsBtn != null)
                noAdsBtn.SetActive(false);
        }
    }

    public void PurchaseIap(string id, System.Action onSuccessPurchase = null)
    {
        var status = MondayOFF.IAPManager.PurchaseProduct(id);

        if (status == MondayOFF.IAPStatus.Success)
        {
            EventManager.instance.CustomEvent(AnalyticsType.IAP, "try Purchase IAP - " + id, true, true);

            onSuccessPurchase?.Invoke();
        }
    }

    public void OnClickSideIapBtn(string id)
    {
        switch (id)
        {
            case iap_starterPack:
                Generate("UI/iapOffer_StarterPack", id);
                break;

            case iap_lv2StickForever:
                Generate("UI/iapOffer_lv2StickForever", id);
                break;

            case iap_rvTicket20:
                Generate("UI/iapOffer_rvTicket20", id);
                break;
        }

        void Generate(string path, string id)
        {
            var popup = Instantiate(Resources.Load<GameObject>(path), MainManager.instance.UI);
            popup.GetComponent<IapPopUp>().Init(id);
            currentPopup = popup.GetComponent<IapPopUp>();
        }
    }

    public void NoAds()
    {
        MondayOFF.AdsManager.HideBanner();
        MondayOFF.AdsManager.DisableInterstitial();
    }

    public GameObject GenerateIapPopup(string path, string id, System.Action onCompletePurcahse = null, bool showNoThanks = false, bool imageMotion = true)
    {
        if (path == "UI/iapOffer_StarterPack")
        {
            if (starterPack)
                return null;
        }

        // if (ES3.KeyExists("lv2stickforever") ? !ES3.Load<bool>("lv2stickforever") : true)
        // {

        var popup = Instantiate(Resources.Load<GameObject>(path), MainManager.instance.UI);

        popup.GetComponent<IapPopUp>().Init(id, onCompletePurcahse, showNoThanks, imageMotion: imageMotion);

        currentPopup = popup.GetComponent<IapPopUp>();

        return popup;
        // }
    }
}
