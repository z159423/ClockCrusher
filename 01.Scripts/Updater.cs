using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IUpdater
{
    public abstract void FrameUpdate();
}
public class Updater : MonoBehaviour
{
    #region singleTone
    private static Updater _instance = null;
    public static Updater Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(Updater)) as Updater;
                if (_instance == null)
                {

                }
            }
            return _instance;
        }
    }
    #endregion
    public List<IUpdater> updaters;
    public void AddUpdater(IUpdater updater)
    {
        updaters.Add(updater);
    }
    public void RemoveUpdater(IUpdater updater)
    {
        updaters.Remove(updater);
    }
    private void Awake()
    {
        updaters = new List<IUpdater>();
    }
    
    
    private void Update()
    {
        for (int i = 0; i < updaters.Count; i++)
        {
            updaters[i].FrameUpdate();
        }
    }
}







