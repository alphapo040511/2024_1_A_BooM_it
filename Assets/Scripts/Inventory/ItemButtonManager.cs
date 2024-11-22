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

    private Dictionary<string, GameObject> selectImages = new Dictionary<string, GameObject>();

    public void AddButtons(Item itemData, bool isWeapon , Action<string> addEvent, Action<string> infoEvent)
    {
        GameObject button = Instantiate(itemButtons, isWeapon ? weaponGrid : itemGrid);
        Image itemImage = button.transform.Find("ItemImage").GetComponent<Image>();
        GameObject selectImage = button.transform.Find("SelectImage").gameObject;

        itemImage.sprite = itemData.itemImage;

        button.GetComponent<Button>().onClick.AddListener (()=> addEvent(itemData.itemName));
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener ((evenetData) => infoEvent(itemData.itemName));
        button.GetComponent<EventTrigger>().triggers.Add(entry);

        if (selectImage != null)
        {
            selectImage.GetComponent<Image>().enabled = false;
            selectImages.Add(itemData.itemName, selectImage);
        }
        else
        {
            Debug.Log(itemData.itemName + "의 이미지 못 찾아냄");
        }
    }

    public void CheckSelected(string name, bool isSelected)
    {
        if(name == null)
        {
            Debug.Log(isSelected);
        }

        if (selectImages.ContainsKey(name))
        {
            selectImages[name].GetComponent<Image>().enabled = isSelected;
        }
    }

    public void ShowDescription(Item data)
    {
        Debug.Log($"이름 : {data.itemName} 설명 : {data.Description}");
    }

}
