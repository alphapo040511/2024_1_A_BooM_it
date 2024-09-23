using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BlockData : MonoBehaviour
{
    [Header("�ı���")]public bool IsDestroyed;                             //���� ���� �ı��� �������� ��Ÿ�� bool
    [Header("�� ������ ������ ��")] public bool Regeneration;            //���� ���� ������� �������� ��Ÿ�� bool

    public Vector3Int intPosition;
    public float respawnTime;                                             //�� �������� �ʿ��� �ð��� ������ float

    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = LevelManager.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DestroyBlock(float RespawnTime)     //���� �ı������� ȣ���� �Լ�
    {
        if (IsDestroyed) return;
        respawnTime = RespawnTime;                  //���� �� ������ �� ���� �ʿ��� �ð�
        IsDestroyed = true;                         //���� �ı��� ������ ����
    }

    public void Timer(float deltaTime)
    {
        if (IsDestroyed == false) return;

        respawnTime -= deltaTime;

        if(respawnTime <= 0 && IsDestroyed)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        this.gameObject.SetActive(true);
        IsDestroyed = false;
        levelManager.DisabledBlocks.Remove(this);
    }

    public void Initialized(Vector3Int Pos, bool isBool)
    {
        IsDestroyed = false;
        intPosition = Pos;
        Regeneration = isBool;
        respawnTime = 0;
    }

    public void ResetPosition()
    {
        //XPos = (int)Mathf.Round(transform.position.x);
        //YPos = (int)Mathf.Round(transform.position.y);
        //ZPos = (int)Mathf.Round(transform.position.z);
        //transform.position = new Vector3(XPos, YPos, ZPos);               //���� ��ġ�� �̵�
    }
}
