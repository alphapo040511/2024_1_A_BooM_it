using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//ItemData에 대한 정보를 저장할 스크립터블 오브젝트, (폭탄, 아이템 두종류 모두 저장 가능)
[CreateAssetMenu(fileName = "NewItemIndex", menuName = "CustomData/ItemIndex")]
public class ItemIndex : ScriptableObject
{
    public List<ItemData> itemList = new List<ItemData> { };
    public ItemType dataType;
}

public enum ItemType
{
    Bomb,
    Item
}