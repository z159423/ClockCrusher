using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStick", menuName = "Game/Stick")]
public class StickObjects : ScriptableObject
{
    public int level;
    public string _name;
    public Sprite icon;
    public Sprite antique_icon;
    public GameObject obj;
    public int rvWatchGoalCount;
}
