using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public MapData mapData;

    public List<NetworkParabola> bombPrefabs;

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
}
