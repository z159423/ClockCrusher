using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SaveableObject : SerializedMonoBehaviour
{
    [HorizontalGroup("Guid")][ReadOnly][SerializeField] string guid;
    public string Guid { get => guid; protected set => guid = value; }

    [HorizontalGroup("Guid")]
    [Button(SdfIconType.ArrowClockwise)]
    virtual protected void NewGuid()
    {
        Guid = System.Guid.NewGuid().ToString();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public virtual void Save()
    {

    }
}
