using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMapGenerator : MonoBehaviour
{
    public List<MapData> levelData;

    // Start is called before the first frame update
    void Start()
    {
        LoadMapData(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMapData(int index)
    {
        DeleteBlock();
        MapData mapData = levelData[index];
        mapData.LoadFormJson();
        MapGenerate(mapData);
    }

    public void DeleteBlock()
    {
        foreach(Transform oldBlock in transform)
        {
            Destroy(oldBlock.gameObject);
        }
    }

    public void MapGenerate(MapData data)
    {
        for(int x = 0; x < data.BlockArr.GetLength(0); x++)
        {
            for(int y = 0; y < data.BlockArr.GetLength(1); y++)
            {
                for(int z = 0; z < data.BlockArr.GetLength(2); z++)
                {
                    if(data.BlockArr[x, y, z] != 0)
                    {
                        GameObject block = Instantiate(data.BlockIndexData.Blocks[data.BlockArr[x, y, z] - 1],new Vector3(x,y,z), Quaternion.identity, transform);
                        block.GetComponent<TutorialBlock>().Spawn(new Vector3Int(x,y,z));
                    }
                }
            }
        }
    }
}
