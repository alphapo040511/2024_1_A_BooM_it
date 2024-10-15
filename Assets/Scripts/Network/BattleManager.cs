using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public enum GameState
{
    Standby,        // 대기 중
    InGame,         // 게임 중
    RoundOver,      // 라운드 종료
    GameOver        // 게임 종료
}

public class BattleManager : NetworkBehaviour
{
    public Button readyButton;
    public Button startButton;

    public TextMeshProUGUI readyText;

    public NetworkManager networkManager;
    public GameState gameState = GameState.Standby;

    public int maxPlayer = 2;       //최대 플레이어 수

    private Dictionary<int, bool> players = new Dictionary<int, bool>();

    public int thisPlayerHash = 0;

    private bool isPlayAble = false;

    // Start is called before the first frame update
    void Start()
    {
        networkManager = NetworkManager.instance;
        networkManager.onPlayerCount += CheckingPlayer;

        readyText.text = "Ready";
        readyButton.onClick.AddListener(OnClickReadyButton);

        Debug.Log(HasStateAuthority);
        if (HasStateAuthority)       //호스트인 경우
        {
            startButton.gameObject.SetActive(true);
            startButton.onClick.AddListener(OnClickStartButton);
        }
    }

    public void OnClickReadyButton()
    {
        Debug.Log("레디 버튼 클릭");
        if(players.ContainsKey(thisPlayerHash))
        {
            Debug.Log(HasInputAuthority);
            bool isReady = players[thisPlayerHash];
            if (HasInputAuthority)
            {
                Debug.Log("신호 전달");
                RPC_SendMessage_Ready(thisPlayerHash, !isReady);
            }
        }
        else
        {
            Debug.Log("플레이어가 목록에 없음");
        }
    }

    public void OnClickStartButton()
    {
        Debug.Log("게임시작");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage_Ready(int hashcode, bool ready)
    {
        RPC_RelayMessage_Ready(hashcode, ready);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage_Ready(int hashcode, bool ready)
    {
        Debug.Log("신호 도착");
        if (players.ContainsKey(hashcode))
        {
            players[hashcode] = ready;
            GetReadyCount();
        }
    }

    //레디 상태 체크
    private void GetReadyCount()
    {
        if (isPlayAble == false) return;        //플레이 가능할 인원이 아닐 경우 리턴

        int count = 0;
        foreach(bool b in players.Values)
        {
            count += b ? 1 : 0;
        }

        if(count >= players.Count)
        {
            startButton.interactable = true;
            Debug.Log("시작 가능");
        }
        else
        {
            startButton.interactable = false;
            Debug.Log("모든 플레이어가 준비를 해야 합니다.");
        }
    }

    //플레이어 인원 체크
    private void CheckingPlayer(int i, int playerRef)
    {
        if (!HasStateAuthority) return;

        if(i > 0)
        {
            Debug.Log("플레이어 추가 : " + playerRef);
            players.Add(playerRef, false);
        }
        else if(i < 0)
        {
            players.Remove(playerRef);
        }


        if(2 <= players.Count || players.Count <= maxPlayer)
        {
            isPlayAble = true;
        }
        else
        {
            isPlayAble = false;
        }

    }
}

