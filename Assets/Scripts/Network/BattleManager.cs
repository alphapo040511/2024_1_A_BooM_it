using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    Matching,       // 매칭 중
    Standby,        // 대기 중
    InGame,         // 게임 중
    RoundOver,      // 라운드 종료
    GameOver        // 게임 종료
}

public class BattleManager : NetworkBehaviour
{
    public NetworkManager networkManager;
    public GameState gameState = GameState.Matching;

    public int maxPlayer = 2;       //일단 최대 플레이어 수

    // Start is called before the first frame update
    void Start()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
        networkManager.onPlayerCount += CheckingPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    private void CheckingPlayer(int nowPlayerCount)
    {
        if(2 <= nowPlayerCount || nowPlayerCount <= maxPlayer)
        {
            //플레이 가능 상태로 전환
            Debug.Log(nowPlayerCount);
        }

    }
}

