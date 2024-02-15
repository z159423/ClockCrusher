using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSideUI : MonoBehaviour
{
    [SerializeField] public GameObject dot;

    private void Start()
    {
        gameObject.SetActive(ES3.KeyExists("enableShop") ? ES3.Load<bool>("enableShop") : false);
    }

    public void OnClickShopBtn()
    {
        EventManager.instance.CustomEvent(AnalyticsType.UI, "OnClickSideShopBtn", true, true);
        MainManager.instance.ShowShopUI(1);
    }
}
