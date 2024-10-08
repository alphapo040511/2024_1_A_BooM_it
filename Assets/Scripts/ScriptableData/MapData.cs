using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "NewLevel", menuName = "CustomData/LevelData")]       //���� ���� �޴��� �߰� �����ش�.
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
            Debug.Log("�� ������ �����ϴ�.");
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
        }, Formatting.Indented);        //JSON ��ȯ �� ���� ���� ����

        System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(mapJsonFile), data);       //���Ͽ� JSON�� ����.
        AssetDatabase.Refresh();                                                            //�Ϸ��� ��������    
    }
#endif
    public void LoadFormJson()
    {
        if(mapJsonFile == null)
        {
            Debug.Log("�� ������ �����ϴ�.");
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
