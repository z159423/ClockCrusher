using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (MainManager.instance.mainCamera != null)
            transform.LookAt(transform.position + MainManager.instance.mainCamera.transform.forward);
    }
}
