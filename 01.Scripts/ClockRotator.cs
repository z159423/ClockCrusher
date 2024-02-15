using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClockRotator : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    Vector2 mPrevPos;

    public void OnBeginDrag(PointerEventData data)
    {
        mPrevPos = data.position;
        // print(data.position);
    }

    public void OnDrag(PointerEventData data)
    {
        // data.clickCount

        // var mDelta = (mPrevPos - data.position);

        // StageManager.instance.currentClock.transform.Rotate(mDelta);

        // mPrevPos = data.position;

        // print(mDelta);
    }
}
