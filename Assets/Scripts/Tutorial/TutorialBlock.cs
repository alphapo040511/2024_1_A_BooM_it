using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TutorialBlock : MonoBehaviour
{
    public bool IsDestroyed;
    public bool Unbreakable;
    public ParticleSystem explosionEffect;

    public Vector3Int intPosition;

    private BoxCollider blockCollider;
    private MeshRenderer meshRenderer;


    private void Awake()
    {
        blockCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Spawn(Vector3Int Pos)
    {
        intPosition = Pos;
    }

    public void DestroyBlock()     //블럭이 파괴됐을때 호출할 함수
    {
        if (IsDestroyed || Unbreakable) return;
        IsDestroyed = true;                         //블럭이 파괴된 것으로 변경
        blockCollider.enabled = false;
        meshRenderer.enabled = false;
        Effect();
    }

    public void Effect()
    {
        if (explosionEffect != null)
        {
            explosionEffect.Play();
        }
    }

    public void Respawn()
    {
        blockCollider.enabled = true;
        meshRenderer.enabled = true;
        IsDestroyed = false;
    }
}
