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
    MapLoading,     // �� ���� ��
    InGame,         // ���� ��
    RoundOver,      // ���� ����
    GameOver        // ���� ����
}

public class BattleManager : NetworkBehaviour
{
    public NetworkLevelManager levelManager;
    public NetworkLevelGenerator levelGenerator;

    public Button readyButton;
    public Button startButton;

    public TextMeshProUGUI readyText;

    public GameState gameState = GameState.Standby;

    const int minPlayer = 2;        //�ּ� �÷��̾� ��
    public int maxPlayer = 2;       //�ִ� �÷��̾� ��

    private bool nowLoading = false;

    [Networked] public NetworkDictionary<PlayerRef, bool> players => default;

    private bool isPlayAble = false;

    private static BattleManager _instance;
    public static BattleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BattleManager>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    public override void Spawned()
    {
        readyButton.onClick.AddListener(OnClickReadyButton);
        Debug.Log(Runner.IsServer);
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
        RPC_PlayerReady(Runner.LocalPlayer);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerReady(PlayerRef player)
    {
        if (players.ContainsKey(player))
        {
            bool nowState = players.Get(player);
            if (gameState == GameState.Standby)
            {
                players.Set(player, !nowState);
            }
            else if (gameState == GameState.MapLoading)
            {
                players.Set(player, true);
            }

        }
        GetReadyCount();
    }

    public void OnClickStartButton()
    {
        RPC_GameStart();
        foreach (var kvp in players)
        {
            bool player = kvp.Value;
            player = false;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_GameStart()
    {
        gameState = GameState.MapLoading;
        UpdateAllPlayersState(PlayerState.Standby);
        MapLoad();
    }

    //��� �÷��̾��� ���¸� �ٲ��ִ� �޼���
    private void UpdateAllPlayersState(PlayerState newState)
    {
        foreach (var kvp in players)
        {
            PlayerRef player = kvp.Key;
            Player playerObject = GetPlayerObject(player);
            if (playerObject != null)
            {
                playerObject.UpdataState(newState);
            }
        }
    }

    private Player GetPlayerObject(PlayerRef player)
    {
        if (NetworkManager.Instance._spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            return networkObject.GetComponent<Player>();
        }
        return null;
    }

    public void MapLoad()
    {
        if (!nowLoading)
        {
            nowLoading = true;
            levelGenerator.MapLoading();
        }
    }

    public void MapLoadDone()
    {
        nowLoading = false;
        RPC_PlayerReady(Runner.LocalPlayer);
    }


    //���� ���� üũ
    private void GetReadyCount()
    {
        if (isPlayAble == false) return;        //�÷��� ������ �ο��� �ƴ� ��� ����

        int count = 0;
        foreach(var kvp in players)
        {
            count += kvp.Value ? 1 : 0;         //�غ� �Ǿ��ٸ� 1, �ƴ϶�� 0 �� ���� �ο� �ľ�
        }

        if (gameState == GameState.Standby)
        {
            if (count >= players.Count)
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
        else if (gameState == GameState.MapLoading)
        {
            if (count >= players.Count)
            {
                gameState = GameState.InGame;
                UpdateAllPlayersState(PlayerState.Playing);
            }
        }
    }

    //�÷��̾� �ο� üũ
    public void PlayerJoin(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            players.Add(player, false);

            CheckPlayerCount();
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            players.Remove(player);

            CheckPlayerCount();
        }
    }

    private void CheckPlayerCount()
    {
        if (minPlayer <= players.Count && players.Count <= maxPlayer)
        {
            isPlayAble = true;
        }
        else
        {
            isPlayAble = false;
        }
    }

}

