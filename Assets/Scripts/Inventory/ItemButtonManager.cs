using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonManager : MonoBehaviour
{
    public GameObject itemButtons;
    public Transform weaponGrid;
    public Transform itemGrid;

    private Dictionary<string, Image> selectImages;

    public void AddButtons(Item itemData,bool isWeapon , Action<string> addEvent)
    {
        GameObject button = Instantiate(itemButtons, isWeapon ? weaponGrid : itemGrid);
        TextMeshProUGUI itemName = button.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI itemDescription = button.transform.Find("Description").GetComponent<TextMeshProUGUI>();
        Image itemImage = button.transform.Find("ItemImage").GetComponent<Image>();
        Image selectImage = button.transform.Find("SelectImage").GetComponent<Image>();

        itemName.text = itemData.name;
        itemDescription.text = itemData.Description;
        itemImage.sprite = itemData.itemImage;
        selectImage.enabled = false;


        button.GetComponent<Button>().onClick.AddListener (()=> addEvent(itemData.name));
        selectImages.Add(itemData.name, selectImage);
    }

    public void CheckSelected(string name, bool isSelected)
    {
        if(selectImages.ContainsKey(name))
        {
            selectImages[name].enabled = isSelected;
        }
    }
}
