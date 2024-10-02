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

    public GameObject ButtonRef;

    public ItemButton[] bombButton = new ItemButton[3];
    public ItemButton itemButton;

    public Canvas bombListCanvas;
    public Canvas itemListCanvas;

    public GameObject bombLayout;
    public GameObject itemLayout;

    //나중에 프로퍼티 설정
    public int[] mountedBombIndex = new int[3];     //현재 장착중인 폭탄의 인덱스
    public int mountedItemIndex;                    //현재 장착중인 아이템의 인덱스 

    [SerializeField]private ItemIndex bombIndexData;    //폭탄 데이터 인덱스
    [SerializeField]private ItemIndex ItemIndexData;    //아이템 데이터 인덱스

    private GameManager gameManager;

    private List<ItemButton> bombList = new List<ItemButton> { };
    private List<ItemButton> itemList = new List<ItemButton> { };

    public ItemType nowSelectType;                 //현재 선택할 아이템의 종류

    void Start()
    {
        gameManager = GameManager.instance;
        Init();
    }

    //장착할 아이템(또는 폭탄)을 선택했을때
    public void SelectItem(ItemButton button)    //매개변수로 해당 버튼 클래스를 받기
    {
        ItemType type = button.type;
        int index = button.itemData.itemIndex;
        bool isMounted = button.isMounted;

        if(isMounted)
        {
            int mountIndex = button.mountIndex;
            DisableItem(type, button, mountIndex);
            return;
        }

        int targetIndex = EnptyIndex();
        if(targetIndex == -1 || mountedItemIndex != 0)
        {
            Debug.Log("이미 모든 아이템(폭탄)이 장착 되어있습니다.");
        }
        else
        {
            SetItem(type, index, button, targetIndex);
        }
    }

    //아이템 장착 
    private void SetItem(ItemType type, int index, ItemButton button, int arrayIndex = 0)
    {
        if (type != nowSelectType) return;

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
        UpdateInventoryButtons();
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
        UpdateInventoryButtons();
    }

    //착용중인 폭탄중에 앞에서부터 비어있는 칸이 있는지 확인
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

    //인덱스 초기화
    private void Init()
    {
        for (int i = 0; i < gameManager.bombIndex.Length; i++)
        {
            int temp = gameManager.bombIndex[i];
            if (temp != 0)
            {
                mountedBombIndex[i] = temp;
                bombButton[i].itemData = bombIndexData.itemList[temp - 1];
            }
        }

        int n = gameManager.itemIndex;
        if (n != 0)
        {
            mountedItemIndex = n;
            itemButton.itemData = ItemIndexData.itemList[n - 1];
        }
        LoadItemList();
        UpdateInventoryButtons();
        InitListButtons();
    }

    //지금 착용한 아이템 버튼의 이미지(정보) 업데이트
    private void UpdateInventoryButtons()
    {
        foreach(ItemButton button in bombButton)
        {
            button.UpdateImage();
        }
        itemButton.UpdateImage();
    }

    //처음 실행 시 리스트 아이템 버튼 데이터 초기화
    private void InitListButtons()
    {
        for (int i = 0; i < gameManager.bombIndex.Length; i++)
        {
            if (gameManager.bombIndex[i] != 0)
            {
                bombList[gameManager.bombIndex[i] - 1].isMounted = true;
                bombList[gameManager.bombIndex[i] - 1].mountIndex = i;
            }
        }
        if (gameManager.itemIndex != 0)
        {
            itemList[gameManager.itemIndex - 1].isMounted = true;
        }
    }

    //아이템 리스트 불러오기
    private void LoadItemList()
    {
        foreach(ItemData data in bombIndexData.itemList)
        {
            SetButton(data, ItemType.Bomb);
        }

        foreach (ItemData data in ItemIndexData.itemList)
        {
            SetButton(data, ItemType.Item);
        }
    }

    //아이템 추가하기
    private void SetButton(ItemData data, ItemType type)
    {
        GameObject button = Instantiate(ButtonRef, Vector3.zero, Quaternion.identity);
        ItemButton buttonData = button.GetComponent<ItemButton>();
        buttonData.Init(data, type);

        if (type == ItemType.Bomb)
        {
            button.transform.SetParent(bombLayout.transform, false);
            bombList.Add(buttonData);
        }
        else
        {
            button.transform.SetParent(itemLayout.transform, false);
            itemList.Add(buttonData);
        }
    }

    public void SaveItems()
    {
        gameManager.SaveItems(mountedBombIndex, mountedItemIndex);
    }

    //아이템 리스트의 UI 활성화
    public void ItemListUIActive(ItemType type)
    {
        nowSelectType = type;

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
    public void AllCanvasTurnOff()
    {
        bombListCanvas.enabled = false;
        itemListCanvas.enabled = false;
    }
}
