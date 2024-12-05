using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

// NetworkInputData ?????? ????
public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 0x01;
    public const byte MOUSEBUTTON1 = 0x02;
    public const byte KEYCODESPACE = 0x03;
    public const byte KEYCODER= 0x04;
    public NetworkButtons buttons;
    public Vector3 direction;
    public Vector2 lookDirection;
    public float wheel;
}

// NetworkInputHandler ??????
public class NetworkInputHandler : MonoBehaviour
{
    public bool isMobile = false;

    //?????? ???? ????
    [Header("Camera Settings")]
    public float mouseSensitivity = 2.0f;       //?????? ????

    public float yMinLimit = -75;               //?????? ???? ???? ??????
    public float yMaxLimit = 75;                //?????? ???? ???? ??????

    private float theta = 0.0f;                 //???????? ???? ???? ????
    private float phi = 0.0f;                   //???????? ???? ???? ????
    private float targetVecticalRotation = 0;   //???? ???? ???? ????
    //private float verticalRotationSpeed = 240f; //???? ???? ????

    public NetworkInputData GetNetworkInput()
    {
        var data = new NetworkInputData();

        if (Cursor.lockState != CursorLockMode.Locked) return data;


        if (!isMobile)      //pcÀÎ °æ¿ì
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            data.direction = new Vector3(vertical, 0, horizontal);

            data.wheel = Input.GetAxis("Mouse ScrollWheel") * 2;


            data.lookDirection = HandleRotation();

            data.buttons.Set(NetworkInputData.MOUSEBUTTON0, Input.GetMouseButtonDown(0));
            data.buttons.Set(NetworkInputData.MOUSEBUTTON1, Input.GetMouseButtonDown(1));
            data.buttons.Set(NetworkInputData.KEYCODESPACE, Input.GetKey(KeyCode.Space));
            data.buttons.Set(NetworkInputData.KEYCODER, Input.GetKey(KeyCode.R));
        }
        else
        {

        }

        return data;
    }

    Vector3 HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;         //?????? ???? ????
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;         //?????? ???? ????

        //???? ????(theta ??)
        theta += mouseX;                            //?????? ?????? ????
        theta = Mathf.Repeat(theta, 360.0f);        //???? ???? 360?? ???? ?????? ????

        //???? ???? ????
        targetVecticalRotation -= mouseY;
        //targetVecticalRotation = Mathf.Clamp(targetVecticalRotation, yMinLimit, yMaxLimit);     //???? ???? ????
        phi = targetVecticalRotation;       //?????? ???????? ?????? ???????? ???? ????
        //phi = Mathf.MoveTowards(phi, targetVecticalRotation, verticalRotationSpeed * Time.DeltaTime);

        return new Vector3(phi, theta);
    }
}