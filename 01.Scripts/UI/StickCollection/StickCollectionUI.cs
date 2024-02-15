using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StickCollectionUI : MonoBehaviour
{
    [SerializeField] Transform layoutParent;

    [SerializeField] Image skinChange_Icon;
    [SerializeField] Text skinChange_Text;
    [SerializeField] Button skinChangeBtn;


    public void Show()
    {
        UpdateUI();

        ES3.Save<bool>("NewStickInCollection", false);
        MainManager.instance.stickCollectionBtn.UpdateDot();
        UpdateChangeSkinUI();

        if (Tutorial.insatnce.myStickTutorial)
        {
            if (Tutorial.insatnce.currentTutorial != null)
                Destroy(Tutorial.insatnce.currentTutorial);

            var find = GetComponentsInChildren<StickCollectionSlot>().Where((n) => n.level == 3).First();
            this.TaskDelay(0.01f, () =>
            {
                GetComponentInChildren<ScrollRect>().enabled = false;
                Tutorial.insatnce.currentTutorial = HightlightFocus.GenerateMask_FullMask(MainManager.instance.UI, find.transform.position, 1.2f);

                find.freeText.SetActive(true);
                find.remainText.SetActive(false);
                find.rvTicketImage.SetActive(false);

            });

            // Tutorial.insatnce.currentTutorial = HightlightFocus.GenerateMask(find.transform, find.transform.position, find.transform, 1.5f);
        }
    }

    public void Hide()
    {
        EventManager.instance.CustomEvent(AnalyticsType.UI, " Hide Stick Collection UI", true, true);

        Destroy(gameObject);
    }

    public void UpdateUI()
    {
        var find = Resources.LoadAll<StickObjects>("Pointer").Where((n) => n.level > 1);
        var unlocked = ES3.KeyExists("UnlockedStick") ? ES3.Load<List<int>>("UnlockedStick") : new List<int>();

        find.ToList().ForEach((n) =>
        {
            if (unlocked.Contains(n.level))
            {
                var load = Instantiate(Resources.Load<GameObject>("UI/UnlockedSlot"), layoutParent).GetComponent<StickCollectionSlot>();
                load.Init(n.level);
                load.UpdateUI();
            }
            else
            {
                var load = Instantiate(Resources.Load<GameObject>("UI/LockedSlot"), layoutParent).GetComponent<StickCollectionSlot>();
                load.Init(n.level);
                load.UpdateUI();
            }
        });

        skinChangeBtn.gameObject.SetActive(IapManager.instance.antiqueSkinPack);
    }

    public void OnClickGetStickRV()
    {

    }

    public void OnClickChangeSkin()
    {
        if (ES3.KeyExists("iap_antiqueSkinPack") ? ES3.Load<bool>("iap_antiqueSkinPack") : false)
        {
            MainManager.instance.pointerSkinType = MainManager.instance.pointerSkinType == PointerSkinType.Default ? PointerSkinType.Antique : PointerSkinType.Default;

            ES3.Save<PointerSkinType>("PointerSkinType", MainManager.instance.pointerSkinType);

            UpdateChangeSkinUI();
            EventManager.instance.CustomEvent(AnalyticsType.UI, "OnClickChangeSkinBtn", true, true);

            foreach (var slot in GetComponentsInChildren<StickCollectionSlot>())
            {
                slot.UpdateUI();
            }

            MainManager.instance.pointerList.ForEach((n) => n.ChangeSkin());

            MainManager.instance.stickCollectionBtn.UpdateIcon();
        }
        else
        {
            MainManager.instance.ShowShopUI(1);

            EventManager.instance.CustomEvent(AnalyticsType.UI, "OnClickChangeSkinBtn - goToStore", true, true);
        }
    }

    public void UpdateChangeSkinUI()
    {
        switch (MainManager.instance.pointerSkinType)
        {
            case PointerSkinType.Default:
                skinChange_Icon.sprite = Resources.Load<Sprite>("icon_scRV");
                skinChange_Text.text = "Antique Sticks";
                break;

            case PointerSkinType.Antique:
                skinChange_Icon.sprite = Resources.Load<Sprite>("icon_MySticks_B");
                skinChange_Text.text = "Default Skin";
                break;
        }
    }
}
