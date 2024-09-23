using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BlockIndex", menuName = "CustomData/BlockIndexdata")]       //���� ���� �޴��� �߰� �����ش�.
public class BlockIndex : ScriptableObject
{
    public List<GameObject> Blocks;
}
