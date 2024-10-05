using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCameraFollow : MonoBehaviour
{
    public Transform playerTransform;           //?????? ?????? ???????? ???? ???????? ????????

    public Transform target;                    //???????? ?????? ????
    public float distance = 1.0f;              //?????????????? ????
    public float mouseSensitivity = 100.0f;     //?????? ????
    public float scrollScsitivity = 2.0f;       //?????? ????
    public float minYAngle = 5.0f;              //???? ???? ????
    public float maxYAngle = 85.0f;             //???? ???? ????

    private float currentHorizontalAngle = 0.0f;            //???? ???? ????
    private float currentVerticalAngle = 0f;              //???? ???? ????

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetPlayerTransform(Transform player, Transform pivot)
    {
        playerTransform = player;
        target = pivot;
    }

    public void CameraRotate(float x, float y, float deltaTime)
    {
        if (target != null && playerTransform != null)
        {
            HandleInput(x, y, deltaTime);
            //UpdateCameraPosition();
            Vector3 targetRotation = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            playerTransform.forward = targetRotation;
        }
    }

    private void HandleInput(float x, float y, float deltaTime)
    {
        //?????? ?????? ???? ???? ????
        currentHorizontalAngle -= x * mouseSensitivity * deltaTime;
        currentVerticalAngle += y * mouseSensitivity * deltaTime;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minYAngle, maxYAngle);         //???? ?????????? ???? ?????? ??????.

        //???????? ???? ???? ????
        distance += -Input.GetAxis("Mouse ScrollWheel") * scrollScsitivity;
        distance = Mathf.Clamp(distance, 0.5f, 2.0f);                             //???? ????
    }

    public void UpdateCameraPosition()
    {
        //???? ???????? ?????? ???? ????
        float verticalAngleRadians = currentVerticalAngle * Mathf.Deg2Rad;                  //?????? ?????????? ????
        float horizontalAngleRaians = currentHorizontalAngle * Mathf.Deg2Rad;

        float x = distance * Mathf.Sin(verticalAngleRadians) * Mathf.Cos(horizontalAngleRaians);
        float z = distance * Mathf.Sin(verticalAngleRadians) * Mathf.Sin(horizontalAngleRaians);
        float y = distance * Mathf.Cos(verticalAngleRadians);

        Vector3 newPosition = new Vector3(x, y, z);
        newPosition = target.position + newPosition;

        //???? ???????? ?? ?????? ????
        transform.position = newPosition;// Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 6);     //???? ???? ????
        transform.LookAt(target);
    }
}