using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBlock : MonoBehaviour
{
    public bool IsDestroyed;
    public bool Regeneration;
    public ParticleSystem explosionEffect;

    public Vector3Int intPosition;
    private float respawnTime = 0;

    private NetworkLevelManager levelManager;

    private BoxCollider blockCollider;
    private MeshRenderer meshRenderer;

    public void DestroyBlock(float RespawnTime)     //���� �ı������� ȣ���� �Լ�
    {
        if (IsDestroyed) return;
        respawnTime = RespawnTime;                  //���� �� ������ �� ���� �ʿ��� �ð�
        IsDestroyed = true;                         //���� �ı��� ������ ����
        blockCollider.enabled = false;
        meshRenderer.enabled = false;
        Effect();
        if (Regeneration)
        {
            levelManager.DisabledBlocks.Add(this);
        }
    }

    public void Effect()
    {
        explosionEffect.Play();
    }

    public void Timer(float deltaTime)
    {
        if (IsDestroyed == false || Regeneration == false) return;

        respawnTime -= deltaTime;

        if (respawnTime <= 0 && IsDestroyed)
        {
            levelManager.RegenerationBlocks(intPosition);
        }
    }

    public void Respawn()
    {
        blockCollider.enabled = true;
        meshRenderer.enabled = true;
        IsDestroyed = false;
        if (levelManager.DisabledBlocks.Contains(this))
        {
            levelManager.DisabledBlocks.Remove(this);
        }
    }

    public void Initialized(Vector3Int Pos, bool isBool, NetworkLevelManager levelManager)
    {
        IsDestroyed = false;
        intPosition = Pos;
        Regeneration = isBool;
        this.levelManager = levelManager;
        blockCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }
}
