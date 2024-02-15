using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PocketWatchOpen : MonoBehaviour
{

    private void OnEnable()
    {
        GetComponentInParent<Clock>().onAfterStageStart += Animation;
    }

    public void Animation()
    {
        // dOTweenAnimation.DORestart();

        transform.DOLocalRotate(new Vector3(0, 180, 0), 1.5f, RotateMode.Fast);
    }
    public DOTweenAnimation dOTweenAnimation;
}
