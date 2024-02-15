using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public enum pointerType
{
    type1,
    type2
}

public enum clockMatType
{
    type1,
    type2
}

public enum MergeType
{
    type1,
    type2
}

public enum BackgroundType
{
    def,
    magenta
}
public class ITRManager : MonoBehaviour
{

    public static ITRManager instance;

    public pointerType pointerType;
    public clockMatType clockMatType;
    public MergeType mergeType;
    public BackgroundType backgroundType;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);


    }

    public void SelectITRType(int num)
    {
        switch (num)
        {
            case 1:
                ChangePointerType(pointerType.type2);
                break;

            case 2:
                ChangeClockMatType(clockMatType.type2);
                break;

            case 3:
                ChangeBackgroundType(BackgroundType.magenta);
                break;

            case 4:
                ChangePointerType(pointerType.type2);
                break;

            case 5:
                ChangeMergeType(MergeType.type2);
                break;

            case 6:

                break;

            case 7:

                break;
            case 8:

                break;
        }

        SceneManager.LoadScene(1);

        this.TaskWaitUntil(() => Tutorial.tutorialEnable = false, () => Tutorial.insatnce != null);
    }

    public void ChangePointerType(pointerType type)
    {
        pointerType = type;
    }

    public void ChangeClockMatType(clockMatType type)
    {
        clockMatType = type;
    }

    public void ChangeMergeType(MergeType type)
    {
        mergeType = type;
    }

    public void ChangeBackgroundType(BackgroundType type)
    {
        backgroundType = type;
    }


}
