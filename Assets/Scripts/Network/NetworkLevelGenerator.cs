using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLevelGenerator : MonoBehaviour
{
    public MapData mapData;

    public NetworkLevelManager levelManager;

    public Dictionary<Vector3Int, NetworkBlock> blockDictionary = new Dictionary<Vector3Int, NetworkBlock>();


    void Start()
    {
        mapData = GameManager.instance.mapData;
        mapData.LoadFormJson();
        GenerateInitialBlocks(); // 초기 블록 생성
    }



    void GenerateInitialBlocks()
    {
        // 3중 for문으로 맵 크기에 해당하는 그리드에 블록 생성
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
        }
        UpdateSurfaceBlocks(); // 모든 블록 생성 후 표면 블록만 활성화
    }

    //해당 블럭이 가장 아래 있는 블럭인지 확인
    private bool CheckBelowBlock(int x, int y, int z)
    {
        bool isBelowBlock = false;
        if (y <= 0)        //해당 블럭의 높이가 0 아래이거나 해당 블럭 아래에 다른 블럭이 없을경우
        {
            isBelowBlock = true;
        }
        else if(mapData.BlockArr[x, y - 1, z] == 0)
        {
            isBelowBlock = true;
        }
        else
        {
            isBelowBlock = false;
        }

        return isBelowBlock;
    }

    private void CreateBlock(Vector3Int position, int blockValue, bool isBool)
    {
        GameObject blockInstance = Instantiate(SelectBlockPrefab(blockValue), position, Quaternion.identity, transform);
        NetworkBlock data = blockInstance.GetComponent<NetworkBlock>();
        data.Initialized(position, isBool, levelManager);
        blockDictionary[position] = data;
        blockInstance.SetActive(false);
    }

    private GameObject SelectBlockPrefab(int value)     //설치할 블럭의 인덱스가 전체 블럭의 인덱스를 오버하는지 체크
    {
        if (value >= mapData.BlockIndexData.Blocks.Count || value < 0)
        {
            return mapData.BlockIndexData.Blocks[0];    //오버 또는 언더라면 0번 인덱스의 블럭을 생성
        }

        return mapData.BlockIndexData.Blocks[value];
    }

    public void UpdateSurfaceBlocksAround(Vector3Int position)      // 변경된 블록 주변 3x3x3 영역의 블록들 상태 업데이트
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
            UpdateBlockSurfaceState(kvp.Key); // 모든 블록의 표면 상태 업데이트
        }
    }

    void UpdateBlockSurfaceState(Vector3Int position)
    {
        bool isOnSurface = IsBlockOnSurface(position);
        NetworkBlock block = blockDictionary[position];
        if (block.IsDestroyed) return;
        block.gameObject.SetActive(isOnSurface); // 표면에 있는 블록만 활성화
    }

    bool IsBlockOnSurface(Vector3Int position)
    {
        // 6방향(상,하,좌,우,앞,뒤)의 이웃 블록 확인
        Vector3Int[] neighbors = new Vector3Int[]
        {
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
                return true; // 하나라도 이웃 블록이 없으면 표면에 있는 것
            }
            else if (blockDictionary[neighborPos].IsDestroyed)
            {
                return true;
            }
        }

        return false; // 모든 이웃이 있으면 내부 블록
    }
}
