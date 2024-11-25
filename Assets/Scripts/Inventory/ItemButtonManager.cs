using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButtonManager : MonoBehaviour
{
    public GameObject itemButtons;
    public Transform weaponGrid;
    public Transform itemGrid;

    public Image[] itemButtonImages = new Image[4];

    private Dictionary<string, Image> selectImages = new Dictionary<string, Image>();

    public void AddButtons(Item itemData, bool isWeapon , Action<string> addEvent, Action<string> infoEvent)
    {
        GameObject button = Instantiate(itemButtons, isWeapon ? weaponGrid : itemGrid);
        Image itemImage = button.transform.Find("ItemImage").GetComponent<Image>();
        Image selectImage = button.transform.Find("SelectImage").GetComponent<Image>();

        itemImage.sprite = itemData.itemImage;

        button.GetComponent<Button>().onClick.AddListener (()=> addEvent(itemData.itemIndex));
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener ((evenetData) => infoEvent(itemData.itemIndex));
        button.GetComponent<EventTrigger>().triggers.Add(entry);

        selectImage.enabled = false;
        selectImages.Add(itemData.itemIndex, selectImage);
    }

    public void CheckSelected(string name, bool isSelected)
    {
        if(name == null)
        {
            Debug.Log(isSelected);
        }

        if (selectImages.ContainsKey(name))
        {
            selectImages[name].enabled = isSelected;
        }
    }

    public void SetCurrentItemButton(int index, bool isSelected, Sprite itemImage = default)
    {
        itemButtonImages[index].enabled = isSelected;
        itemButtonImages[index].sprite = itemImage;
        Debug.Log($"{index} 칸에 {itemImage} 이미지 적용");
    }

    public void ShowDescription(Item data)
    {
        Debug.Log($"이름 : {data.itemName} 설명 : {data.Description}");
    }

}
