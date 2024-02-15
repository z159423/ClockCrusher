using UnityEngine;

public class TouchZoom : MonoBehaviour
{
    public float zoomSpeed = 0.5f;
    public float minSize = 1.8f;
    public float maxSize = 3f;

    public float zoomTime = 0.2f;

    private Camera cam;
    private float initialPinchDistance;

    private float pressTime = 0;


    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // void Update()
    // {
    //     // 터치 입력 감지
    //     if (Input.touchCount == 2)
    //     {
    //         pressTime += Time.deltaTime;

    //         Touch touch1 = Input.GetTouch(0);
    //         Touch touch2 = Input.GetTouch(1);

    //         if (touch1.phase == TouchPhase.Ended)
    //         {
    //             pressTime = 0;
    //         }

    //         if (touch1.phase == TouchPhase.Ended)
    //         {
    //             pressTime = 0;
    //         }

    //         if (pressTime > zoomTime)
    //         {
    //             // 현재 두 터치 간의 거리 계산
    //             float pinchDistance = Vector2.Distance(touch1.position, touch2.position);


    //             if (touch2.phase == TouchPhase.Began)
    //             {
    //                 // 줌 시작 시 초기 거리 저장
    //                 initialPinchDistance = pinchDistance;
    //             }

    //             // 줌인 또는 줌아웃
    //             float pinchDelta = (pinchDistance - initialPinchDistance) * zoomSpeed * Time.deltaTime;
    //             cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - pinchDelta, minSize, maxSize);
    //             print(Mathf.Clamp(cam.orthographicSize - pinchDelta, minSize, maxSize));

    //             // 초기 거리 갱신
    //             initialPinchDistance = pinchDistance;
    //         }
    //     }
    // }
}