using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public ItemButtonManager itemButtonManager;

    private Dictionary<string, Item> itemDatas = new Dictionary<string, Item>();

    public Action<string> setWeapon;
    public Action<string> setItem;

    public Action<string> showDescription;

    private int SelectWeaponIndex;

    void Start()
    {
        setWeapon += SetWeapon;
        setItem += SetItem;
        showDescription += ShowDescription;

        Item[] weapons = Resources.LoadAll<Item>("Weapons");
        Item[] items = Resources.LoadAll<Item>("Items");

        KeepItemData(weapons);
        KeepItemData(items);

        GenerateButtons(weapons, true, SetWeapon);
        GenerateButtons(items, false, SetItem);

        Invoke("CheckCurrentItem", 0.5f);
    }

    public void LoadItemList()
    {
        foreach(string index in itemDatas.Keys)
        {
            ManagerItemCheck(index);
        }
    }

    public void ItemButton(int index)
    {
        SelectWeaponIndex = index;
        itemButtonManager.weaponGrid.gameObject.SetActive(!(SelectWeaponIndex == 3));
        itemButtonManager.itemGrid.gameObject.SetActive((SelectWeaponIndex == 3));
    }

    private void KeepItemData(Item[] datas)
    {
        foreach (Item data in datas)
        {
            itemDatas.Add(data.itemIndex, data);
        }
    }

    private void GenerateButtons(Item[] target, bool isWeapon, Action<string> addEvent)
    {
        foreach(Item item in target)
        {
            itemButtonManager.AddButtons(item, isWeapon, addEvent, showDescription);
            ManagerItemCheck(item.itemIndex);
        }
    }

    private void SetWeapon(string key)
    {
        string oldWeapon = GameManager.instance.weaponIndex[SelectWeaponIndex];
        GameManager.instance.SetWeapon(key, SelectWeaponIndex);
        ManagerItemCheck(oldWeapon);
        ManagerItemCheck(key);
        CheckCurrentItem();
    }

    private void SetItem(string key)
    {
        string oldWeapon = GameManager.instance.itemIndex;
        GameManager.instance.SetItem(key);
        ManagerItemCheck(oldWeapon);
        ManagerItemCheck(key);
        CheckCurrentItem();
    }

    private void ManagerItemCheck(string key)
    {
        if (key == "None")
        {
            return;
        }

        bool isActive = (GameManager.instance.weaponIndex.Contains(key) || GameManager.instance.itemIndex == key);
        itemButtonManager.CheckSelected(key, isActive);
    }

    private void CheckCurrentItem()
    {
        for(int i = 0; i < 3; i++)
        {
            string weaponName = GameManager.instance.weaponIndex[i];
            if (weaponName != "None")
            {
                if (itemDatas.ContainsKey(weaponName))
                {
                    itemButtonManager.SetCurrentItemButton(i, true, itemDatas[weaponName].itemImage);
                }
            }
            else
            {
                itemButtonManager.SetCurrentItemButton(i, false);
            }
        }

        string itemName = GameManager.instance.itemIndex;
        if (itemName != null)
        {
            if (itemDatas.ContainsKey(itemName))
            {
                itemButtonManager.SetCurrentItemButton(3, true, itemDatas[itemName].itemImage);
            }
        }
        else
        {
            itemButtonManager.SetCurrentItemButton(3, false);
        }
    }


    private void ShowDescription(string key)
    {
        if(itemDatas.ContainsKey(key))
        {
            Vector3 point = Input.mousePosition;
            point -= new Vector3(960, 540, 0);
            itemButtonManager.ShowDescription(itemDatas[key], point);
        }
    }
}
