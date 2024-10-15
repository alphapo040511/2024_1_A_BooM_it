using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public MapData mapData;

    public List<NetworkPrefabRef> bombPrefabs;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public int[] bombIndex = new int[3];
    public int itemIndex;

    public void SaveItems(int[] bombIndex, int itemIndex)
    {
        this.bombIndex = (int[])bombIndex.Clone();
        this.itemIndex = itemIndex;
        Debug.Log("Save");
    }
}
