using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StickCollectionBtn : MonoBehaviour
{
    [SerializeField] GameObject dot;
    [SerializeField] public Image icon;


    private void Start()
    {
        gameObject.SetActive(ES3.KeyExists("StickCollectionEnable") ? ES3.Load<bool>("StickCollectionEnable") : false);

        UpdateIcon();
    }

    public void ShowAnimation()
    {
        this.TaskDelay(3f, () =>
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(-280, GetComponent<RectTransform>().anchoredPosition.y);

            gameObject.SetActive(true);

            GetComponent<RectTransform>().DOAnchorPos3DX(0, 1f);
        });
    }

    public void UpdateDot()
    {
        dot.SetActive(ES3.KeyExists("NewStickInCollection") ? ES3.Load<bool>("NewStickInCollection") : false);
    }

    public void UpdateIcon()
    {
        if (MainManager.instance.pointerSkinType == PointerSkinType.Default)
            icon.sprite = Resources.Load<Sprite>("icon_MySticks_B");
        else
            icon.sprite = Resources.Load<Sprite>("icon_scRV");


    }

    public void OnClickBtn()
    {
        Instantiate(Resources.Load<GameObject>("UI/StickCollectionUI"), MainManager.instance.UI).GetComponent<StickCollectionUI>().Show();

        EventManager.instance.CustomEvent(AnalyticsType.UI, " OnClick Stick Collection Btn", true, true);
    }
}
