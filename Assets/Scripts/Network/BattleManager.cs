using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    Standby,        // 대기 중
    InGame,         // 게임 중
    RoundOver,      // 라운드 종료
    GameOver        // 게임 종료
}

public class BattleManager : NetworkBehaviour
{
    public NetworkManager networkManager;
    public GameState gameState = GameState.Standby;

    public int maxPlayer = 2;       //최대 플레이어 수

    private Dictionary<int, bool> players = new Dictionary<int, bool>();

    private bool isHost = false;

    // Start is called before the first frame update
    void Start()
    {
        networkManager = NetworkManager.instance;
        networkManager.onPlayerCount += CheckingPlayer;
        isHost = GetComponent<NetworkObject>().HasStateAuthority;

        if (isHost)       //호스트인 경우
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
            Debug.Log("시작 가능");
        }
        else
        {
            Debug.Log("모든 플레이어가 준비를 해야 합니다.");
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
            //플레이 가능 상태로 전환
            
        }

    }
}

