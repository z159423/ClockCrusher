using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExtendedRewardPopUp : MonoBehaviour
{
    public RVType type;

    public System.Action onClickFreeChanceBtnEvent;

    public Button btn;

    public Image icon;

    public void Init()
    {
        this.TaskDelay(1f, () => { btn.enabled = true; });
    }

    public void OnClickFreeChance()
    {
        AdManager.instance.OnClickFreeChance(type);


        onClickFreeChanceBtnEvent.Invoke();

        MondayOFF.AdsManager.SetIsTime(Time.realtimeSinceStartup - 40);
        AdManager.instance.enableIS = true;

        MainManager.instance.GenerateImageMotion(icon, icon.transform, AdManager.instance._speedFeverBtn.transform);

        SaveManager.instance.UseRVTicket(1);

        Hide();
    }

    public void Hide()
    {
        Destroy(gameObject);
    }
}
