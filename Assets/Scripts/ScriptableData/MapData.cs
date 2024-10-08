using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "NewLevel", menuName = "CustomData/LevelData")]       //생성 파일 메뉴에 추가 시켜준다.
public class MapData : ScriptableObject
{
    public BlockIndex BlockIndexData;
    public int[,,] BlockArr = new int[,,] { };
    public Vector3Int[] SpawnPosition = new Vector3Int[] { };
    private int posLength = 1;
    private int[] xPos = new int[] { };
    private int[] zPos = new int[] { };
    public TextAsset mapJsonFile;

#if UNITY_EDITOR
    public void SaveToJson()
    {
        if(mapJsonFile == null)
        {
            Debug.Log("맵 파일이 없습니다.");
            return;
        }

        posLength = SpawnPosition.Length;
        xPos = new int[posLength];
        zPos = new int[posLength];

        for (int i = 0; i < posLength; i++)
        {
            xPos[i] = SpawnPosition[i].x;
            zPos[i] = SpawnPosition[i].z;
        }

        var data = JsonConvert.SerializeObject(new
        {
            xPos,
            zPos,
            BlockArr,
        }, Formatting.Indented);        //JSON 변환 빛 파일 포맷 설정

        System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(mapJsonFile), data);       //파일에 JSON을 쓴다.
        AssetDatabase.Refresh();                                                            //완료후 리프레시    
    }
#endif
    public void LoadFormJson()
    {
        if(mapJsonFile == null)
        {
            Debug.Log("맵 파일이 없습니다.");
            return;
        }

        var data = JsonConvert.DeserializeAnonymousType(mapJsonFile.text, new
        {
            xPos = new int[] { },
            zPos = new int[] { },
            BlockArr = new int[,,] { },
        });
        
        BlockArr = data.BlockArr;

        posLength = xPos.Length;

        SpawnPosition = new Vector3Int[posLength];

        for (int i = 0; i < posLength; i++)
        {
            SpawnPosition[i] = new Vector3Int(xPos[i], 0 , zPos[i]);
        }
    }


}
