using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class CameraZoom : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public float zoomSpeed = 0.5f;

    public float maxZoom = 4f;
    public float minZoom = 1.5f;

    public Camera _camera;

    Vector2 mPrevPos;


    public void OnBeginDrag(PointerEventData data)
    {
        mPrevPos = data.position;
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.clickCount >= 2)
        {
            var mDelta = (mPrevPos - data.position);

            StageManager.instance.currentClock.transform.Rotate(mDelta);

            mPrevPos = data.position;
        }
    }


    // void Update()
    // {
    //     float scroll = Input.GetAxis("Mouse ScrollWheel");

    //     // Orthographic Size 변경
    //     _camera.orthographicSize -= scroll * zoomSpeed;
    //     _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom);

    //     // 터치 수 확인
    //     if (Input.touchCount == 2)
    //     {
    //         // 두 터치 간의 거리 계산
    //         Touch touch0 = Input.GetTouch(0);
    //         Touch touch1 = Input.GetTouch(1);
    //         Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
    //         Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
    //         float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
    //         float touchDeltaMag = (touch0.position - touch1.position).magnitude;
    //         float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

    //         _camera.orthographicSize += deltaMagnitudeDiff * zoomSpeed;
    //         _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom);

    //         // 줌 인/아웃
    //         // freeLookCamera.m_Orbits[1].m_SplineCurvature += deltaMagnitudeDiff * zoomSpeed;
    //         // freeLookCamera.m_Orbits[1].m_SplineCurvature = Mathf.Clamp(freeLookCamera.m_Orbits[1].m_SplineCurvature, 0.1f, 10f);
    //     }
    // }
}
