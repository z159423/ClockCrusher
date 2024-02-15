using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;


public class PointerClicker : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerMoveHandler
{

    public ClockPointer selectedPointer;
    public ClockPointer mergeablePointer;


    public Transform vector1;

    public Vector3 touchPos;

    public LayerMask pointerLayer;
    public LayerMask backLayer;

    public float text;

    [Space]

    public float mergeableDist = 0.5f;

    private float startRotation;

    public void OnPointerUp(PointerEventData eventData)
    {
        if (selectedPointer != null && mergeablePointer != null)
        {
            // 머지가 가능한 상태라면 머지
            MainManager.instance.MergePointerInDrag(selectedPointer, mergeablePointer);

            selectedPointer.UnSelectThisPointer();
            mergeablePointer.HighlightThisPointerOFF();

            selectedPointer = null;
            mergeablePointer = null;

            if (MainManager.instance.tutorial1)
                MainManager.instance.CompleteMergeTutorial();
        }
        else if (selectedPointer != null)
        {
            if (MainManager.instance.tutorial1)
            {
                selectedPointer.transform.localRotation = Quaternion.Euler(Vector3.zero);
                selectedPointer.transform.Rotate(new Vector3(0, 0, startRotation));
            }

            EventManager.instance.CustomEvent(AnalyticsType.GAME, " Complete Drag Pointer");

            QuestManager.instance.OnProgressQuest(QuestType.MoveLvNStickToAnotherArea);

            if (Tutorial.insatnce.pauseTutorial)
                Tutorial.insatnce.EndDragPhase();

            if (Tutorial.insatnce.pauseTutorial)
            {
                Tutorial.insatnce.EndPauseTutorial();
            }
            else if (MainManager.instance.pause)
            {
                MainManager.instance.pauseBtn.EndPause();
                EventManager.instance.CustomEvent(AnalyticsType.GAME, " Moved Pointer " + selectedPointer.Level, true, true);
            }

            selectedPointer.UnSelectThisPointer();
            selectedPointer = null;
        }
        else if (mergeablePointer != null)
        {
            mergeablePointer.HighlightThisPointerOFF();
            mergeablePointer = null;
        }

        MainManager.instance.onDragPointer = false;
        MainManager.instance.Unfocus_SelectedStickAndMergeableStick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!MainManager.instance.pause)
            return;

        Ray ray = MainManager.instance.mainCamera.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        // 광선이 어떤 물체와 충돌했는지 확인
        if (Physics.Raycast(ray, out hit, int.MaxValue, pointerLayer))
        {
            if (hit.collider.gameObject.CompareTag("PointerClickPoint"))
            {
                HandleObjectClick(hit.collider.gameObject);

                if (Tutorial.insatnce.pauseTutorial)
                {
                    Tutorial.insatnce.tutorialFinger.gameObject.SetActive(false);
                }
            }
            // 충돌한 물체에 대한 동작 수행하는 함수 호출
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (selectedPointer != null)
        {
            Ray ray2 = MainManager.instance.mainCamera.ScreenPointToRay(eventData.position);

            RaycastHit hit2;

            int layerMask = 1 << LayerMask.NameToLayer("BackgroundWall");

            // 광선이 어떤 물체와 충돌했는지 확인
            if (Physics.Raycast(ray2, out hit2, int.MaxValue, backLayer))
            {
                touchPos = hit2.point;

                vector1.transform.position = hit2.point;
                // 충돌한 물체에 대한 동작 수행하는 함수 호출
            }

            selectedPointer.transform.localRotation = Quaternion.Euler(Vector3.zero);
            selectedPointer.transform.Rotate(new Vector3(0, 0, CalculateAngle2(selectedPointer.transform.position, new Vector3(touchPos.x, touchPos.y, 0))));

            // if (MainManager.instance.GetPointerMaxLevel() > selectedPointer.Level)
            //     FindMergeableStick();
        }
    }


    // void Update()
    // {
    //     return;

    //     // 터치 입력이 있을 때만 처리
    //     if (Input.touchCount > 0)
    //     {
    //         Touch touch = Input.GetTouch(0); // 첫 번째 터치만 고려

    //         Debug.LogError(011);

    //         // 터치가 시작된 경우
    //         if (touch.phase == TouchPhase.Began)
    //         {
    //             // 터치 포인터의 위치에서 화면으로 광선을 쏴서 충돌 검사
    //             Ray ray = Camera.main.ScreenPointToRay(touch.position);
    //             RaycastHit hit;

    //             // 광선이 어떤 물체와 충돌했는지 확인
    //             if (Physics.Raycast(ray, out hit, int.MaxValue, pointerLayer))
    //             {
    //                 if (hit.collider.gameObject.CompareTag("PointerClickPoint"))
    //                 {
    //                     HandleObjectClick(hit.collider.gameObject);
    //                 }

    //                 // 충돌한 물체에 대한 동작 수행하는 함수 호출
    //             }
    //         }

    //         if (selectedPointer != null)
    //         {
    //             Ray ray2 = Camera.main.ScreenPointToRay(touch.position);

    //             RaycastHit hit2;

    //             int layerMask = 1 << LayerMask.NameToLayer("BackgroundWall");

    //             // 광선이 어떤 물체와 충돌했는지 확인
    //             if (Physics.Raycast(ray2, out hit2, int.MaxValue, backLayer))
    //             {
    //                 touchPos = hit2.point;

    //                 vector1.transform.position = hit2.point;
    //                 // 충돌한 물체에 대한 동작 수행하는 함수 호출
    //             }

    //             selectedPointer.transform.localRotation = Quaternion.Euler(Vector3.zero);
    //             selectedPointer.transform.Rotate(new Vector3(0, 0, CalculateAngle2(selectedPointer.transform.position, new Vector3(touchPos.x, touchPos.y, 0))));

    //             if (MainManager.instance.GetPointerMaxLevel() > selectedPointer.Level)
    //                 FindMergeableStick();
    //         }

    //         if (touch.phase == TouchPhase.Ended)
    //         {
    //             if (selectedPointer != null && mergeablePointer != null)
    //             {
    //                 // 머지가 가능한 상태라면 머지
    //                 MainManager.instance.MergePointerInDrag(selectedPointer, mergeablePointer);

    //                 selectedPointer.UnSelectThisPointer();
    //                 mergeablePointer.HighlightThisPointerOFF();

    //                 selectedPointer = null;
    //                 mergeablePointer = null;

    //                 if (MainManager.instance.tutorial1)
    //                     MainManager.instance.CompleteMergeTutorial();
    //             }
    //             else if (selectedPointer != null)
    //             {
    //                 if (MainManager.instance.tutorial1)
    //                 {
    //                     selectedPointer.transform.localRotation = Quaternion.Euler(Vector3.zero);
    //                     selectedPointer.transform.Rotate(new Vector3(0, 0, startRotation));
    //                 }

    //                 selectedPointer.UnSelectThisPointer();
    //                 selectedPointer = null;

    //             }
    //             else if (mergeablePointer != null)
    //             {
    //                 mergeablePointer.HighlightThisPointerOFF();
    //                 mergeablePointer = null;
    //             }

    //             MainManager.instance.onDragPointer = false;
    //             MainManager.instance.Unfocus_SelectedStickAndMergeableStick();

    //         }
    //     }
    // }

    void HandleObjectClick(GameObject clickedObject)
    {
        selectedPointer = clickedObject.GetComponentInParent<ClockPointer>();

        MainManager.instance.onDragPointer = true;

        MainManager.instance.pointerList.ForEach((n) => n.HighlightThisPointerOFF());

        selectedPointer.SelectThisPointer();

        startRotation = selectedPointer.transform.eulerAngles.z;

        MainManager.instance.Focus_OnlySelectedStick(clickedObject.GetComponentInParent<ClockPointer>());

        // if (MainManager.instance.GetPointerMaxLevel() > selectedPointer.Level)
        //     MainManager.instance.Focus_SelectedStickAndMergeableStick(selectedPointer.Level);
    }

    public void FindMergeableStick()
    {
        var find = MainManager.instance.pointerList.Where((n) => n.Level == selectedPointer.Level && n != selectedPointer && (Vector3.Distance(n.transform.up * 0.5f, selectedPointer.transform.up * 0.5f) < mergeableDist)).OrderBy((n) => Vector3.Distance(n.transform.up * 0.5f, selectedPointer.transform.up * 0.5f));

        if (find.Count() > 0)
        {
            print(Vector3.Distance(find.First().transform.up * 0.5f, selectedPointer.transform.up * 0.5f));

            if (mergeablePointer != null)
            {
                if (mergeablePointer != find.First())
                {
                    mergeablePointer.HighlightThisPointerOFF();

                    mergeablePointer = find.First().GetComponent<ClockPointer>();
                    find.First().GetComponent<ClockPointer>().HighlightThisPointerON();
                }
            }
            else
            {
                mergeablePointer = find.First().GetComponent<ClockPointer>();
                find.First().GetComponent<ClockPointer>().HighlightThisPointerON();
            }
        }
        else if (mergeablePointer != null)
        {
            mergeablePointer.HighlightThisPointerOFF();
            mergeablePointer = null;
        }
    }


    // 예시 함수: AB 벡터와 BC 벡터 사이의 내각을 구하는 함수
    public static float CalculateAngle(Vector2 A, Vector2 B, Vector2 C)
    {
        // AB 벡터와 BC 벡터 계산
        Vector2 AB = B - A;
        Vector2 BC = C - B;

        Debug.DrawLine(B, A);
        Debug.DrawLine(C, B);

        // 내적 계산
        float dotProduct = Vector2.Dot(AB.normalized, BC.normalized);

        // 각도 계산 (라디안)
        float angleRad = Mathf.Acos(Mathf.Clamp(dotProduct, -1f, 1f));

        // 라디안에서 도로 변환
        float angleDegree = Mathf.Rad2Deg * angleRad;

        return Mathf.Abs(angleDegree - 180f);
    }

    public static float CalculateAngle2(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }


}
