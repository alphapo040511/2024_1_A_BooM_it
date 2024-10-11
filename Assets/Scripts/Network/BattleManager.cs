using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    Matching,       // ��Ī ��
    Standby,        // ��� ��
    InGame,         // ���� ��
    RoundOver,      // ���� ����
    GameOver        // ���� ����
}

public class BattleManager : NetworkBehaviour
{
    public NetworkManager networkManager;
    public GameState gameState = GameState.Matching;

    public int maxPlayer = 2;       //�ϴ� �ִ� �÷��̾� ��

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
            //�÷��� ���� ���·� ��ȯ
            Debug.Log(nowPlayerCount);
        }

    }
}

