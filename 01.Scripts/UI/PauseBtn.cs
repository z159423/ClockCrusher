using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseBtn : MonoBehaviour
{
    public Image btnImage;
    public GameObject puaseText;
    public GameObject moveTheStickText;
    public Text btnText;

    [Space]
    public Sprite playImage;
    public Sprite puaseImage;

    [Space]
    public GameObject rvBtn;

    private void Start()
    {
        gameObject.SetActive(ES3.KeyExists("PauseTutorial"));
    }

    public void UpdateUI()
    {
        rvBtn.SetActive(!IapManager.instance.freePause);
    }

    public void OnClickBtn()
    {
        if (MainManager.instance.tutorial1 || Tutorial.insatnce.dragPhase)
            return;

        if (MainManager.instance.pause)
            return;

        if (Tutorial.insatnce.pauseTutorial)
        {
            ShowHide();
            Tutorial.insatnce.StartDragPhase();
        }
        else
        {
            EventManager.instance.CustomEvent(AnalyticsType.UI, " OnClickPauseBtn - Pause", true, true);

            if (IapManager.instance.freePause)
            {
                StartPuase();
                EventManager.instance.CustomEvent(AnalyticsType.GAME, "Pause Start [Free Pause Purchase User]", true, true);
            }
            else
            {
                AdManager.instance.RV(() =>
        {
            StartPuase();
        }, "Pause", onWatchedAds: () => MainManager.instance.rvPause = true);

            }
        }
    }

    public void StartPuase()
    {
        MainManager.instance.pause = true;

        MainManager.instance.pointerList.ForEach((n) => n.HighlightThisPointerON());

        puaseText.SetActive(true);
        rvBtn.SetActive(!MainManager.instance.pause);

        btnImage.sprite = MainManager.instance.pause ? playImage : puaseImage;
        gameObject.SetActive(false);
        moveTheStickText.SetActive(true);

        UpdateUI();

        // EventManager.instance.CustomEvent(AnalyticsType.RV, "Pause", true, true);
    }

    public void EndPause()
    {
        MainManager.instance.pause = false;

        if (MainManager.instance.rvPause)
        {
            ES3.Save<int>("RvPauseCount", Es3Extension.LoadDataWhenItExist<int>("RvPauseCount", 0) + 1);

            var value = Es3Extension.LoadDataWhenItExist<int>("RvPauseCount", 0);

            if ((value == 1 || (value % 3 == 0)) && !IapManager.instance.freePause)
            {

                this.TaskDelay(3f, () =>
                {
                    IapManager.instance.GenerateIapPopup("UI/iapOffer_FreePause", IapManager.iap_freePause);
                });
            }
        }

        MainManager.instance.rvPause = false;

        if (!MainManager.instance._ITR_MODE)
        {
            rvBtn.SetActive(!MainManager.instance.pause);
            btnImage.sprite = MainManager.instance.pause ? playImage : puaseImage;
            puaseText.SetActive(false);
            moveTheStickText.SetActive(false);

            gameObject.SetActive(true);
        }

        UpdateUI();
    }

    public void FirstPause()
    {
        rvBtn.SetActive(false);
    }

    public void ShowHide()
    {
        MainManager.instance.pause = !MainManager.instance.pause;

        puaseText.SetActive(MainManager.instance.pause);

        if (MainManager.instance.pause)
        {
            btnText.text = "Resume";
        }
        else
        {
            btnText.text = "Time Pause";
        }

        rvBtn.SetActive(!MainManager.instance.pause);

        btnImage.sprite = MainManager.instance.pause ? playImage : puaseImage;
    }
}
