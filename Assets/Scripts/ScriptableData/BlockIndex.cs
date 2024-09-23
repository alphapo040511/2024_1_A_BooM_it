using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BlockIndex", menuName = "CustomData/BlockIndexdata")]       //생성 파일 메뉴에 추가 시켜준다.
public class BlockIndex : ScriptableObject
{
    public List<GameObject> Blocks;
}
