using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using DG.Tweening;

public class MoneyDropMesh : MonoBehaviour
{
    public static float numberBetweenDist = 0.08f;
    [SerializeField] GameObject[] numberObjects;
    [SerializeField] Transform dollerUnit;


    public float value;

    private Vector3 baseScale = Vector3.one;

    private void Awake()
    {
        baseScale = transform.localScale;
    }



    public void ChangeValue(float value)
    {

        int objCount = 0;

        NumbericUnit numbericUnit;
        short shortNum = 0;

        //단위 계산
        if (value > 1000)
        {
            numberObjects[3].GetComponentInChildren<MeshRenderer>(true).enabled = true;
        }
        else
            numberObjects[objCount].GetComponentInChildren<MeshRenderer>(true).enabled = false;

        if (value <= 0)
        {
            gameObject.SetActive(false);
        }
        else if (value > 1000000000)
        {
            shortNum = (short)(value / 1000000000);
            numbericUnit = NumbericUnit.B;

            ChangeNumberMesh(shortNum, numbericUnit);
        }
        else if (value > 1000000)
        {
            shortNum = (short)(value / 1000000);
            numbericUnit = NumbericUnit.M;

            ChangeNumberMesh(shortNum, numbericUnit);
        }
        else if (value > 1000)
        {
            shortNum = (short)(value / 1000);
            numbericUnit = NumbericUnit.K;

            ChangeNumberMesh(shortNum, numbericUnit);
        }
        else if (value > 0)
        {
            shortNum = (short)value;
            numbericUnit = NumbericUnit.None;

            ChangeNumberMesh(shortNum, numbericUnit);
        }

        void ChangeNumberMesh(short _shortNum, NumbericUnit _numbericUnit)
        {
            List<Transform> list = new List<Transform>();

            list.Add(dollerUnit);

            numberObjects.ToList().ForEach((n) =>
            {
                n.GetComponentInChildren<MeshRenderer>(true).enabled = false;
            });

            objCount = 0;

            foreach (char digitChar in _shortNum.ToString())
            {
                var digit = int.Parse(digitChar.ToString());
                numberObjects[objCount].GetComponentInChildren<MeshRenderer>(true).enabled = true;
                numberObjects[objCount].GetComponentInChildren<MeshFilter>(true).mesh = MainManager.instance.GetNumberObject(digit);
                objCount++;
            }

            numberObjects[3].GetComponentInChildren<MeshFilter>(true).mesh = MainManager.instance.GetNumbericUnit(_numbericUnit);

            list.AddRange(numberObjects.Where((n) => n.GetComponentInChildren<MeshRenderer>(true).enabled).Select((n) => n.transform).ToList());

            ArrangeNumberText(list);
        }
    }

    public void ArrangeNumberText(List<Transform> nums)
    {
        float middleValue = ((numberBetweenDist * nums.Count) * 0.5f);

        for (int i = 0; i < nums.Count; i++)
        {
            nums[i].transform.localPosition = new Vector3((numberBetweenDist * (i + 1)) - middleValue, nums[i].transform.localPosition.y, nums[i].transform.localPosition.z);
        }
    }

    [Button("ForceChange")]
    public void ForceChangeNumber()
    {
        ChangeValue(value);
    }
}
