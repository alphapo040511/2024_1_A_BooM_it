using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    const int minPlayer = 2;        //최소 플레이어 수
    public int maxPlayer = 2;       //최대 플레이어 수

    [Networked][Capacity(4)] public NetworkDictionary<int, bool> players => default;
    [Networked][Capacity(4)] public NetworkLinkedList<int> playersHash { get; }

    public int thisPlayerHash = 0;

    private bool isPlayAble = false;

    public override void Spawned()
    {
        base.Spawned();
    }

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<NetworkManager>().onPlayerCount += CheckingPlayer;
        thisPlayerHash = Runner.LocalPlayer.GetHashCode();
        readyButton.onClick.AddListener(OnClickReadyButton);
        if(Runner.IsServer)
        {
            startButton.gameObject.SetActive(true);
            startButton.onClick.AddListener(OnClickStartButton);
        }    
        else
        {
            startButton.gameObject.SetActive(false);
        }
    }

    public void OnClickReadyButton()
    {
        Debug.Log("레디 버튼 클릭");
        if(players.ContainsKey(thisPlayerHash))
        {
            bool nowState = players.Get(thisPlayerHash);
            players.Set(thisPlayerHash, nowState);
        }
    }

    public void OnClickStartButton()
    {
        Debug.Log("게임시작");
    }

   
    //레디 상태 체크
    private void GetReadyCount()
    {
        if (isPlayAble == false) return;        //플레이 가능할 인원이 아닐 경우 리턴

        int count = 0;
        foreach(int hash in playersHash)
        {
            count += players.Get(hash) ? 1 : 0;
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
    private void CheckingPlayer(bool add, int playerHash)
    {
        //if (Runner.IsServer)
        {
            if (add)
            {
                Debug.Log("플레이어 추가 : " + playerHash);
                players.Add(playerHash, false);
                playersHash.Add(playerHash);
            }
            else
            {
                players.Remove(playerHash);
                playersHash.Remove(playerHash);
            }


            if (minPlayer <= players.Count || players.Count <= maxPlayer)
            {
                isPlayAble = true;
            }
            else
            {
                isPlayAble = false;
            }
        }

    }
}

