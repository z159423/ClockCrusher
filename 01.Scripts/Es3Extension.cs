using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Es3Extension
{
    public static T LoadDataWhenItExist<T>(string key, T returnWhenItNotExist)
    {
        return ES3.KeyExists(key) ? ES3.Load<T>(key) : returnWhenItNotExist;
    }
}
