using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPad : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        // QuestManager.instance.OnProgressQuest(QuestType.TapNTimes);

        MainManager.instance.PadTouchEvent();
        MainManager.instance.PopParticle("Pooling/Particle/ClickParticle", eventData.pointerPressRaycast.worldPosition, Quaternion.identity, MainManager.instance.UI);

    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnTouch()
    {
        QuestManager.instance.OnProgressQuest(QuestType.TapNTimes);
        MainManager.instance.PadTouchEvent();
    }
}
