using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public ItemButtonManager itemButtonManager;

    public Action<string> setWeapon;
    public Action<string> setItem;

    private int SelectWeaponIndex;

    void Start()
    {
        setWeapon += SetWeapon;
        setItem += SetItem;

        Item[] weapons = Resources.LoadAll<Item>("Weapons");
        Item[] items = Resources.LoadAll<Item>("Items");

        GenerateButtons(weapons, true, SetWeapon);
        GenerateButtons(items, false, SetItem);
    }

    private void GenerateButtons(Item[] target, bool isWeapon, Action<string> addEvent)
    {
        foreach(Item item in target)
        {
            itemButtonManager.AddButtons(item, isWeapon, addEvent);
        }
    }

    private void SetWeapon(string key)
    {
        string oldWeapon = GameManager.instance?.weaponIndex[SelectWeaponIndex];
        GameManager.instance?.SetWeapon(key, SelectWeaponIndex);
        itemButtonManager.CheckSelected(oldWeapon, GameManager.instance?.weaponIndex.Contains(oldWeapon));
    }

    private void SetItem(string key)
    {
        GameManager.instance?.SetItem(key);
    }
}
