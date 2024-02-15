using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture = null;
    [SerializeField] Texture2D cursorTexture_clicked = null;
    [SerializeField] Vector2 hotSpot;
    bool isShowCursor = false;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
    // private void Start()
    // {
    //     StartCoroutine(SetCursor());
    // }

    // IEnumerator SetCursor()
    // {
    //     yield return new WaitForEndOfFrame();

    //     Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    // }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isShowCursor = !isShowCursor;
            Cursor.SetCursor(isShowCursor == true ? cursorTexture : null, hotSpot, CursorMode.Auto);
        }

        if (isShowCursor)
            Cursor.SetCursor(cursorTexture_clicked, hotSpot, CursorMode.ForceSoftware);
            
        else
        if (Input.GetMouseButtonDown(0) && isShowCursor)
        {
            Cursor.SetCursor(cursorTexture_clicked, hotSpot, CursorMode.ForceSoftware);
        }
        if (Input.GetMouseButtonUp(0) && isShowCursor)
        {
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.ForceSoftware);
        }

    }
#endif
    // private void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         fingerImage.anchoredPosition = Input.mousePosition * 2688f / Screen.height;
    //         fingerImage.transform.DOScale(Vector3.one * 0.8f, 0.1f).SetEase(Ease.Linear);
    //     }
    //     if (Input.GetMouseButton(0))
    //     {
    //         fingerImage.anchoredPosition = Input.mousePosition * 2688f / Screen.height;
    //     }
    //     if (Input.GetMouseButtonUp(0))
    //     {
    //         fingerImage.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.Linear);
    //     }
    // }
}
