using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class RvSideUI : MonoBehaviour
{
    public RVType type;

    // moneyFeverReady
    // AutoFeverReady
    // speedFeverReady
    public GameObject rvBtn;
    public GameObject timeGague;
    public Image timeGagueFill;

    TaskUtil.DelayTaskMethod delayTaskMethod = null;

    [Space]

    [ReadOnly] public string currentStatus = "";

    public void Start()
    {
        gameObject.SetActive(false);

        if (MainManager.instance._ITR_MODE)
            return;

        currentStatus = "Start";


        switch (type)
        {
            case RVType.MoneyFever:

                if (ES3.KeyExists("moneyFeverReady") ? ES3.Load<bool>("moneyFeverReady") : false)
                    AdManager.instance.TaskDelay(15f, () => { gameObject.SetActive(true); Show(); });

                break;

            case RVType.AutoFever:

                if (ES3.KeyExists("autoFeverReady") ? ES3.Load<bool>("autoFeverReady") : false)
                    AdManager.instance.TaskDelay(20f, () => { gameObject.SetActive(true); Show(); });

                break;

            case RVType.SpeedFever:

                if ((ES3.KeyExists("speedFeverReady") ? ES3.Load<bool>("speedFeverReady") : false) && !IapManager.instance.starterPack)
                    AdManager.instance.TaskDelay(30f, () => { gameObject.SetActive(true); Show(); });

                break;
        }
    }

    TaskUtil.DelayTaskMethod showhideroutine = null;

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

    public void ActiveFever(float duration, bool freeChance = false)
    {
        Show();

        rvBtn.gameObject.SetActive(false);
        timeGague.SetActive(true);

        timeGagueFill.fillAmount = 1f;
        timeGagueFill.DOFillAmount(0, freeChance ? duration / 2 : duration);
        // moneyFeverTimeGagueFill.GetComponent<RectTransform>().sizeDelta = new Vector2(654, 78);
        // moneyFeverTimeGagueFill.GetComponent<RectTransform>().DOSizeDelta(new Vector2(0, 78), moneyFeverDuration);

        AdManager.instance.TaskDelay(freeChance ? duration / 2 : duration, () =>
        {
            AdManager.instance.DeActiveFever(type);
            rvBtn.gameObject.SetActive(true);
            timeGague.SetActive(false);
        });
    }

    // IEnumerator showHideRoutine()
    // {
    //     while (true)
    //     {
    //         Show();

    //         yield return new WaitForSeconds(15f);

    //         Hide();

    //         yield return new WaitForSeconds(30f);
    //     }
    // }

    [Button]
    public void Show()
    {
        if (MainManager.instance._ITR_MODE)
            return;

        if (type == RVType.SpeedFever && IapManager.instance.starterPack)
            return;

        if (!ES3.KeyExists(type + "_firstShow") && type == RVType.SpeedFever && !MainManager.instance.isReadyOnRV)
        {
            gameObject.SetActive(true);

            var popup = Instantiate(Resources.Load<GameObject>("UI/" + type + "_FreeRVPopUp"), MainManager.instance.UI);

            popup.GetComponentInChildren<ExtendedRewardPopUp>().Init();
            popup.GetComponentInChildren<ExtendedRewardPopUp>().onClickFreeChanceBtnEvent += Show;

            ES3.Save<bool>(type + "_firstShow", true);
        }
        else
        {
            if (showhideroutine != null)
            {
                showhideroutine.Kill();
                showhideroutine = null;
            }

            gameObject.SetActive(true);

            transform.GetComponent<RectTransform>().DOAnchorPos3D(Vector3.zero, 1f);

            showhideroutine = MainManager.instance.TaskDelay(15f, () =>
            {
                currentStatus = "Wait Hide";

                Hide();
            });
        }
    }

    IEnumerator _Show()
    {
        transform.GetComponent<RectTransform>().DOAnchorPos3D(Vector3.zero, 1f);

        yield return new WaitForSeconds(15f);

        Hide();
    }

    public void Hide()
    {
        if (showhideroutine != null)
        {
            showhideroutine.Kill();
            showhideroutine = null;
        }

        if (!AdManager.instance.GetFeverActive(type))
            transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(320, 0, 0), 1f);

        showhideroutine = MainManager.instance.TaskDelay(20f, () =>
        {
            currentStatus = "Wait Show";

            Show();
        });
    }


    IEnumerator _Hide()
    {
        if (!AdManager.instance.GetFeverActive(type))
            transform.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(320, 0, 0), 1f);

        yield return new WaitForSeconds(30f);

        Show();
    }


    public void OnClickRVbtn()
    {
        AdManager.instance.OnClickRVbtn(type);
    }
}
