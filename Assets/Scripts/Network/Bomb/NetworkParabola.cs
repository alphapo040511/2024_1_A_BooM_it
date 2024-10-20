using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public abstract class NetworkParabola : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    [Networked] private Vector3 startPosition{ get; set; }
    [Networked] private Vector3 initialVelocity{ get; set; }
    [Networked] private float currentTime{ get; set; }

    public float speed = 8.0f;
    public float gravity = 9.81f;

    private protected int knockbackDistance = 0;

    private float adjustedMinAngle = -30f;     //��� �߻� ���� �ּ� �ִ밪
    private float adjustedMaxAngle = 80f;

    private float minAngle = -75;       //�Է� �߻� ���� ��, ���� ���Ŀ��� �˾ƺ��� ������ �����
    private float maxAngle = 75;

    public void Init(float angle, Vector3 position)
    {
        life = TickTimer.CreateFromSeconds(Runner, 15.0f);      //Ȥ�� �� ��Ȳ�� ����ؼ� �ð� üũ
        float newAngle = (((angle - minAngle) * (adjustedMaxAngle - adjustedMinAngle)) / (maxAngle - minAngle)) + adjustedMinAngle;
        startPosition = position;
        transform.position = startPosition;
        float initialX = speed * Mathf.Cos(newAngle * Mathf.Deg2Rad);
        float initialY = speed * Mathf.Sin(newAngle * Mathf.Deg2Rad);
        initialVelocity = transform.forward * initialX + transform.up * initialY;
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
            CheckCollision();                           //�ϴ� �� �����Ӹ��� Ȯ��
        }
    }

    public virtual void Movement()      //�̵� ���� �Լ�
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

    public virtual void Explosion(Vector3Int blockData)       //���� (�� �ı�) ���
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


    
}
