using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks.Linq;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] private UpgradeType type;
    [SerializeField] private Text LvText;
    [SerializeField] private Text costText;
    [SerializeField] private Button button;
    [SerializeField] private Image costImage;
    [SerializeField] private Image backframe;

    [SerializeField] private GameObject[] RVblacks;
    [SerializeField] private GameObject rvBtn;
    [SerializeField] private GameObject purpleRvBtn;

    TaskUtil.DelayTaskMethod showPurpleRVTask = null;
    TaskUtil.DelayTaskMethod hidePurpleRVTask = null;

    private Upgrade currentUpgrade;


    private void Start()
    {
        UpdateUI(MainManager.instance.FindUpgrade(type));

        currentUpgrade = MainManager.instance.FindUpgrade(type);
        SaveManager.instance.onChangeMoneyEvent += UpdateUI;

        if (type != UpgradeType.Merge && type != UpgradeType.Income)
        {
            if (ES3.KeyExists("PurpleRV_" + type))
            {
                showPurpleRV(30);
            }
            else
            {
                ES3.Save<bool>("PurpleRV_" + type, true);
                showPurpleRV(60);
            }
        }

    }



    private void OnDisable()
    {
        // SaveManager.instance.onChangeMoneyEvent -= UpdateUI;
    }

    public void OnClickUpgradeBtn()
    {
        var upgrade = MainManager.instance.FindUpgrade(type);

        if (MainManager.instance.GetCost(upgrade) > SaveManager.instance.money || MainManager.instance.tutorial1)
            return;

        Managers.Sound.Play("Sound/Pop");

        SaveManager.instance.UseMoney(MainManager.instance.GetCost(upgrade));

        switch (type)
        {
            case UpgradeType.AddStick:
                MainManager.instance.Upgrade_AddStick();
                break;

            case UpgradeType.Speed:
                MainManager.instance.Upgarde_Speed();
                break;

            case UpgradeType.Income:
                MainManager.instance.Upgrade_Income();
                break;
        }

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Upgrade - " + type, true, true);
        EventManager.instance.CustomEvent(AnalyticsType.UI, "On Click Upgrade Btn - " + type, true, true);

        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack)
            .OnComplete(() => transform.DOScale(1f, 0.15f));

        UpdateUI(MainManager.instance.FindUpgrade(type));
    }

    public void OnClickRVBtn()
    {
        var upgrade = MainManager.instance.FindUpgrade(type);

        // if (GetCost(upgrade) > SaveManager.instance.money)
        //     return;

        Managers.Sound.Play("Sound/Pop");

        // SaveManager.instance.UseMoney(GetCost(upgrade));

        switch (type)
        {
            case UpgradeType.AddStick:
                AdManager.instance.RV(() =>
        {
            MainManager.instance.Upgrade_AddStick(); MainManager.instance.Upgrade_AddStick();
        }, "Upgrade Free - " + type);
                break;

            case UpgradeType.Speed:
                AdManager.instance.RV(() =>
            {
                MainManager.instance.Upgarde_Speed(); MainManager.instance.Upgarde_Speed();
            }, "Upgrade Free - " + type);
                break;

            case UpgradeType.Income:
                AdManager.instance.RV(() =>
            {
                MainManager.instance.Upgrade_Income();
            }, "Upgrade Free - " + type);
                break;
        }

        // EventManager.instance.CustomEvent(AnalyticsType.RV, "Upgrade Free - " + type, true, true);

        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack)
            .OnComplete(() => transform.DOScale(1f, 0.15f));

        UpdateUI(MainManager.instance.FindUpgrade(type));
    }

    public void UpdateUI(Upgrade upgrade)
    {
        LvText.text = "Lv. " + (upgrade.currentLevel);
        if (upgrade.currentLevel >= upgrade.maxLevel)
        {
            if (upgrade.type == UpgradeType.AddStick || upgrade.type == UpgradeType.Merge)
            {

            }
            else
            {
                button.enabled = false;
                costText.text = "MAX";
                costText.color = Color.white;
            }
        }
        else
        {
            button.enabled = true;
            costText.text = MainManager.FormatNumber(MainManager.instance.GetCost(upgrade)).ToString();

            if (MainManager.instance.GetCost(upgrade) > SaveManager.instance.money)
            {
                Color color;
                ColorUtility.TryParseHtmlString("#FF6161", out color);
                costText.color = color;
            }
            else
            {
                costText.color = Color.white;
            }
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < RVblacks.Length; i++)
            RVblacks[i].SetActive(false);
        rvBtn.SetActive(false);


        LvText.text = "Lv. " + (currentUpgrade.currentLevel);

        if ((type == UpgradeType.AddStick && StageManager.instance.currentClock != null && MainManager.instance.pointerList.Count >= StageManager.instance.currentClock.maxPointerCount) ||
                    (currentUpgrade.currentLevel >= currentUpgrade.maxLevel && type != UpgradeType.AddStick && type != UpgradeType.Merge))
        {
            button.enabled = false;
            costText.text = "MAX";
            costText.color = Color.white;

            costImage.sprite = Resources.Load<Sprite>("btn_Upgrade_Gray_01");
            backframe.sprite = Resources.Load<Sprite>("btn_Upgrade_02");
        }
        else
        {
            button.enabled = true;
            costText.text = MainManager.FormatNumber(MainManager.instance.GetCost(currentUpgrade)).ToString();

            if (MainManager.instance.GetCost(currentUpgrade) > SaveManager.instance.money)
            {
                Color color;
                ColorUtility.TryParseHtmlString("#FF6161", out color);
                costText.color = color;

                ActiveUpgradeUV();
            }
            else
            {
                LvText.text = "Lv. " + (currentUpgrade.currentLevel);
                button.enabled = true;
                costText.text = MainManager.FormatNumber(MainManager.instance.GetCost(currentUpgrade)).ToString();
                costText.color = Color.white;

                costImage.sprite = Resources.Load<Sprite>("btn_Upgrade_01");
                backframe.sprite = Resources.Load<Sprite>("btn_Upgrade_02");
            }
        }
        // }


    }

    public void ActiveUpgradeUV()
    {
        // if (ES3.KeyExists("UpgardeRVActive") ? ES3.Load<bool>("UpgardeRVActive") : false)
        // {

        if (MainManager.instance.UpgardeRVActive)
        {
            // foreach (var black in RVblacks)
            //     black.SetActive(true);

            for (int i = 0; i < RVblacks.Length; i++)
                RVblacks[i].SetActive(true);

            costImage.sprite = Resources.Load<Sprite>("btn_Upgrade_Gray_01");
            backframe.sprite = Resources.Load<Sprite>("btn_Upgrade_Gray_02");

            if (!MainManager.instance._ITR_MODE)
                if (!purpleRvBtn.activeSelf)
                    rvBtn.SetActive(!currentUpgrade.CheckIsMax());
        }
    }

    void showPurpleRV(float SEC)
    {
        if (MainManager.instance._ITR_MODE)
            return;

        MainManager.instance.TaskDelay(SEC, () =>
        {
            if (MainManager.instance.FindUpgrade(type).CheckIsMax())
            {

            }
            else
            {
                purpleRvBtn.SetActive(true);
                rvBtn.SetActive(false);
            }

            hidePurpleRVTask = MainManager.instance.TaskDelay(10, () => HidePurpleRVBtn());
        });
    }

    public void ShowPurpleRVBtn(bool first = false)
    {
        if (MainManager.instance._ITR_MODE)
            return;

        if (MainManager.instance.FindUpgrade(type).CheckIsMax())
        {

        }
        else
        {
            purpleRvBtn.SetActive(true);
            rvBtn.SetActive(false);
        }

        hidePurpleRVTask = MainManager.instance.TaskDelay(10, () => HidePurpleRVBtn());
    }

    public void HidePurpleRVBtn()
    {
        purpleRvBtn.SetActive(false);

        showPurpleRVTask = MainManager.instance.TaskDelay(50, () => ShowPurpleRVBtn());
    }

    public void OnClickPurpleRVBtn()
    {
        AdManager.instance.RV(() =>
        {
            if (showPurpleRVTask != null)
                showPurpleRVTask.Kill();

            if (hidePurpleRVTask != null)
                hidePurpleRVTask.Kill();

            purpleRvBtn.SetActive(false);


            showPurpleRV(60);

            var upgrade = MainManager.instance.FindUpgrade(type);
            Managers.Sound.Play("Sound/Pop");

            switch (type)
            {
                case UpgradeType.AddStick:
                    MainManager.instance.Upgrade_AddStick(); MainManager.instance.Upgrade_AddStick(); MainManager.instance.Upgrade_AddStick();
                    break;

                case UpgradeType.Speed:
                    MainManager.instance.Upgarde_Speed(); MainManager.instance.Upgarde_Speed(); MainManager.instance.Upgarde_Speed();
                    break;

                case UpgradeType.Income:
                    MainManager.instance.Upgrade_Income(); MainManager.instance.Upgrade_Income();
                    break;
            }

            // EventManager.instance.CustomEvent(AnalyticsType.RV, "Upgrade Free Purple - " + type, true, true);

            transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack)
                .OnComplete(() => transform.DOScale(1f, 0.15f));

            UpdateUI(MainManager.instance.FindUpgrade(type));
        }, "Upgrade Free Purple - " + type);
    }
}
