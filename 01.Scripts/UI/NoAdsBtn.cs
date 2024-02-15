using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAdsBtn : MonoBehaviour
{
    void Start()
    {
        if (ES3.KeyExists("EnableNoAdsBtn") && !MondayOFF.NoAds.IsNoAds)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
