using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

// NetworkInputData ����ü ����
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

// NetworkInputHandler Ŭ����
public class NetworkInputHandler : MonoBehaviour
{
    //ī�޶� ���� ����
    [Header("Camera Settings")]
    public float mouseSensitivity = 2.0f;       //���콺 ����

    public float yMinLimit = -75;               //ī�޶� ���� ȸ�� �ּҰ�
    public float yMaxLimit = 75;                //ī�޶� ���� ȸ�� �ִ밢

    private float theta = 0.0f;                 //ī�޶��� ���� ȸ�� ����
    private float phi = 0.0f;                   //ī�޶��� ���� ȸ�� ����
    private float targetVecticalRotation = 0;   //��ǥ ���� ȸ�� ����
    //private float verticalRotationSpeed = 240f; //���� ȸ�� ����

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
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;         //���콺 �¿� �Է�
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;         //���콺 ���� �Է�

        //���� ȸ��(theta ��)
        theta += mouseX;                            //���콺 �Է°� �߰�
        theta = Mathf.Repeat(theta, 360.0f);        //���� ���� 360�� ���� �ʵ��� ����

        //���� ȸ�� ó��
        targetVecticalRotation -= mouseY;
        //targetVecticalRotation = Mathf.Clamp(targetVecticalRotation, yMinLimit, yMaxLimit);     //���� ȸ�� ����
        phi = targetVecticalRotation;       //�ӵ��� �����ϰ� ���ִ� �κ��ε� �ϴ� ����
        //phi = Mathf.MoveTowards(phi, targetVecticalRotation, verticalRotationSpeed * Time.DeltaTime);

        return new Vector3(phi, theta);
    }
}