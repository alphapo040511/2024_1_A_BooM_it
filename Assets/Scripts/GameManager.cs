using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public NetworkPrefabRef playerRef;

    public string mapIndex;

    public string[] weaponIndex = new string[3];
    public string itemIndex;

    public Dictionary<string, int> awardPoints = new Dictionary<string, int>();

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
                weaponIndex[i] = "None";
                if(i == index)
                {
                    return;
                }
            }
        }

        weaponIndex[index] = name;
    }

    public void SetItem(string name)
    {
        if(itemIndex == name)
        {
            itemIndex = "None";
        }
        else
        {
            itemIndex = name;
        }
    }

    public void AwardAddValue(string name, int point)
    {
        if (awardPoints.ContainsKey(name))
        {
            awardPoints[name] += point;
        }
        else
        {
            awardPoints.Add(name, point);
        }
    }

    public void ClearAward()
    {
        awardPoints.Clear();
    }
}
