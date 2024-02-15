using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIText : MonoBehaviour
{
    public SaveTextType type;


    private void OnEnable()
    {
        this.TaskWaitUntil(() => SaveManager.instance.AddText(type, GetComponent<Text>()), () => SaveManager.instance != null);
    }

    private void OnDestroy()
    {
        SaveManager.instance.RemoveText(type, GetComponent<Text>());
    }

    public void ShowShopUI()
    {
        EventManager.instance.CustomEvent(AnalyticsType.UI, "OnClickSmallPlusBtnInRVTicket", true, true);
        MainManager.instance.ShowShopUI(0);
    }
}
