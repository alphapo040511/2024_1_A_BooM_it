using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum TutorialType
{
    Goal,
    DestroyBlocks,
    KillDummy,
    UseSkill,
}

public enum DummyType
{
    Defalut,
    Resist,
    Fire,
}
public class TutorialMapGenerator : MonoBehaviour
{
    public TutorialType tutoType;
    public DummyType dummyType;
    public GameObject[] dummyPrefabs;

    public MapData levelData;
    public GameObject PlayerPrefab;
    private SinglePlayer player;
    private List<Dummy> dummys = new List<Dummy>();

    private Dictionary<Vector3Int, TutorialBlock> blocks = new Dictionary<Vector3Int, TutorialBlock>();

    public static TutorialMapGenerator Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadMapData();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            RespawnBlocks();
            TutorialManager.instance.ResetPoint();
            player.Respawn();
        }
    }

    public void LoadMapData()
    {
        DeleteBlock();
        levelData.LoadFormJson();
        StartCoroutine(MapGenerate());
    }

    public void DeleteBlock()
    {
        foreach(Transform oldBlock in transform)
        {
            Destroy(oldBlock.gameObject);
        }
    }

    private void PlayerSpawn()
    {
        GameObject temp = Instantiate(PlayerPrefab, levelData.SpawnPosition[0] + Vector3.up, quaternion.identity);
        temp.GetComponent<SinglePlayer>().SetSpawnPoint(levelData.SpawnPosition[0] + Vector3Int.up);
        player = temp.GetComponent<SinglePlayer>();
    }

    public IEnumerator MapGenerate()
    {
        int count = 0;

        for(int x = 0; x < levelData.BlockArr.GetLength(0); x++)
        {
            for(int y = 0; y < levelData.BlockArr.GetLength(1); y++)
            {
                for(int z = 0; z < levelData.BlockArr.GetLength(2); z++)
                {
                    if(levelData.BlockArr[x, y, z] != 0)
                    {
                        if(levelData.BlockArr[x, y, z] == 5)
                        {
                            GameObject dummy = Instantiate(dummyPrefabs[(int)dummyType], new Vector3(x, y + 1, z), Quaternion.Euler(0,180,0));
                            dummy.GetComponent<Dummy>().Spawn(new Vector3Int(x, y, z));
                            dummys.Add(dummy.GetComponent<Dummy>());
                        }
                        else
                        {
                            GameObject block = Instantiate(levelData.BlockIndexData.Blocks[levelData.BlockArr[x, y, z] - 1], new Vector3(x, y, z), Quaternion.identity, transform);
                            block.GetComponent<TutorialBlock>().Spawn(new Vector3Int(x, y, z));
                            blocks.Add(new Vector3Int(x, y, z), block.GetComponent<TutorialBlock>());
                        }

                        if(tutoType == TutorialType.DestroyBlocks && levelData.BlockArr[x, y, z] == 2)
                        {
                            count++;
                        }
                        else if(tutoType == TutorialType.KillDummy && levelData.BlockArr[x, y, z] == 5)
                        {
                            count++;
                        }
                    }
                }
            }
        }

        if (tutoType == TutorialType.UseSkill)
        {
            count = 5;
        }

        TutorialManager.instance.Init(tutoType, count);

        yield return null;

        PlayerSpawn();
        yield return null;
    }

    public void RespawnBlocks()
    {
        foreach(TutorialBlock block in blocks.Values)
        {
            block.Respawn();
        }

        foreach(Dummy temp in dummys)
        {
            temp.Respawn();
        }
    }

    public void DestroyBlocks(Vector3Int pos , Vector3Int[] range)
    {
        for (int y = pos.y; y < levelData.BlockArr.GetLength(1); y++)
        {
            foreach (Vector3Int dir in range)
            {
                Vector3Int targetPos = dir + new Vector3Int(pos.x, y, pos.z);
                if (blocks.ContainsKey(targetPos))
                {
                    blocks[targetPos].DestroyBlock();
                }
            }
        }
    }
}
