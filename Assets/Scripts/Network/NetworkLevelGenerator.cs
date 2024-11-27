using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class NetworkLevelGenerator : MonoBehaviour
{
    public MapData mapData;

    public Dictionary<Vector3Int, NetworkBlock> blockDictionary = new Dictionary<Vector3Int, NetworkBlock>();

    private bool isFirstGenerate = false;

    public void MapDataLoad(string index)
    {
        //???? ???????? ???? ?????? ???????????? ????
        string path = Path.Combine("CompletedData", index);
        mapData = Resources.Load<MapData>(path);
        mapData.LoadFormJson();
        StartCoroutine(GenerateInitialBlocks());
    }


    //?? ???? ????
    public void MapLoading(string index)
    {
        if (!isFirstGenerate)
        {
            MapDataLoad(index);
        }
        else
        {
            StartCoroutine(MapRespawn());
        }
    }


    private IEnumerator GenerateInitialBlocks()
    {
        // 3?? for?????? ?? ?????? ???????? ???????? ???? ????
        for (int x = 0; x < mapData.BlockArr.GetLength(0); x++)
        {
            for (int y = 0; y < mapData.BlockArr.GetLength(1); y++)
            {
                for (int z = 0; z < mapData.BlockArr.GetLength(2); z++)
                {
                    if (mapData.BlockArr[x, y, z] != 0)
                    {
                        Vector3Int position = new Vector3Int(x, y, z);
                        bool temp = CheckBelowBlock(x, y, z);
                        CreateBlock(position, mapData.BlockArr[x, y, z] - 1, temp);
                    }
                }
            }
            yield return null;
        }
        UpdateSurfaceBlocks(); // ???? ???? ???? ?? ???? ?????? ??????
        isFirstGenerate = true;
        MapLoadComplete();
    }

    private IEnumerator MapRespawn()
    {
        int count = 0;
        foreach (var block in blockDictionary.Values)
        {
            block.Respawn();
            count++;
            if(count >= 50)
            {
                count = 0;
                yield return null;
            }
        }
        MapLoadComplete();
    }

    private void MapLoadComplete()
    {
        BattleManager.Instance.MapLoadDone();
    }

    //???? ?????? ???? ???? ???? ???????? ????
    private bool CheckBelowBlock(int x, int y, int z)
    {
        if (y == 0) return true;        //???? ?????? ?????? 0 ????

        bool isBelowBlock = true;

        for (int i = y - 1; i >= 0; i--)    //???? ?????? ???? ???????? ???? ????
        {
            if(mapData.BlockArr[x, i, z] != 0)
            {
                isBelowBlock = false;
            }
        }

        for(int X = -1; X <= 1; X++)
        {
            for(int Z = -1; Z <= 1; Z++)
            {
                if (mapData.BlockArr[Mathf.Clamp(X + x, 0, mapData.BlockArr.GetLength(0) - 1), y - 1, Mathf.Clamp(Z + z, 0, mapData.BlockArr.GetLength(2) - 1)] != 0)
                {
                    isBelowBlock = false;
                }
            }
        }

        return isBelowBlock;
    }

    private void CreateBlock(Vector3Int position, int blockValue, bool isBool)
    {
        GameObject blockInstance = Instantiate(SelectBlockPrefab(blockValue), position, Quaternion.identity, transform);
        NetworkBlock data = blockInstance.GetComponent<NetworkBlock>();
        data.Initialized(position, isBool, BattleManager.Instance.levelManager);
        blockDictionary[position] = data;
        blockInstance.SetActive(false);
    }

    private GameObject SelectBlockPrefab(int value)     //?????? ?????? ???????? ???? ?????? ???????? ?????????? ????
    {
        if (value >= mapData.BlockIndexData.Blocks.Count || value < 0)
        {
            return mapData.BlockIndexData.Blocks[0];    //???? ???? ???????? 0?? ???????? ?????? ????
        }

        return mapData.BlockIndexData.Blocks[value];
    }

    public void UpdateSurfaceBlocksAround(Vector3Int position)      // ?????? ???? ???? 3x3x3 ?????? ?????? ???? ????????
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    Vector3Int neighborPos = position + new Vector3Int(x, y, z);
                    if (blockDictionary.ContainsKey(neighborPos))
                    {
                        UpdateBlockSurfaceState(neighborPos);
                    }
                }
            }
        }
    }

    void UpdateSurfaceBlocks()
    {
        foreach (var kvp in blockDictionary)
        {
            UpdateBlockSurfaceState(kvp.Key); // ???? ?????? ???? ???? ????????
        }
    }

    void UpdateBlockSurfaceState(Vector3Int position)
    {
        bool isOnSurface = IsBlockOnSurface(position);
        NetworkBlock block = blockDictionary[position];
        if (block.IsDestroyed) return;
        block.gameObject.SetActive(isOnSurface); // ?????? ???? ?????? ??????
    }

    bool IsBlockOnSurface(Vector3Int position)
    {
        // 6????(??,??,??,??,??,??)?? ???? ???? ????
        // ?????? ???? ?????? ???????? ???????? ????
        Vector3Int[] neighbors = new Vector3Int[]
        {
            new Vector3Int(-1, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(0, 1, -1),
            new Vector3Int(0, 1, 1),
            new Vector3Int(-1, 1, 1),
            new Vector3Int(1, 1, -1),
            new Vector3Int(-1, 1, -1),
            new Vector3Int(1, 1, 1),
            //?? ?????? ???? ????
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        foreach (Vector3Int offset in neighbors)
        {
            Vector3Int neighborPos = position + offset;
            if (!blockDictionary.ContainsKey(neighborPos))
            {
                return true; // ???????? ???? ?????? ?????? ?????? ???? ??
            }
            else if (blockDictionary[neighborPos].IsDestroyed)
            {
                return true;
            }
        }

        return false; // ???? ?????? ?????? ???? ????
    }
}
