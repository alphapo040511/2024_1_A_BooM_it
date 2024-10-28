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
    Countdown,      // ī��Ʈ�ٿ� ��
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
    [Networked] public NetworkDictionary<PlayerRef, int> playerPoints => default;

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

    // ó�� ���۽� ����
    public override void Spawned()
    {
        readyButton.onClick.AddListener(OnClickReadyButton);            //���� ��ư�� ������ ���
        if(Runner.IsServer)                                             //�÷��̾ ȣ��Ʈ �϶�
        {
            startButton.gameObject.SetActive(true);                     //���� ���� ��ư Ȱ��ȭ �� ������ ���
            startButton.onClick.AddListener(OnClickStartButton);
        }    
        else
        {
            startButton.gameObject.SetActive(false);
        }
    }


    //���� ��ư�� ������ ��
    public void OnClickReadyButton()
    {
        PlayerRef player = Runner.LocalPlayer;
        if (players.ContainsKey(player))                //�ش� Ű ���� ������
        {
            bool nowState = players.Get(player);        //�ش� �÷��̾��� �غ� �� 
            readyButton.image.color = nowState ? Color.red : Color.white;
            RPC_PlayerValueChange(player, !nowState);   //�÷��̾� ��ųʸ��� bool(�غ�) �� ����
        }
    }

    //�÷��̾� ��ųʸ��� bool �� ����
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerValueChange(PlayerRef player, bool value = true)
    {
        if (players.ContainsKey(player))            //�ش� Ű ���� ������
        {
            players.Set(player, value);             //�÷��̾� ���¸� ���� (�⺻�� true)
        }
        GetReadyCount();                            //true ���� �ش��ϴ� �ο� �� üũ
    }

    //���� ���� ��ư�� ������ ��
    public void OnClickStartButton()
    {
        RPC_GameStart();                            //���� ���� RPC ȣ��
        AllPlayerValueReset();                      //�÷��̾� �غ� ���� false�� �ʱ�ȭ
    }



    //���� ���� RPC
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_GameStart()
    {
        gameState = GameState.MapLoading;               //���� ���� �� �ε������� ����
        UpdateAllPlayersState(PlayerState.Loading);     //��� �÷��̾��� ���¸� '�ε���'���� ����
        MapLoad();                                      //�� �ε� ����
    }

    //�� �ε� ����
    public void MapLoad()
    {
        if (!nowLoading)
        {
            nowLoading = true;
            levelGenerator.MapLoading();
        }
    }

    //�� �ε� �Ϸ�
    public void MapLoadDone()
    {
        nowLoading = false;
        RPC_PlayerValueChange(Runner.LocalPlayer);
    }


    //���� ���� üũ
    private void GetReadyCount()
    {
        if (isPlayAble == false) return;                        //�÷��� ������ �ο��� �ƴ� ��� ����

        int count = 0;                                          //�غ� (Value == true) �ο��� �ľ��� int
        List<PlayerRef> alivePlayers = new List<PlayerRef>();   //����ִ� �ο��� ������ List (���� 3�� �̻� �÷��� �� ���)

        foreach (var kvp in players)
        {
            if(players.Get(kvp.Key))        //�÷��̾��� ���°��� true �� ���
            {
                count++;                    //�ο� �� + 1
            }
            else                            //false�� ��� (����ִ� ���)
            {
                alivePlayers.Add(kvp.Key);
            }
        }

        switch (gameState)                                                      //���� ���� ����
        {
            case GameState.Standby:                                             //�غ��� �϶�
                startButton.interactable = (count >= players.Count);            //��� �÷��̾ �غ� �Ͽ��ٸ� ���۹�ư Ȱ��ȭ
                break;

            case GameState.MapLoading:                                          //�� �ε��� �϶�
                if (count >= players.Count)                                     //��� �÷��̾ �ε� �Ϸ� �ߴٸ�
                {
                    PlayerPositionChange();
                    gameState = GameState.Countdown;                            //ī��Ʈ�ٿ� ������ ���� ����
                    UpdateAllPlayersState(PlayerState.Standby);                 //��� �÷��̾��� ���¸� �����(ī��Ʈ�ٿ�)���� ����
                    StartCoroutine(Countdown());
                    AllPlayerValueReset();                                      //��� �÷��̾��� Value �� false�� ����
                }
                break;

            case GameState.InGame:                                              //���� ������ �϶�
                if (count >= players.Count - 1)                                 //��Ƴ��� �ο��� 1���� ��(1�� ���� ��� �׾��� ��)
                {
                    gameState = GameState.RoundOver;                            //���� ���¸� ���� ����� ����
                    foreach (var kvp in players)                                //����ִ� �÷��̾� �˻�(2�� ���� �÷��̽�)
                    {
                        if (players[kvp.Key] == false)                          //����ִ� ���
                        {
                            playerPoints.Set(kvp.Key, +1);                      //�ش� �÷��̾��� ���� + 1
                            if (playerPoints[kvp.Key] >= 3)                     //�÷��̾��� ������ 3���� �޼� �ߴٸ�
                            {
                                gameState = GameState.GameOver;                 //���� ���� ���� ����� ����
                            }
                        }
                    }
                    UpdataPoints();                                             //���� ǥ�� ������Ʈ
                    AllPlayerValueReset();                                      //��� �÷��̾��� ���� �ʱ�ȭ
                }
                break;
        }

        Debug.Log(gameState);
    }

    private IEnumerator Countdown()
    {
        for(int i = 3; i >= 0; i--)
        {
            RPC_Countdown(i);
            yield return new WaitForSeconds(1);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Countdown(int i)
    {
        Debug.Log(i);
        if(i == 0)
        {
            UpdateAllPlayersState(PlayerState.Playing);
            gameState = GameState.InGame;
        }
    }

    private void UpdataPoints()
    {
        //���� ǥ��     RPC�� ��ü�� �Ѹ��� ����
    }

    //�÷��̾� ����
    public void PlayerJoin(PlayerRef player)
    {
        if (Runner.IsServer)                            //�÷��̾ ȣ��Ʈ���
        {
            players.Add(player, false);                 //�÷��̾� ��ųʸ��� �߰�
            playerPoints.Add(player, 0);                //�÷��̾� ����Ʈ ��ųʸ��� �߰�
            CheckPlayerCount();                         //�÷��̾� ���� Ȯ��
        }
    }

    //�÷��̾� ����
    public void PlayerLeft(PlayerRef player)
    {
        if (Runner.IsServer)                            //�÷��̾ ȣ��Ʈ���
        {
            players.Remove(player);                     //�÷��̾� ��ųʸ����� ����
            playerPoints.Remove(player);                //�÷��̾� ����Ʈ ��ųʸ����� ����
            CheckPlayerCount();                         //�÷��̾� ���� Ȯ��
        }
    }

    //�÷��̾� ���� Ȯ��
    private void CheckPlayerCount()
    {
        if (minPlayer <= players.Count && players.Count <= maxPlayer)       //���� �÷��̾� ���ڰ� �ּ� �ο� ~ �ִ� �ο� ������ ���
        {
            isPlayAble = true;
        }
        else
        {
            isPlayAble = false;
        }
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

    //PlayerRef�� ���� Player Ŭ������ ��ȯ
    private Player GetPlayerObject(PlayerRef player)
    {
        if (NetworkManager.Instance._spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            return networkObject.GetComponent<Player>();
        }
        return null;
    }

    //��� �÷��̾��� Value �� false�� ����
    private void AllPlayerValueReset()
    {
        foreach (var kvp in players)
        {
            players.Set(kvp.Key, false);
        }
    }

    public void PlayerPositionChange()
    {
        int count = 0;
        foreach (var kvp in players)
        {
            Vector3 position = GetNextSpawnPosition(count);
            Quaternion lookDir = GetLookDirection(position);
            Player player = GetPlayerObject(kvp.Key);
            player.Respawn(position, lookDir);
            count++;
        }
    }

    private Vector3 GetNextSpawnPosition(int playerNum)
    {
        Vector3Int pos = GameManager.instance.mapData.SpawnPosition[playerNum];
        pos.y = GameManager.instance.mapData.BlockArr.GetLength(1);
        for (int y = GameManager.instance.mapData.BlockArr.GetLength(1) - 1; y >= 0; y--)
        {
            if (GameManager.instance.mapData.BlockArr[pos.x, y, pos.z] == 0)
            {
                pos.y = y;
            }
            else
            {
                break;
            }
        }
        return pos;
    }

    private Quaternion GetLookDirection(Vector3 spawnPosition)
    {
        float x = GameManager.instance.mapData.BlockArr.GetLength(0) / 2;
        float z = GameManager.instance.mapData.BlockArr.GetLength(2) / 2;
        Vector3 target = new Vector3(x, spawnPosition.y, z);
        return Quaternion.LookRotation(target - spawnPosition).normalized;
    }
}

