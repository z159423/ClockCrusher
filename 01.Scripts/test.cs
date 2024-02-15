using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    public Texture2D cursorImg;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorImg, new Vector3(50, 50, 0), CursorMode.ForceSoftware);
    }
}
