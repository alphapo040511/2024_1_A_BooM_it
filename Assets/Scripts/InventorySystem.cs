using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ItemButton[] bombButton = new ItemButton[3];
    public ItemButton itemButton;

    public Canvas bombListCanvas;
    public Canvas itemListCanvas;

    //나중에 프로퍼티 설정
    public int[] mountedBombIndex = new int[3];     //현재 장착중인 폭탄의 인덱스
    public int mountedItemIndex;                    //현재 장착중인 아이템의 인덱스 

    [SerializeField]private ItemIndex bombIndex;    //폭탄 데이터 인덱스
    [SerializeField]private ItemIndex ItemIndex;    //아이템 데이터 인덱스

    private ItemType nowSelectType;                 //현재 선택할 아이템의 종류

    void Start()
    {
        Init();
    }

    //장착할 아이템(또는 폭탄)을 선택했을때
    public void SelectItem(ItemButton button)    //매개변수로 해당 버튼 클래스를 받기
    {
        ItemType type = button.itemData.itemType;
        int index = button.itemData.itemIndex;
        bool isMounted = button.isMounted;

        if(isMounted)
        {
            int mountIndex = button.mountIndex;
            DisableItem(type, button, mountIndex);
            return;
        }

        int targetIndex = EnptyIndex();
        if(targetIndex == -1)
        {
            Debug.Log("이미 3개의 폭탄이 장착 되어있습니다.");
        }
        else
        {
            SetItem(type, index, button, targetIndex);
        }
    }

    //아이템 장착 
    private void SetItem(ItemType type, int index, ItemButton button, int arrayIndex = 0)
    {
        if(type == ItemType.Bomb)
        {
            mountedBombIndex[arrayIndex] = index;
            bombButton[arrayIndex].itemData = button.itemData;
            button.mountIndex = arrayIndex;
        }
        else if (type == ItemType.Item)
        {
            mountedItemIndex = index;
            itemButton.itemData = button.itemData;
        }
        button.isMounted = true;
        UpdateButtons();
    }

    //아이템 해제
    private void DisableItem(ItemType type, ItemButton button, int arrayIndex = 0)
    {
        if (type == ItemType.Bomb)
        {
            mountedBombIndex[arrayIndex] = 0;
            bombButton[arrayIndex].itemData = null;
            button.mountIndex = 0;
        }
        else if (type == ItemType.Item)
        {
            mountedItemIndex = 0;
            itemButton.itemData = null;
        }
        button.isMounted = false;
        UpdateButtons();
    }

    private int EnptyIndex()
    {
        for(int i = 0; i < mountedBombIndex.Length; i++)
        {
            if(mountedBombIndex[i] == 0)
            {
                return i;
            }
        }

        return -1;
    }

    private void UpdateButtons()
    {
        foreach(ItemButton button in bombButton)
        {
            button.UpdateImage();
        }
        itemButton.UpdateImage();
    }

    //인덱스 초기화
    private void Init()
    {
        for (int i = 0; i < bombButton.Length; i++)
        {
            if (bombButton[i].itemData != null)
            {
                mountedBombIndex[i] = bombButton[i].itemData.itemIndex;
            }
        }
        mountedItemIndex = itemButton.itemData.itemIndex;
        UpdateButtons();
    }

    //아이템 리스트의 UI 활성화
    public void ItemListUIActive(ItemType type)
    {
        nowSelectType = ItemType.Item;

        AllCanvasTurnOff();
        if (type == ItemType.Bomb)
        {
            bombListCanvas.enabled = true;
        }
        else if(type == ItemType.Item)
        {
            itemListCanvas.enabled = true;
        }
    }

    //모든 캔버스 비활성화
    private void AllCanvasTurnOff()
    {
        bombListCanvas.enabled = false;
        itemListCanvas.enabled = false;
    }
}
