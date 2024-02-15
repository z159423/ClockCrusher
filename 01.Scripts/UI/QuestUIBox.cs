using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuestUIBox : MonoBehaviour
{
    public Image moneyIcon;
    public Image rvTicketIcon;
    public Text rewardText;

    public bool show = false;


    private void Start()
    {
        if (!ES3.KeyExists("EnableQuest"))
        {
            gameObject.SetActive(false);
        }

    }

    public void ShowAnimation()
    {
        if (show)
            return;
            
        this.TaskDelay(0, () =>
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, 150);

            gameObject.SetActive(true);

            GetComponent<RectTransform>().DOAnchorPos3DY(-420, 1f).SetEase(Ease.InOutQuad);

            show = true;
        });
    }
}
