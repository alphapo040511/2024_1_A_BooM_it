using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

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

    public ItemIndex bombIndexData;
    public ItemIndex itemIndexData;

    public int[] bombIndex = new int[3];
    public int itemIndex;

    public void SaveItems(int[] bombIndex, int itemIndex)
    {
        this.bombIndex = (int[])bombIndex.Clone();
        this.itemIndex = itemIndex;
        Debug.Log("Save");
    }
}