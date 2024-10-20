using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapOcullusionCulling : MonoBehaviour
{
    public MapData mapData;         //맵 정보를 저장하는 데이터
    public LayerMask blockLayer; // 블록이 속한 레이어
    public Camera mainCamera; // 메인 카메라 참조

    public Dictionary<Vector3Int, GameObject> blockDictionary = new Dictionary<Vector3Int, GameObject>(); // 위치를 키로, 블록 게임오브젝트를 값으로 저장하는 딕셔너리

    private Plane[] cameraFrustumPlanes = new Plane[6]; // 카메라 프러스텀 평면 배열
    void Start()
    {
#if UNITY_EDITOR
        mapData.LoadFormJson();
#endif
        GenerateInitialBlocks(); // 초기 블록 생성
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // 메인 카메라가 설정되지 않았다면 자동으로 찾아 설정
        }
        //UpdateBlockVisibility(); // 초기 블록 가시성 업데이트
    }

    void Update()
    {
        //UpdateBlockVisibility(); // 매 프레임마다 블록 가시성 업데이트
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
                        bool temp = CheckBlockBelow(x, y, z);
                        CreateBlock(position, mapData.BlockArr[x, y, z] - 1, temp);
                    }
                }
            }
        }
        UpdateSurfaceBlocks(); // 모든 블록 생성 후 표면 블록만 활성화
    }

    void CreateBlock(Vector3Int position, int blockValue, bool isBool)
    {
        GameObject blockInstance = Instantiate(SelectBlockPrefab(blockValue), position, Quaternion.identity, transform);
        blockDictionary[position] = blockInstance;
        blockInstance.GetComponent<BlockData>().Initialized(position, isBool);
        blockInstance.SetActive(false); // 초기에는 모든 블록을 비활성화
    }

    private bool CheckBlockBelow(int x, int y, int z)
    {
        if(y <= 0 || mapData.BlockArr[x, y - 1, z] == 0)        //해당 블럭의 높이가 0 아래이거나 해당 블럭 아래에 다른 블럭이 없을경우
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private GameObject SelectBlockPrefab(int value)
    {
        if (value >= mapData.BlockIndexData.Blocks.Count || value < 0)
        {
            return null;
        }

        return mapData.BlockIndexData.Blocks[value];
    }

    void UpdateBlockVisibility()
    {
        GeometryUtility.CalculateFrustumPlanes(mainCamera, cameraFrustumPlanes); // 카메라 프러스텀 평면 계산

        foreach (var kvp in blockDictionary)
        {
            Vector3Int position = kvp.Key;
            GameObject block = kvp.Value;

            if (block.activeSelf)
            {
                bool isVisible = IsBlockVisible(position);
                block.GetComponent<Renderer>().enabled = isVisible; // 프러스텀 내에 있는 블록만 렌더러 활성화
                block.GetComponent<BoxCollider>().enabled = isVisible;
            }
        }
    }

    bool IsBlockVisible(Vector3Int position)
    {
        Vector3 blockCenter = position + new Vector3(0.5f, 0.5f, 0.5f);
        return GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, new Bounds(blockCenter, Vector3.one)); // 블록이 카메라 프러스텀 내에 있는지 확인
    }

    void UpdateSurfaceBlocks()
    {
        foreach (var kvp in blockDictionary)
        {
            UpdateBlockSurfaceState(kvp.Key); // 모든 블록의 표면 상태 업데이트
        }
    }

    public void UpdateSurfaceBlocksAround(Vector3Int position)
    {
        // 제거된 블록 주변 3x3x3 영역의 블록들 상태 업데이트
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

    void UpdateBlockSurfaceState(Vector3Int position)
    {
        bool isOnSurface = IsBlockOnSurface(position);
        GameObject block = blockDictionary[position];
        if (block.GetComponent<BlockData>().IsDestroyed) return;
        block.SetActive(isOnSurface); // 표면에 있는 블록만 활성화
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
            else if (blockDictionary[neighborPos].GetComponent<BlockData>().IsDestroyed)
            {
                return true;
            }
        }

        return false; // 모든 이웃이 있으면 내부 블록
    }
}
