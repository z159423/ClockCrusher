using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class ClearUI : MonoBehaviour
{
    [SerializeField] Text moneyText;
    [SerializeField] GameObject noThxBtn;
    [SerializeField] Transform pointer;
    [SerializeField] DOTweenAnimation animation;
    [SerializeField] GameObject fanfare;
    [SerializeField] GameObject rvBtn;

    TaskUtil.DelayTaskMethod notahnksTask = null;

    float multifly = 1;

    public float angle;

    private void OnEnable()
    {
        rvBtn.SetActive(true);
        notahnksTask = this.TaskDelay(3.5f, () => noThxBtn.SetActive(true));
    }

    private void OnDisable()
    {
        noThxBtn.SetActive(false);
    }

    private void Update()
    {
        angle = pointer.localRotation.z;

        if ((pointer.localRotation.z < 0.75f && pointer.localRotation.z > 0.54f) || (pointer.localRotation.z < -0.54f && pointer.localRotation.z > -0.75f))
        {
            multifly = 1f;
        }
        else if ((pointer.localRotation.z < 0.34f && pointer.localRotation.z > 0.54f) || (pointer.localRotation.z < -0.34f && pointer.localRotation.z > -0.54f))
        {
            multifly = 2f;
        }
        else if ((pointer.localRotation.z < 0.13f && pointer.localRotation.z > 0.34f) || (pointer.localRotation.z < -0.13f && pointer.localRotation.z > -0.34f))
        {
            multifly = 3f;
        }
        else if ((pointer.localRotation.z < 0.13f && pointer.localRotation.z > -0.13f))
        {
            multifly = 5f;
        }

        moneyText.text = (StageManager.instance.currentStage * 500 * multifly).ToString();
    }

    public void OnClickFreeBtn()
    {
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "_Try_Roulette");
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "_Try_Roulette_" + multifly);

        SaveManager.instance.AddMoney(StageManager.instance.currentStage * 500 * multifly);
        animation.DOKill();

        notahnksTask.Kill();

        fanfare.SetActive(true);

        rvBtn.SetActive(false);

        this.TaskDelay(3.5f, () =>
        {
            AdManager.instance.RV(() =>
        {
            StageManager.instance.NextStage();
            noThxBtn.SetActive(false);
        }, "_Try_Roulette");
        });
    }
}
