using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RVBtnUI : MonoBehaviour
{
    [SerializeField] GameObject defaultUI;
    [SerializeField] GameObject rvTicketUI;

    private void Start() {
        SaveManager.instance.AddRvBtn(this);
        UpdateUI();
    }

    private void OnDestroy()
    {
        SaveManager.instance.RemoveRvBtn(this);
    }

    public void UpdateUI()
    {
        rvTicketUI.SetActive(SaveManager.instance.rvTicket > 0);
        defaultUI.SetActive(!(SaveManager.instance.rvTicket > 0));
    }
}
