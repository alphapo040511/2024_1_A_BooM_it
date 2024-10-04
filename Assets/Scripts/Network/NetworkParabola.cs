using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkParabola : NetworkBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField]  private float gravity = 9.81f;
    private Vector3 stargPosition;
    private Vector3 initialVelocity;
    private float currentTime = 0;

    public void Init(float angle)
    {
        stargPosition = transform.position;
        float initialX = speed * Mathf.Cos(angle * Mathf.Rad2Deg);
        float initialY = speed * Mathf.Sin(angle * Mathf.Rad2Deg);
        initialVelocity = transform.forward * initialX + transform.up * initialY;
    }

    public override void FixedUpdateNetwork()
    {
        currentTime += Runner.DeltaTime;
        Vector3 currentPosition = stargPosition + initialVelocity * Time.deltaTime;
        currentPosition.y -= 0.5f * gravity * currentTime * currentTime;
        transform.position = currentPosition;
        CheckCollision();
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
