using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Search;
using System.Drawing;

#if UNITY_EDITOR
public class BlockPositionSet : EditorWindow
{
    private LevelData levelData;
    private BlockIndex blockIndex;
    private Vector3Int MapSize = new Vector3Int(16,3,16);
    private int NowHeight = 0;
    private int[,,] map = new int[16, 3, 16];
    private int playerCount = 2;
    private int nowPlayer = 1;
    private Vector3Int[] spawnPos = new Vector3Int[2];
    private Dictionary<Vector3Int, int> BlockDataDictionary = new Dictionary<Vector3Int, int> { };
    private int nowBlockIndex = 1;


    [MenuItem("Tool/LevelEditor")]
    private static void ShowWindow()
    {
        var winodw = GetWindow<BlockPositionSet>();
        winodw.titleContent = new GUIContent("LevelData");
        winodw.Show();
    }

    private void OnEnable()
    {
        levelData = FindAnyObjectByType<LevelData>();
        blockIndex = levelData.mapData.BlockIndexData;
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(640));                                //GUI ���̾ƿ� ���� ����

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("LevelData ����", GUILayout.Width(100), GUILayout.Height(50)))
        {
            levelData = FindAnyObjectByType<LevelData>();
            if (levelData == null)
            {
                GameObject level = new GameObject("LevelData");
                level.transform.position = Vector3.zero;
                level.AddComponent<LevelData>();
            }
            blockIndex = levelData.mapData.BlockIndexData;
        }

        GUILayout.Space(10);
        if (GUILayout.Button("������ �ҷ�����", GUILayout.Width(100), GUILayout.Height(50)))
        {
            levelData.LoadLevelData();
            map = levelData.BlockArr;
            spawnPos = levelData.SpawnPos;
            MapSize.x = map.GetLength(0);
            MapSize.y = map.GetLength(1);
            MapSize.z = map.GetLength(2);
            playerCount = spawnPos.Length;
        }

        GUILayout.Space(10);
        if (GUILayout.Button("�� ���� ä���", GUILayout.Width(100), GUILayout.Height(50)))
        {
            AllBlocksChange(true);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("�� ���� �����",GUILayout.Width(100), GUILayout.Height(50)))
        {
            AllBlocksChange(false);
        }

            GUILayout.Space(10);

        if (GUILayout.Button("�� ����", GUILayout.Width(100), GUILayout.Height(50)))
        {
            SetBlock();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("������ ����", GUILayout.Width(100), GUILayout.Height(50)))
        {
            SetBlock();
            levelData.SaveLevelData(map, BlockDataDictionary, spawnPos);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        MapSize.x = EditorGUILayout.IntField("�� ���� ũ��", MapSize.x);
        MapSize.z = EditorGUILayout.IntField("�� ���� ũ��", MapSize.z);
        MapSize.y = EditorGUILayout.IntField("�� �ִ� ����", MapSize.y);
        EditorGUILayout.EndHorizontal();

        if (MapSize.x <= 0) MapSize.x = 1;
        if(MapSize.z <= 0) MapSize.z = 1;

        if (MapSize.x != map.GetLength(0) || MapSize.y != map.GetLength(1) || MapSize.z != map.GetLength(2))
        {
            map = new int[MapSize.x, MapSize.y, MapSize.z];
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-")) nowBlockIndex--;
        nowBlockIndex = EditorGUILayout.IntField("��ġ�� �� �ε���", nowBlockIndex);
        if (GUILayout.Button("+")) nowBlockIndex++;
        if (nowBlockIndex > blockIndex.Blocks.Count) nowBlockIndex = blockIndex.Blocks.Count;
        if (nowBlockIndex < 1) nowBlockIndex = 1;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-")) NowHeight--;
            NowHeight = EditorGUILayout.IntField("���� Y��ǥ", NowHeight);
        if (GUILayout.Button("+")) NowHeight++;
        if (NowHeight >= MapSize.y) NowHeight = MapSize.y - 1;
        if (NowHeight < 0) NowHeight = 0;
        GUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        playerCount = EditorGUILayout.IntField("�� �÷��̾� ��", playerCount);
        GUILayout.Space(10);
        nowPlayer = EditorGUILayout.IntField("������ �÷��̾�", nowPlayer);
        GUILayout.Space(10);
        if (GUILayout.Button("��ġ Ȯ��")) CheckSpawnPosition();
        EditorGUILayout.EndHorizontal();

        if(spawnPos.Length != playerCount)
        spawnPos = new Vector3Int[playerCount];

        if (playerCount < 1) playerCount = 1;

        if (nowPlayer < 1) nowPlayer = 1;
        else if (nowPlayer > spawnPos.Length) nowPlayer = spawnPos.Length;

        EditorGUILayout.BeginHorizontal();

        for (int x = 0; x < MapSize.x; x++)
        {
            EditorGUILayout.BeginVertical();
            for (int z = MapSize.z - 1; z >= 0; z--)
            {
                DrawBlock(x,z);
                EditorGUILayout.Space(2f);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2f);
        }

        

        EditorGUILayout.EndHorizontal();

        


        EditorGUILayout.EndVertical();
    }

    private void SetBlock()
    {
        levelData.ResetMap(MapSize.x, MapSize.y, MapSize.z);

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int z = 0; z < MapSize.z; z++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    Vector3Int IntPos = new Vector3Int(x, y, z);
                    if (map[x, y, z] != 0)
                    {
                        if (BlockDataDictionary.ContainsKey(IntPos))
                        {
                            BlockDataDictionary[IntPos] = map[x, y, z];
                        }
                        else
                        {
                            BlockDataDictionary.Add(IntPos, map[x, y, z]);
                        }
                        var newBlock = Instantiate(blockIndex.Blocks[CheckBlockIndex(map[x, y, z] - 1)], new Vector3(x, y, z), Quaternion.identity);
                        newBlock.transform.parent = levelData.gameObject.transform;
                        BlockData temp = newBlock.GetComponent<BlockData>();
                    }
                    else
                    {
                        if (BlockDataDictionary.ContainsKey(IntPos))
                        {
                            BlockDataDictionary.Remove(IntPos);
                        }
                    }
                }
            }
        }
    }

    private int CheckBlockIndex(int value)
    {
        if(blockIndex.Blocks.Count <= value)
        {
            return 0;
        }
        else
        {
            return value;
        }
    }

    private void AllBlocksChange(bool Bool)
    {
        for (int x = 0; x < MapSize.x; x++)
        {
            for (int z = 0; z < MapSize.z; z++)
            {
                map[x,NowHeight,z] = Bool ? nowBlockIndex : 0;        //�ش� ���̸� ä�������� ����
                //for (int y = 0; y < YSize; y++)
                //{
                //    map[x, y, z] = Bool ? 1 : 0;
                //}
            }
        }
    }

    private void CheckSpawnPosition()
    {
        for(int i = 0; i < playerCount; i++)
        {
            Debug.Log($"{i + 1}��° �÷��̾��� ���� ��ġ {spawnPos[i]}");
        }
    }


    private void DrawBlock(int x, int z)
    {
        if (levelData == null || blockIndex == null) return;

        Rect rect = GUILayoutUtility.GetRect(640 / MapSize.x, 640 / MapSize.z);

        UnityEngine.Color blockColor = UnityEngine.Color.gray;
        switch(map[x, NowHeight, z])
        {
            case 0:
                blockColor = UnityEngine.Color.gray;
                break;
            case 1:
                blockColor = UnityEngine.Color.green;
                break;
            case 2:
                blockColor = UnityEngine.Color.yellow;
                break;
            case 3:
                blockColor = UnityEngine.Color.red;
                break;
            case 4:
                blockColor = UnityEngine.Color.blue;
                break;
            default:
                blockColor = UnityEngine.Color.white;
                break;
        }

       

        EditorGUI.DrawRect(rect, blockColor);

        if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) && rect.Contains(Event.current.mousePosition))
        {
            if (Event.current.button == 0)        //���콺 ���� Ŭ��
            {
                map[x, NowHeight, z] = nowBlockIndex;
            }
            else if (Event.current.button == 1)      //���콺 ������ Ŭ��
            {
                map[x, NowHeight, z] = 0;
            }
            else if (Event.current.button == 2)          //���콺 �� Ŭ��
            {
                spawnPos[nowPlayer - 1] = new Vector3Int(x, 0, z);
                Debug.Log($"{nowPlayer}��° �÷��̾��� ���� ��ġ�� {spawnPos[nowPlayer - 1]}�� �����Ǿ����ϴ�.");
            }
            Event.current.Use();
        }
    }
}
#endif