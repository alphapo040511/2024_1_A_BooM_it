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

    public int maxPlayer = 2;       //�ִ� �÷��̾� ��

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
        if (HasStateAuthority)       //ȣ��Ʈ�� ���
        {
            startButton.gameObject.SetActive(true);
            startButton.onClick.AddListener(OnClickStartButton);
        }
    }

    public void OnClickReadyButton()
    {
        Debug.Log("���� ��ư Ŭ��");
        if(players.ContainsKey(thisPlayerHash))
        {
            Debug.Log(HasInputAuthority);
            bool isReady = players[thisPlayerHash];
            if (HasInputAuthority)
            {
                Debug.Log("��ȣ ����");
                RPC_SendMessage_Ready(thisPlayerHash, !isReady);
            }
        }
        else
        {
            Debug.Log("�÷��̾ ��Ͽ� ����");
        }
    }

    public void OnClickStartButton()
    {
        Debug.Log("���ӽ���");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage_Ready(int hashcode, bool ready)
    {
        RPC_RelayMessage_Ready(hashcode, ready);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage_Ready(int hashcode, bool ready)
    {
        Debug.Log("��ȣ ����");
        if (players.ContainsKey(hashcode))
        {
            players[hashcode] = ready;
            GetReadyCount();
        }
    }

    //���� ���� üũ
    private void GetReadyCount()
    {
        if (isPlayAble == false) return;        //�÷��� ������ �ο��� �ƴ� ��� ����

        int count = 0;
        foreach(bool b in players.Values)
        {
            count += b ? 1 : 0;
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
    private void CheckingPlayer(int i, int playerRef)
    {
        if (!HasStateAuthority) return;

        if(i > 0)
        {
            Debug.Log("�÷��̾� �߰� : " + playerRef);
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

