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
    Standby,        // ��� ��
    InGame,         // ���� ��
    RoundOver,      // ���� ����
    GameOver        // ���� ����
}

public class BattleManager : NetworkBehaviour
{
    public Button readyButton;
    public Button startButton;

    public TextMeshProUGUI readyText;

    public NetworkManager networkManager;
    public GameState gameState = GameState.Standby;

    const int minPlayer = 2;        //�ּ� �÷��̾� ��
    public int maxPlayer = 2;       //�ִ� �÷��̾� ��

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
        Debug.Log("���� ��ư Ŭ��");
        if(players.ContainsKey(thisPlayerHash))
        {
            bool nowState = players.Get(thisPlayerHash);
            players.Set(thisPlayerHash, nowState);
        }
    }

    public void OnClickStartButton()
    {
        Debug.Log("���ӽ���");
    }

   
    //���� ���� üũ
    private void GetReadyCount()
    {
        if (isPlayAble == false) return;        //�÷��� ������ �ο��� �ƴ� ��� ����

        int count = 0;
        foreach(int hash in playersHash)
        {
            count += players.Get(hash) ? 1 : 0;
        }

        if(count >= players.Count)
        {
            startButton.interactable = true;
            Debug.Log("���� ����");
        }
        else
        {
            startButton.interactable = false;
            Debug.Log("��� �÷��̾ �غ� �ؾ� �մϴ�.");
        }
    }

    //�÷��̾� �ο� üũ
    private void CheckingPlayer(bool add, int playerHash)
    {
        //if (Runner.IsServer)
        {
            if (add)
            {
                Debug.Log("�÷��̾� �߰� : " + playerHash);
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

