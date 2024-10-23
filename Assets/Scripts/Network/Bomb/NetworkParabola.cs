using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public abstract class NetworkParabola : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [Networked] private Vector3 startPosition{ get; set; }
    [Networked] private Vector3 initialVelocity{ get; set; }
    [Networked] private float currentTime{ get; set; }

    public float speed = 8.0f;
    public float gravity = 9.81f;

    private protected int knockbackDistance = 3;

    private const float adjustedMinAngle = -30f;     //출력 발사 각도 최소 최대값
    private const float adjustedMaxAngle = 80f;

    private const float minAngle = -75;       //입력 발사 각도 값, 밑의 공식에서 알아보기 쉽도록 적어둠
    private const float maxAngle = 75;

    public void Init(float angle, Vector3 firePoint, Quaternion cameraRotation)
    {
        life = TickTimer.CreateFromSeconds(Runner, 15.0f);      //혹시 모를 상황에 대비해서 시간 체크
        float newAngle = (((angle - minAngle) * (adjustedMaxAngle - adjustedMinAngle)) / (maxAngle - minAngle)) + adjustedMinAngle;
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
            return firePoint.forward * initialX + firePoint.up * initialY;
        }
        else
        {
            return transform.forward * initialX + transform.up * initialY;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner) || transform.position.y < -3)
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
            KnockBack();
            Runner.Despawn(Object);
            break;
        }
    }

    public virtual void Explosion(Vector3Int blockData)       //폭발 (블럭 파괴) 기능
    {
        Vector3Int[] range = ExplosionRange();
        NetworkLevelManager.instance.DestroyBlocks(blockData, range);
    }

    public abstract Vector3Int[] ExplosionRange();



    public virtual void KnockBack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, knockbackDistance, 1 << 6);

        foreach (var hitCollider in hitColliders)
        {
            Player player = hitCollider.GetComponent<Player>();
            player.Knockback(transform.position);
        }
    }

    public Vector3[] Trajectory(float angle, Transform firePoint, Transform cameraDirection)
    {
        float newAngle = (((angle - minAngle) * (adjustedMaxAngle - adjustedMinAngle)) / (maxAngle - minAngle)) + adjustedMinAngle;
        Vector3 vel = VelocityCalculate(newAngle, firePoint.position, cameraDirection);

        Vector3[] point = new Vector3[10];
        for (int i = 0; i < 10; i++)
        {
            float targetTime = i * 0.2f;
            Vector3 currentPosition = firePoint.position + vel * targetTime;
            currentPosition.y -= 0.5f * gravity * targetTime * targetTime;
            point[i] = currentPosition;
        }

        return point;
    }


}
