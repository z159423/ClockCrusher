using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stopper : MonoBehaviour
{
    public Vector3 btnOffset;

    public bool blocked = false;

    [SerializeField] StopperBtn btn;
    [SerializeField] Collider _collider;
    [SerializeField] MeshRenderer _renderer;

    [Space]
    [SerializeField] Material activeMat;
    [SerializeField] Material deactiveMat;


    private void Awake()
    {

    }

    public void Show()
    {
        gameObject.SetActive(true);
        btn.gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);

    }

    public void On()
    {

    }

    public void Off()
    {

    }

    public void OnClick()
    {
        Turn();

        // blocked = !blocked;
    }

    public void Turn()
    {
        blocked = !blocked;

        transform.DOScale(0.8f, 0.2f).SetEase(Ease.OutBack)
            .OnComplete(() => transform.DOScale(0.7f, 0.15f));

        // _collider.enabled = blocked;
        _renderer.material = blocked ? activeMat : deactiveMat;
        btn.GetComponent<UnityEngine.UI.Image>().color = blocked ? Color.white : Color.gray;
    }
}
