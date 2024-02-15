using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using MoreMountains.NiceVibrations;

public class StickCollectionSlot : MonoBehaviour
{
    public int level;

    [SerializeField] Image stickIcon;
    [SerializeField] Text rvWatchedCount;

    [SerializeField] Image halo;
    [SerializeField] Image check;
    [SerializeField] DOTweenAnimation iconAnimation;
    [SerializeField] DOTweenAnimation haloAnimation;
    [SerializeField] public GameObject remainText;
    [SerializeField] public GameObject freeText;
    [SerializeField] public GameObject rvTicketImage;




    bool pause = false;

    public void Init(int level)
    {
        this.level = level;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (rvWatchedCount != null)
        {
            var find = Resources.LoadAll<StickObjects>("Pointer").Where((n) => n.level == level).First();
            rvWatchedCount.text = (ES3.KeyExists("PointerCollectionRvWatchedCount_" + level) ? ES3.Load<int>("PointerCollectionRvWatchedCount_" + level) : 0).ToString() + "/" + find.rvWatchGoalCount;
        }

        switch (MainManager.instance.pointerSkinType)
        {
            case PointerSkinType.Default:
                stickIcon.sprite = Resources.LoadAll<StickObjects>("Pointer").Where((n) => n.level == level).First().icon;
                break;

            case PointerSkinType.Antique:
                stickIcon.sprite = Resources.LoadAll<StickObjects>("Pointer").Where((n) => n.level == level).First().antique_icon;

                break;
        }

        // stickIcon.GetComponent<RectTransform>().sizeDelta = stickIcon.sprite.textureRect.size;
    }

    public void OnClickRVGetBtn()
    {
        if (pause)
            return;

        if (Tutorial.insatnce.myStickTutorial)
        {
            Tutorial.insatnce.EndMyStickTutorial();
            
            UpdateUI();
            StickGainEffect();

            if (this != null)
                GetComponentInParent<StickCollectionUI>().Hide();

            MMVibrationManager.Haptic(HapticTypes.HeavyImpact);

            MainManager.instance.SpawnPointer(level);

        }
        else
        {
            AdManager.instance.RV(() =>
                {
                    var count = (ES3.KeyExists("PointerCollectionRvWatchedCount_" + level) ? ES3.Load<int>("PointerCollectionRvWatchedCount_" + level) : 0);
                    ES3.Save<int>("PointerCollectionRvWatchedCount_" + level, count + 1);

                    // EventManager.instance.CustomEvent(AnalyticsType.RV, "Stick Collection - Try Get Stick", true, true);
                    EventManager.instance.CustomEvent(AnalyticsType.GAME, "Stick Collection - Try Get Stick - " + level, true, true);

                    //스틱 보상 주기
                    if (count + 1 >= Resources.LoadAll<StickObjects>("Pointer").Where((n) => n.level == level).First().rvWatchGoalCount)
                    {
                        UpdateUI();
                        ES3.Save<int>("PointerCollectionRvWatchedCount_" + level, 0);
                        pause = true;
                        StickGainEffect();
                        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Stick Collection - Success Get Stick", true, true);
                        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Stick Collection - Success Get Stick - " + level, true, true);

                        if (this != null)
                            GetComponentInParent<StickCollectionUI>().Hide();

                        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);

                        MainManager.instance.SpawnPointer(level);
                    }
                    else
                        UpdateUI();
                }, "Stick Collection - Try Get Stick");
        }
    }

    public void StickGainEffect()
    {
        iconAnimation.DOPlay();
        haloAnimation.DOPlay();
        check.gameObject.SetActive(true);
        this.TaskDelay(2f, () =>
        {
            pause = false;
            if (check != null)
            {
                check.gameObject.SetActive(false);
                UpdateUI();
                iconAnimation.DOPause();
                haloAnimation.DOPause();
            }
        });
    }
}
