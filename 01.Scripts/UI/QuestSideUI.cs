using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QuestSideUI : MonoBehaviour
{
    private void Start()
    {
        if (!ES3.KeyExists("EnableQuest"))
        {
            gameObject.SetActive(false);
        }
    }

    public void ShowAnimation()
    {
        this.TaskDelay(10, () =>
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(-280, GetComponent<RectTransform>().anchoredPosition.y);

            gameObject.SetActive(true);

            GetComponent<RectTransform>().DOAnchorPos3DX(0, 1f);

            ES3.Save<bool>("EnableQuest", true);
        });
    }
}
