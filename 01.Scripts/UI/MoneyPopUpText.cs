using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MoneyPopUpText : MonoBehaviour
{
    [SerializeField] Text moneyText;

    public void Init(float value)
    {
        moneyText.text = "+" + MainManager.FormatNumber(value);

        GetComponentInChildren<Animator>().SetTrigger("popUp");
    }

    public void Push()
    {
        Managers.Pool.Push(GetComponentInParent<Poolable>());
    }
}
