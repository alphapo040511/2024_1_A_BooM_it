using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//아이템 정보를 저장할 스크립터블 오브젝트, (폭탄, 아이템 두종류 모두 저장 가능)
[CreateAssetMenu(fileName = "NewItem", menuName = "CustomData/ItemData")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public int itemIndex;
    public Sprite itemImage;
    public string itemName;
    public GameObject itemObject;       //추후 네트워크 오브젝트로 변경 가능성 있음
    public string itemInfomation;
}
