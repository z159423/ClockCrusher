using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class MoneyOfferSideUI : MonoBehaviour
{
    TaskUtil.DelayTaskMethod showhideroutine = null;

    public Text moneyValueText;
    public float moneyValue;
    private bool enbale = false;

    private void OnDisable()
    {
        if (showhideroutine != null)
        {
            showhideroutine.Kill();
            showhideroutine = null;
        }
    }

    public void ShowHideRoutine()
    {
        transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0, 0, 0), 1f);

        Show();
    }


    [Button]
    public void Show()
    {
        enbale = true;

        if (showhideroutine != null)
        {
            showhideroutine.Kill();
            showhideroutine = null;
        }

        SetMoneyValue();

        gameObject.SetActive(true);

        transform.GetComponent<RectTransform>().DOAnchorPos3D(Vector3.zero, 1f);

        showhideroutine = MainManager.instance.TaskDelay(8f, () =>
        {
            Hide();
        });
    }

    public void Hide()
    {
        enbale = false;

        if (showhideroutine != null)
        {
            showhideroutine.Kill();
            showhideroutine = null;
        }

        transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(320, 0, 0), 1f);

        showhideroutine = MainManager.instance.TaskDelay(50f, () =>
        {
            Show();
        });
    }

    public void SetMoneyValue()
    {
        moneyValue = ((MainManager.instance.FindUpgrade(UpgradeType.AddStick).GetCurrentCost() + MainManager.instance.FindUpgrade(UpgradeType.Income).GetCurrentCost()) / 2f) * 1.6f;
        moneyValueText.text = "$" + MainManager.FormatNumber(moneyValue);
    }


    public void OnClickRVbtn()
    {
        if (enbale)
        {
            AdManager.instance.RV(() =>
        {
            SaveManager.instance.AddMoney(moneyValue);
            MainManager.instance.GenerateUIParticleAttractor(AdManager.instance.IRMoneyEndPoint, moneyValueText.transform);

            Hide();
            // EventManager.instance.CustomEvent(AnalyticsType.RV, "Gain Money Offer", true, true);
        },"Gain Money Offer");
        }
    }
}
