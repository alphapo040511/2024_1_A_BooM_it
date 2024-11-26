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

    public GameObject descriptionPopup;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI description;
    public TextMeshProUGUI cooltime;
    public TextMeshProUGUI count;

    public Action infoOutEvent;

    private Dictionary<string, Image> selectImages = new Dictionary<string, Image>();

    private void Start()
    {
        infoOutEvent += HideDescription;
    }

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

        EventTrigger.Entry _entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        _entry.callback.AddListener((evenetData) => infoOutEvent());
        button.GetComponent<EventTrigger>().triggers.Add(_entry);

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
        Debug.Log($"{index} ĭ�� {itemImage} �̹��� ����");
    }

    public void ShowDescription(Item data, Vector3 mousePos)
    {
        descriptionPopup.transform.localPosition = mousePos;

        itemName.text = data.itemName;
        string info = data.Description.Replace("duration", data.duration.ToString());
        description.text = info;

        if(data.itemType != ItemType.Weapon)
        {
            cooltime.text = $"��Ÿ�� : {data.cooldownTime}��";
            count.text = $"��밡�� Ƚ�� : {data.maxUses}";
        }
        else
        {
            cooltime.text = $"�߻� ��Ÿ�� : {data.cooldownTime}��";
            count.text = "";
        }

        descriptionPopup.SetActive(true);
    }

    private void HideDescription()
    {
        descriptionPopup.SetActive(false);
    }
}
