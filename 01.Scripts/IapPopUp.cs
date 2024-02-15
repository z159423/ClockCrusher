using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IapPopUp : MonoBehaviour
{
    public System.Action onClickEvent;
    public System.Action afterPurchaseEvent;

    public Text title;
    public void ChangeTitle(string name) => title.text = name;

    public Button purchaseBtn;
    public Button noThanksBtn;

    public Image icon;

    public string id;

    private System.Action onCompletePurchase = null;

    private bool imageMotion = true;

    public void Init(string id, System.Action onCompletePurchase = null, bool showNoThanks = false, bool imageMotion = true)
    {
        this.onCompletePurchase = onCompletePurchase;

        EventManager.instance.CustomEvent(AnalyticsType.UI, "IAP POPUP - " + id, true, true);

        this.id = id;
        this.TaskDelay(1f, () => { purchaseBtn.enabled = true; });
        this.TaskDelay(1.2f, () => noThanksBtn.gameObject.SetActive(true));

        this.imageMotion = imageMotion;

        // purchaseBtn.onClick.AddListener(() => IapManager.instance.PurchaseIap(IapManager.iap_lv2StickForever));

        // MondayOFF.IAPManager.RegisterProduct(id, () =>
        //         {
        //             Hide();
        //         });

        if (showNoThanks)
            noThanksBtn.gameObject.SetActive(true);
    }

    public void OnClickPurchase(string id)
    {
        onClickEvent?.Invoke();

        IapManager.instance.PurchaseIap(id, () =>
        {
            onCompletePurchase?.Invoke();
            Hide();
        });

        EventManager.instance.CustomEvent(AnalyticsType.UI, " OnClick IAP PopUp - " + id, true, true);

        // Hide();
    }

    public void Hide()
    {
        if (id == IapManager.iap_starterPack && !IapManager.instance.starterPack && !ES3.KeyExists("enableShop"))
        {
            ES3.Save<bool>("enableShop", false);
            MainManager.instance.TaskDelay(5, () =>
            {
                Tutorial.insatnce.StartRVTicketTutorial();
            });
        }
        else if (id == IapManager.iap_lv2StickForever)
        {
            if (!ES3.KeyExists("MyStickTutorial"))
                MainManager.instance.TaskDelay(5f, () => Tutorial.insatnce.StartMyStickTutorial());
        }

        if (this != null)
            Destroy(gameObject);
    }

    public void OnClickNoThanks()
    {
        IapManager.instance.iapSideBtn.SetActive(true);

        if (id != IapManager.iap_noAds)
        {
            IapManager.instance.iapSideBtn.GetComponent<sideIapBtn>().Init(icon.sprite, "UI/" + gameObject.name, id);
            if (imageMotion)
                MainManager.instance.GenerateImageMotion(icon, icon.transform, IapManager.instance.iapSideBtn.transform);
        }

        Hide();
        // IapManager.instance.iapSideBtn.
    }
}
