using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class sideIapBtn : MonoBehaviour
{
    public string popupPath;
    public string iapId;

    [Space]

    public Image icon;
    public Button btn;

    public void Init(Sprite icon, string path, string iapId)
    {
        if (icon != null)
            this.icon.sprite = icon;
        this.popupPath = path;
        this.iapId = iapId;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            IapManager.instance.GenerateIapPopup(path.Replace("(Clone)", ""), iapId, () =>
            {
                Hide();

                // Debug.LogError("Iap Purchase Success On Side IAP Btn - " + iapId);
                EventManager.instance.CustomEvent(AnalyticsType.UI, "Iap Purchase Success On Side IAP Btn - " + iapId, true, true);
            }, true, imageMotion: false);
        });

        IapManager.instance.onSuccessPurchase = null;

        IapManager.instance.onSuccessPurchase += (id) =>
        {
            if (id == iapId)
                Hide();
        };
    }

    public void OnPurchaseIap(string iapId)
    {
        if (this.iapId == iapId)
        {
            Hide();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
