using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System.Linq;

public abstract class NetworkParabola : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [Networked] private Vector3 startPosition{ get; set; }
    [Networked] private Vector3 initialVelocity{ get; set; }
    [Networked] private float currentTime{ get; set; }

    public float speed = 16.0f;
    public float gravity = 9.81f;
    private int knockbackDistance = 3;

    private const float adjustedMinAngle = -40f;     //출력 발사 각도 최소 최대값
    private const float adjustedMaxAngle = 80;

    private const float minAngle = -75;       //입력 발사 각도 값, 밑의 공식에서 알아보기 쉽도록 적어둠
    private const float maxAngle = 75;

    public void Init(float angle, Vector3 firePoint, Quaternion cameraRotation)
    {
        float newAngle = ChangeAngle(angle);
        startPosition = firePoint;
        transform.position = startPosition;
        transform.rotation = cameraRotation;
        initialVelocity = VelocityCalculate(newAngle, firePoint);
    }

    private Vector3 VelocityCalculate(float angle, Vector3 position, Transform firePoint = null)
    {
        float initialX = speed * Mathf.Cos(angle * Mathf.Deg2Rad);
        float initialY = speed * Mathf.Sin(angle * Mathf.Deg2Rad);
        if(firePoint != null)
        {
            Vector3 forward = firePoint.forward;
            forward.y = 0;
            forward.Normalize();
            return forward * initialX + Vector3.up * initialY;
        }
        else
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            return forward * initialX + Vector3.up * initialY;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (transform.position.y < -3)
        {
            Runner.Despawn(Object);
        }
        else
        {
            Movement();
            CheckCollision();                           //일단 매 프레임마다 확인
        }
    }

    public virtual void Movement()      //이동 관련 함수
    {
        currentTime += Runner.DeltaTime;
        Vector3 currentPosition = startPosition + initialVelocity * currentTime;
        currentPosition.y -= 0.5f * gravity * currentTime * currentTime;
        transform.position = currentPosition;
    }

    private void CheckCollision()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.25f, 1 << 3);

        foreach (var hitCollider in hitColliders)
        {
            Vector3Int blockData = hitCollider.GetComponent<NetworkBlock>().intPosition;
            Explosion(blockData);
            KnockBack(knockbackDistance);
            Runner.Despawn(Object);
            break;
        }
    }

    public virtual void Explosion(Vector3Int blockData)       //폭발 (블럭 파괴) 기능
    {
        Vector3Int[] range = ExplosionRange();
        if (range != null)
        {
            BattleManager.Instance.levelManager.DestroyBlocks(blockData, range);
        }

        SoundManager.Instance.ExplosionSound();
    }

    public abstract Vector3Int[] ExplosionRange();



    public virtual void KnockBack(float distance, float force = 10)
    {
        float fixedForce = force;
        if(GameManager.instance.mapIndex.ToString().Split('_').Contains("Ice"))
        {
            fixedForce = force * 0.5f;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, distance, 1 << 6);

        foreach (var hitCollider in hitColliders)
        {
            Player player = hitCollider.GetComponent<Player>();
            player.Knockback(transform.position, fixedForce);
        }
    }

    public List<Vector3> Trajectory(float angle, Transform firePoint, Transform cameraDirection)
    {
        float newAngle = ChangeAngle(angle);
        Vector3 vel = VelocityCalculate(newAngle, firePoint.position, cameraDirection);

        List<Vector3> point = new List<Vector3>();
        for (int i = 0; i < 10; i++)
        {
            float targetTime = i * 0.2f;
            Vector3 currentPosition = firePoint.position + vel * targetTime;
            currentPosition.y -= 0.5f * gravity * targetTime * targetTime;
            point.Add(currentPosition);

            if (ArrivalPoint(currentPosition))
            {
                break;
            }
        }

        return point;
    }

    private bool ArrivalPoint(Vector3 Pos)
    {
        Collider[] hitColliders = Physics.OverlapSphere(Pos, 0.25f, 1 << 3);

        if (hitColliders.Length > 0)
        {
            return true;
        }

        return false;
    }


    private float ChangeAngle(float angle)
    {
        return (((angle - minAngle) * (adjustedMaxAngle - adjustedMinAngle)) / (maxAngle - minAngle)) + adjustedMinAngle;
    }


}
