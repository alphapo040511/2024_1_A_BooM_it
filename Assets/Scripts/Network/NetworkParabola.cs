using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkParabola : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }

    [Networked] private Vector3 startPosition{ get; set; }
    [Networked] private Vector3 initialVelocity{ get; set; }
    [Networked] private float currentTime{ get; set; }


    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float adjustedMinAngle = -30f;     //출력 발사 각도 최소 최대값
    [SerializeField] private float adjustedMaxAngle = 75f;

    private float minAngle = -75;       //입력 발사 각도 값, 밑의 공식에서 알아보기 쉽도록 적어둠
    private float maxAngle = 75;

    //[Networked] private bool isReady { get; set; }

    public void Init(float angle, Vector3 position)
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        float newAngle = (((angle - minAngle) * (adjustedMaxAngle - adjustedMinAngle)) / (maxAngle - minAngle)) + adjustedMinAngle;
        Debug.Log(angle);
        startPosition = position;
        transform.position = startPosition;
        float initialX = speed * Mathf.Cos(newAngle * Mathf.Deg2Rad);
        float initialY = speed * Mathf.Sin(newAngle * Mathf.Deg2Rad);
        initialVelocity = transform.forward * initialX + transform.up * initialY;
        //isReady = true;
    }

    public override void FixedUpdateNetwork()
    {
        //if (!isReady) return;

        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            currentTime += Runner.DeltaTime;
            Vector3 currentPosition = startPosition + initialVelocity * currentTime;
            currentPosition.y -= 0.5f * gravity * currentTime * currentTime;
            transform.position = currentPosition;
            //CheckCollision();
        }
    }

    private void CheckCollision()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != this.gameObject)      //일단 자기 자신만 아니면 됨
            {
                Runner.Despawn(Object);
                break;
            }
        }
    }
}
