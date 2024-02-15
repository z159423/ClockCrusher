using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopperBtn : MonoBehaviour
{
    public Stopper stopper;

    public void OnClick()
    {
        stopper.Turn();
    }
}
