using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class NewStickUnlockedPopUp : MonoBehaviour
{
    [SerializeField] private Text levelText;
    [SerializeField] private Image stickImage;
    [SerializeField] private Button rvBtn;
    [SerializeField] private Button noThanksBtn;

    System.Action onHideAction;

    public int stickLevel = 2;

    public void Init(int level, System.Action _onHideAction = null)
    {
        stickLevel = level;
        levelText.text = "LV. " + level;

        switch (MainManager.instance.pointerSkinType)
        {
            case PointerSkinType.Default:
                stickImage.sprite = Resources.LoadAll<StickObjects>("Pointer").Where((n) => n.level == level).First().icon;
                break;

            case PointerSkinType.Antique:
                stickImage.sprite = Resources.LoadAll<StickObjects>("Pointer").Where((n) => n.level == level).First().antique_icon;
                break;
        }

        this.TaskDelay(1.2f, () => { noThanksBtn.gameObject.SetActive(true); rvBtn.enabled = true; noThanksBtn.enabled = true; });

        onHideAction = null;
        onHideAction = _onHideAction;
    }

    public void OnClickGain()
    {
        AdManager.instance.RV(() =>
        {
            MainManager.instance.TaskDelay(1f, () =>
                {
                    MainManager.instance.SpawnPointer(StageManager.instance.currentClock.clockSpawnRotates[MainManager.instance.FindUpgrade(UpgradeType.AddStick).currentLevel % StageManager.instance.currentClock.clockSpawnRotates.Length], level: stickLevel);
                });
            // EventManager.instance.CustomEvent(AnalyticsType.RV, "Extra Stick _ " + stickLevel, true, true);
        }, "Extra Stick _ " + stickLevel);

        Hide();
    }

    public void OnClickNoThanks()
    {
        Hide();
    }

    public void Hide()
    {
        if (stickLevel == 3 && !ES3.KeyExists("IAP_ShowiapOffer_StarterPack"))
        {
            MainManager.instance.TaskDelay(5f, () =>
            {
                IapManager.instance.GenerateIapPopup("UI/iapOffer_StarterPack", IapManager.iap_starterPack);
                ES3.Save<bool>("IAP_ShowiapOffer_StarterPack", true);
            });
        }
        else if (stickLevel == 4 && !IapManager.instance.lv2StickForever)
        {
            MainManager.instance.TaskDelay(2f, () =>
            {
                IapManager.instance.GenerateIapPopup("UI/iapOffer_lv2StickForever", IapManager.iap_lv2StickForever);
            });
        }

        Destroy(gameObject);

        MainManager.instance.isReadyOnRV = false;

        onHideAction?.Invoke();
    }
}
