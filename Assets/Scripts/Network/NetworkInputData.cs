using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NetworkInputData 구조체 정의
public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 0x01;
    public const byte MOUSEBUTTON1 = 0x02;
    public NetworkButtons buttons;
    public Vector3 direction;
    public Vector2 lookDirection;
}

// NetworkInputHandler 클래스
public class NetworkInputHandler : MonoBehaviour
{
    public NetworkInputData GetNetworkInput()
    {
        var data = new NetworkInputData();

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        data.direction = new Vector3(vertical, 0, horizontal);

        //if (data.direction.magnitude > 1)
        //{
        //    data.direction.Normalize();
        //}

        data.lookDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, Input.GetMouseButton(0));
        data.buttons.Set(NetworkInputData.MOUSEBUTTON1, Input.GetMouseButton(1));

        return data;
    }
}