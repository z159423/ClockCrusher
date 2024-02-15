using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class IapPriceLocalize : MonoBehaviour
{
    public string iapcode;

    public bool aos = true;
    public bool ios = true;


    private void OnEnable()
    {
#if !UNITY_EDITOR

        (string code, string price) = MondayOFF.IAPManager.GetLocalizedPrice(iapcode);
        GetComponent<Text>().text = code + GetOnlyNumericValue(price);

#endif

    }

    private string GetOnlyNumericValue(string priceWithSymbol)
    {
        // 숫자만 추출
        string numericValue = new string(priceWithSymbol.Where(char.IsDigit).ToArray());
        return numericValue;
    }
}
