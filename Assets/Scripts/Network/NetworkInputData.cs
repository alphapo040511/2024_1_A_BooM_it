using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

// NetworkInputData 구조체 정의
public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 0x01;
    public const byte MOUSEBUTTON1 = 0x02;
    public const byte KEYBOARDSPACE = 0x03;
    public NetworkButtons buttons;
    public Vector3 direction;
    public Vector2 lookDirection;
    public float wheel;
}

// NetworkInputHandler 클래스
public class NetworkInputHandler : MonoBehaviour
{
    //카메라 설정 변수
    [Header("Camera Settings")]
    public float mouseSensitivity = 2.0f;       //마우스 감도

    public float yMinLimit = -75;               //카메라 수직 회전 최소각
    public float yMaxLimit = 75;                //카메라 수직 회전 최대각

    private float theta = 0.0f;                 //카메라의 수평 회전 각도
    private float phi = 0.0f;                   //카메라의 수직 회전 각도
    private float targetVecticalRotation = 0;   //목표 수직 회전 각도
    //private float verticalRotationSpeed = 240f; //수직 회전 각도

    public NetworkInputData GetNetworkInput()
    {
        var data = new NetworkInputData();

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        data.direction = new Vector3(vertical, 0, horizontal);

        data.wheel = Input.GetAxis("Mouse ScrollWheel") * 2;


        data.lookDirection = HandleRotation();

        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, Input.GetMouseButton(0));
        data.buttons.Set(NetworkInputData.MOUSEBUTTON1, Input.GetMouseButton(1));
        data.buttons.Set(NetworkInputData.KEYBOARDSPACE, Input.GetKey(KeyCode.Space));

        return data;
    }

    Vector3 HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;         //마우스 좌우 입력
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;         //마우스 상하 입력

        //수평 회전(theta 값)
        theta += mouseX;                            //마우스 입력값 추가
        theta = Mathf.Repeat(theta, 360.0f);        //각도 값이 360을 넘지 않도록 조정

        //수직 회전 처리
        targetVecticalRotation -= mouseY;
        //targetVecticalRotation = Mathf.Clamp(targetVecticalRotation, yMinLimit, yMaxLimit);     //수직 회전 제한
        phi = targetVecticalRotation;       //속도를 일정하게 해주는 부분인데 일단 제외
        //phi = Mathf.MoveTowards(phi, targetVecticalRotation, verticalRotationSpeed * Time.DeltaTime);

        return new Vector3(phi, theta);
    }
}