using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    Standby,        // ��� ��
    InGame,         // ���� ��
    RoundOver,      // ���� ����
    GameOver        // ���� ����
}

public class BattleManager : NetworkBehaviour
{
    public NetworkManager networkManager;
    public GameState gameState = GameState.Standby;

    public int maxPlayer = 2;       //�ִ� �÷��̾� ��

    private Dictionary<int, bool> players = new Dictionary<int, bool>();

    private bool isHost = false;

    // Start is called before the first frame update
    void Start()
    {
        networkManager = NetworkManager.instance;
        networkManager.onPlayerCount += CheckingPlayer;
        isHost = GetComponent<NetworkObject>().HasStateAuthority;

        if (isHost)       //ȣ��Ʈ�� ���
        {

        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage_Ready(int hashcode, bool ready)
    {
        RPC_RelayMessage_Ready(hashcode, ready);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage_Ready(int hashcode, bool ready)
    {
        players[hashcode] = ready;
        GetReadyCount();
    }

    private void GetReadyCount()
    {
        int count = 0;
        foreach(bool b in players.Values)
        {
            count += b ? 1 : 0;
        }

        if(count >= players.Count)
        {
            Debug.Log("���� ����");
        }
        else
        {
            Debug.Log("��� �÷��̾ �غ� �ؾ� �մϴ�.");
        }
    }

    private void CheckingPlayer(int i, int playerRef)
    {
        if(i > 0)
        {
            players.Add(playerRef, false);
        }
        else if(i < 0)
        {
            players.Remove(playerRef);
        }


        if(2 <= players.Count || players.Count <= maxPlayer)
        {
            //�÷��� ���� ���·� ��ȯ
            
        }

    }
}

