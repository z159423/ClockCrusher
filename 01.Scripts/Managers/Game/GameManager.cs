using UnityEngine;
public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public JoyStickController JoyStickController;
    public void SetDownAction(System.Action action)
    {
        JoyStickController?.AddDownEvent(action);
    }
    public void SetUpAction(System.Action action)
    {
        JoyStickController?.AddUpEvent(action);
    }
    public void SetMoveAction(System.Action<Vector2> action)
    {
        JoyStickController?.AddMoveEvent(action);
    }

    public void Init()
    {
    }
    public void Clear()
    {
        if (JoyStickController != null)
        {
            JoyStickController.DownAction = null;
            JoyStickController.UpAction = null;
            JoyStickController.JoystickMoveAction = null;
        }
    }
}
