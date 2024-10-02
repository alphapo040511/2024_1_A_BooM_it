using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;       //아이템 이미지 등으로 변경
    public ItemData itemData;
    public int mountIndex = 0;
    public bool isMounted = false;

    public ItemType type;

    private InventorySystem inventorySystem;

    // Start is called before the first frame update
    void Start()
    {
        inventorySystem = InventorySystem.instance;
        UpdateImage();
    }

    public void UpdateImage()
    {
        if (itemData != null)
        {
            nameText.text = itemData.itemName;
        }
        else
        {
            nameText.text = "Null";
        }
    }

    public void ClickMoutedButton()
    {
        inventorySystem.ItemListUIActive(type);
    }

    public void ClickListButton()
    {
        inventorySystem.SelectItem(this);
    }
}
