using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonType
{ 
    Fire,
    Aiming,
    Jump,
    Skill
}



public class MobileTuouchInput : MonoBehaviour
{
    public static MobileTuouchInput Instance;

    private void Awake()
    {
        Instance = this;
    }

    public FloatingJoystick moveJoystick;
    public FloatingJoystick cameraJoystick;

    private int wheel;
    public int Wheel
    {
        get
        {
            if(wheel == 0)
            {
                wheel = 0;
                return 0;
            }
            else if(wheel > 0)
            {
                wheel = 0;
                return 1;
            }
            else
            {
                wheel = 0;
                return -1;
            }
        }
    }

    private bool fire;
    public bool Fire 
    {   
        get 
        { 
            if(fire)
            {
                fire = false;
                return true;
            }
            return false;
        }
    }

    private bool aiming;
    public bool Aiming
    {
        get
        {
            if (aiming)
            {
                aiming = false;
                return true;
            }
            return false;
        }
    }

    private bool jump;
    public bool Jump
    {
        get
        {
            if (jump)
            {
                jump = false;
                return true;
            }
            return false;
        }
    }

    private bool skill;
    public bool Skill
    {
        get
        {
            if (skill)
            {
                skill = false;
                return true;
            }
            return false;
        }
    }


    public void ButtonDown(int type)
    {
        switch((ButtonType)type)
        {
            case ButtonType.Fire:
                fire = true;
                break;
            case ButtonType.Aiming:
                aiming = true;
                break;
            case ButtonType.Jump:
                jump = true; 
                break;
            case ButtonType.Skill: 
                skill = true;
                break;
        }
    }

    public void ChangeWeapon(bool add)
    {
        wheel = add ? 1 : -1;
    }
}
