using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class BoomParabola : MonoBehaviour
{
    public BoomType type;


    public Vector3 startPosition;
    public Vector3 targetPosition;

    private Vector3 targetDistance;

    public float velocity = 30f;
    public float gravity = 9.82f;
    public float arriveTime = 3f;

    private Vector3 currentPosition;
    private Vector3 initialVelocity;

    private float currentTime = 0;

    private MeshRenderer meshRenderer;

    private bool isArrived = false;

    private Vector3Int targetIntPos;

    private LayerMask leveLayer;

    // Start is called before the first frame update
    void Start()
    {
        leveLayer = LayerMask.NameToLayer("Level");
    }

    // Update is called once per frame
    void Update()
    {
        ThrowBoom();
    }

    public void BoomInitialize(Vector3 pos, Vector3Int intPos, Transform FirePosition, BoomType boomType)
    {
        meshRenderer = GetComponent<MeshRenderer>();
        type = boomType;
        targetPosition = pos;
        targetIntPos = intPos;
        startPosition = FirePosition.position;
        float dis = Vector3.Distance(startPosition, targetPosition);
        arriveTime = dis / velocity;
        currentTime = 0;
        transform.position = startPosition;
        CalculateVelocity();
        meshRenderer.enabled = true;
    }

    private void CalculateVelocity()
    {
        targetDistance = targetPosition - startPosition;
        float x = targetDistance.x;
        float y = targetDistance.y;
        float z = targetDistance.z;

        initialVelocity.x = x / arriveTime;
        initialVelocity.z = z / arriveTime;

        //실제 이동 거리 = 기존 속도로 이동한 거리 - 중력가속도로 이동한 거리
        //기존 속도로 이동한 거리 = 실제 이동거리(y) + 중력가속도로 이동한 거리
        initialVelocity.y = (y + 0.5f * gravity * arriveTime * arriveTime) / arriveTime;
    }

    private void ThrowBoom()
    {
        if(transform.position.y <= -5)
        {
            Destroy(gameObject);
        }


        if (isArrived == false)
        {
            currentTime += Time.deltaTime;
            currentPosition = startPosition + initialVelocity * currentTime;
            currentPosition.y -= 0.5f * gravity * currentTime * currentTime;

            transform.position = currentPosition;
        }

        Collider[] target = Physics.OverlapSphere(transform.position, 0.1f, (1 << leveLayer));
        foreach (Collider C in target)
        {
            if (C != null)
            {
                targetIntPos = C.GetComponent<BlockData>().intPosition;
                isArrived = true;
                Explosion(targetIntPos);
                break;
            }
        }
    }

    private void Explosion(Vector3Int targetP)
    {
        LevelManager.instance.DestroyBlock(type, targetP);
        Destroy(gameObject);
    }


}
