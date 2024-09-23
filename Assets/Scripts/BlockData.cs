using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BlockData : MonoBehaviour
{
    [Header("파괴됨")]public bool IsDestroyed;                             //현재 블럭이 파괴된 상태인지 나타낼 bool
    [Header("재 생성이 가능한 블럭")] public bool Regeneration;            //현재 블럭이 재생성이 가능한지 나타낼 bool

    public Vector3Int intPosition;
    public float respawnTime;                                             //재 생성까지 필요한 시간을 저장할 float

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

    public void DestroyBlock(float RespawnTime)     //블럭이 파괴됐을때 호출할 함수
    {
        if (IsDestroyed) return;
        respawnTime = RespawnTime;                  //블럭이 재 생성될 때 까지 필요한 시간
        IsDestroyed = true;                         //블럭이 파괴된 것으로 변경
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
        //transform.position = new Vector3(XPos, YPos, ZPos);               //보정 위치로 이동
    }
}
