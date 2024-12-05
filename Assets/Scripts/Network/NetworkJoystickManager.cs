using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonType
{
    Fire,
    Zoom,
    Jump,
    Skill
}

public class NetworkJoystickManager : MonoBehaviour
{
    public static NetworkJoystickManager Instance;

    public Vector2 moveDir;
    public Vector2 cameraDir;
    public bool fire;
    public bool zoom;
    public bool jump;
    public bool skill;
    public int wheel;

    public void ButtonDown(ButtonType type)
    {
        
    }

    public void ButtonUp(ButtonType type)
    {

    }
}
