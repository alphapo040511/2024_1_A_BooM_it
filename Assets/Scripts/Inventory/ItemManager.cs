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

        //CurrentItemCheck();

        GenerateButtons(weapons, true, SetWeapon);
        GenerateButtons(items, false, SetItem);
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
            itemDatas.Add(data.itemName, data);
            ManagerItemCheck(data.itemName);
        }
    }

    private void GenerateButtons(Item[] target, bool isWeapon, Action<string> addEvent)
    {
        foreach(Item item in target)
        {
            itemButtonManager.AddButtons(item, isWeapon, addEvent, showDescription);
        }
    }

    private void SetWeapon(string key)
    {
        string oldWeapon = GameManager.instance.weaponIndex[SelectWeaponIndex];
        GameManager.instance.SetWeapon(key, SelectWeaponIndex);
        ManagerItemCheck(oldWeapon);
        ManagerItemCheck(key);
    }

    private void SetItem(string key)
    {
        string oldWeapon = GameManager.instance.itemIndex;
        GameManager.instance.SetItem(key);
        ManagerItemCheck(oldWeapon);
        ManagerItemCheck(key);
    }

    private void ManagerItemCheck(string key)
    {
        bool isActive = (GameManager.instance.weaponIndex.Contains(key) || GameManager.instance.itemIndex == key);
        itemButtonManager.CheckSelected(key, isActive);
    }



    private void ShowDescription(string key)
    {
        if(itemDatas.ContainsKey(key))
        {
            itemButtonManager.ShowDescription(itemDatas[key]);
        }
    }
}
