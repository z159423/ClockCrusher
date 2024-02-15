using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    enum Images
    {
        BackGround,
    }
    enum GameObjects
    {
        Jerry,
    }
    private void Awake()
    {
        Bind<UnityEngine.UI.Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        base.Init();

    }
    private void JerryFighting()
    {
        Debug.Log("Test");
    }
}
