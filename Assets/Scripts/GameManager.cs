using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public string mapIndex;

    public string[] weaponIndex = new string[3];
    public string itemIndex;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetWeapon(string name, int index)
    {
        for(int i = 0; i < weaponIndex.Length; i++)
        {
            if (weaponIndex[i] == name)
            {
                weaponIndex[i] = null;
            }
        }

        weaponIndex[index] = name;
    }

    public void SetItem(string name)
    {
        if(itemIndex == name)
        {
            itemIndex = null;
            return;
        }
        else
        {
            itemIndex = name;
            return; 
        }
    }
    }
}
