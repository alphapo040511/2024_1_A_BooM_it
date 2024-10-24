using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelData : MonoBehaviour
{
    public MapData mapData;

    public List<BlockData> DestroyedBlocks = new List<BlockData>();
    public int[,,] BlockArr = new int[,,] { };
    public Vector3Int[] SpawnPos = new Vector3Int[] { };
    public Dictionary<Vector3Int, int> BlockDataDic = new Dictionary<Vector3Int, int> { };


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < DestroyedBlocks.Count; i++)
        {
            DestroyedBlocks[i].Timer(Time.deltaTime);
        }
    }

#if UNITY_EDITOR
    public void SaveLevelData(int[,,] map, Dictionary<Vector3Int, int> mapDic, Vector3Int[] spawnPos)
    {
        mapData.BlockArr = map;
        mapData.SpawnPosition = spawnPos;
        //mapData.BlockDataDictionary = mapDic;
        mapData.SaveToJson();
    }


    public void LoadLevelData()
    {
        mapData.LoadFormJson();
        BlockArr = mapData.BlockArr;
        SpawnPos = mapData.SpawnPosition;
        //BlockDataDic = mapData.BlockDataDictionary;

        ResetMap(BlockArr.GetLength(0), BlockArr.GetLength(1), BlockArr.GetLength(2));
        for (int x = 0; x < BlockArr.GetLength(0); x++)
        {
            for(int y = 0; y < BlockArr.GetLength(1); y++)
            {
                for(int z = 0;  z < BlockArr.GetLength(2); z++)
                {
                    if (BlockArr[x, y, z] != 0)
                    {
                        GameObject temp = Instantiate(mapData.BlockIndexData.Blocks[CheckBlockIndex(BlockArr[x, y, z] - 1)], new Vector3(x, y, z), Quaternion.identity);
                        temp.transform.parent = this.transform;
                        BlockData data = temp.GetComponent<BlockData>();
                    }
                }
            }
        }
    }
#endif

    private int CheckBlockIndex(int value)
    {
        if (mapData.BlockIndexData.Blocks.Count <= value)
        {
            return 0;
        }
        else
        {
            return value;
        }
    }

    public void DestroyBlock(BoomType boom, Vector3 Pos)
    {
        switch(boom)
        {
            case BoomType.Default:
                DefaultBoom(Pos);
                break;
            case BoomType.Cross:
                CrossBoom(Pos);
                break;
            case BoomType.Long:
                LongBoom(Pos);
                break;
            case BoomType.KnockBack:
                KnockBackBoom(Pos);
                break;
        }
    }

    private void DefaultBoom(Vector3 Pos)
    {

    }

    private void CrossBoom(Vector3 Pos)
    {

    }

    private void LongBoom(Vector3 Pos)
    {

    }

    private void KnockBackBoom(Vector3 Pos) 
    { 
        //플레이어가 특정 거리 안쪽에 있으면 밀쳐냄
    }

    public void ResetMap(int x, int y, int z)
    {
        DestroyedBlocks = new List<BlockData> { };
        mapData.BlockArr = new int[x, y, z];
        for (; transform.childCount > 0;)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        //정렬할때 쓰는거
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    BlockData temp = transform.GetChild(i).GetComponent<BlockData>();
        //    temp.ResetPosition();
        //    Blocks.Add(temp);
        //}
    }

}
