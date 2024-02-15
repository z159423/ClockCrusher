using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ClockNumForceChangeSender : MonoBehaviour
{
    public int num;
    public InputField field;
    public void ForceChange()
    {
        Cheat.instance.ForceChangeValue(num, field.text);
    }
}
