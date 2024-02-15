using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingBtn : MonoBehaviour
{
    public void OpenSettingUI()
    {
        var ui = Managers.UI.ShowPopupUI<UI_Popup>("UI_PopupSetting");

        var rect = ui.GetComponent<RectTransform>();
        rect.GetComponent<Canvas>().sortingOrder = 200;

        rect.anchorMin = new Vector3(0, 0);
        rect.anchorMax = new Vector3(1, 1);

        rect.offsetMin = new Vector2(0, 0);
        rect.offsetMax = new Vector2(0, 0);

        rect.localScale = Vector3.one;

        rect.anchoredPosition3D = Vector3.zero;

        // EventManager.instance.CustomEvent(AnalyticsType.UI, "OpenSetting", true, true);
    }
}
