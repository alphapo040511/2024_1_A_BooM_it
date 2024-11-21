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
        selectImage.enabled = false;

        button.GetComponent<Button>().onClick.AddListener (()=> addEvent(itemData.itemName));
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener ((evenetData) => infoEvent(itemData.itemName));
        button.GetComponent<EventTrigger>().triggers.Add(entry);

        selectImages.Add(itemData.itemName, selectImage);
        Debug.Log(itemData.itemName + selectImage);
    }

    public void CheckSelected(string name, bool isSelected)
    {
        if (selectImages.ContainsKey(name))
        {
            selectImages[name].enabled = isSelected;
        }
    }

    public void ShowDescription(Item data)
    {
        Debug.Log($"이름 : {data.itemName} 설명 : {data.Description}");
    }
}
