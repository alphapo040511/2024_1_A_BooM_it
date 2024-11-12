using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public HUDManager hudManager;

    public Button readyButton;
    public Button startButton;

    public CanvasGroup menuCanvas;
    public TextMeshProUGUI readyCount;

    public GameObject[] countdownImage = new GameObject[4];

    public GameState gameState = GameState.Standby;

    const int minPlayer = 1;        //�ּ� �÷��̾� ��
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

        GetReadyCount();
    }


    //���� ��ư�� ������ ��
    public void OnClickReadyButton()
    {
        PlayerRef player = Runner.LocalPlayer;
        if (players.ContainsKey(player))                //�ش� Ű ���� ������
        {
            bool nowState = players.Get(player);        //�ش� �÷��̾��� �غ� �� 
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
        StartCoroutine(FadeCanvas(menuCanvas, false));
        MapLoad();                                      //�� �ε� ����
    }


    

    private IEnumerator FadeCanvas(CanvasGroup target, bool fadeIn)
    {
        yield return new WaitForSeconds(0.5f);
        hudManager.finish.position = new Vector3(-960, 255, 0);             //���� UI �ʱ�ȭ (���߿� �ٽ� ����)
        if (fadeIn)
        {
            while(target.alpha < 1.0f)
            {
                target.alpha += Runner.DeltaTime;
                yield return Runner.DeltaTime;
            }
        }
        else
        {
            while (target.alpha > 0)
            {
                target.alpha -= Runner.DeltaTime;
                yield return Runner.DeltaTime;
            }
            target.gameObject.SetActive(false);
        }
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
        if (HasStateAuthority)
        {
            RPC_ReadyCountUI(count);
        }

        if (isPlayAble == false) return;                        //�÷��� ������ �ο��� �ƴ� ��� ����

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
                            int point = playerPoints.Get(kvp.Key);
                            playerPoints.Set(kvp.Key, point + 1);               //�ش� �÷��̾��� ���� + 1
                            Debug.Log(kvp.Key + "�÷��̾� +1 ��");
                            if (playerPoints[kvp.Key] >= 3)                     //�÷��̾��� ������ 3���� �޼� �ߴٸ�
                            {
                                gameState = GameState.GameOver;                 //���� ���� ���� ����� ����
                                Debug.Log(kvp.Key + "�÷��̾� �¸�");
                            }
                            RPC_UpdatePoint(kvp.Key, playerPoints[kvp.Key]);
                        }
                    }
                    AllPlayerValueReset();
                }
                break;
            case GameState.RoundOver:                                           //���� ���� ������ ���
                if (count >= players.Count)                                     //��� ������ ���� �Ǿ��ٸ�
                {
                    AllPlayerValueReset();
                    RPC_GameStart();                                            //���� �����
                }
                break;
            case GameState.GameOver:
                RPC_Shutdown();
                break;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Shutdown()
    {
        Debug.Log("��������");
        Cursor.lockState = CursorLockMode.None;
        Runner.LoadScene("LobbyScene");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ReadyCountUI(int count)
    {
        readyCount.text = $"{count}/{Runner.SessionInfo.PlayerCount}";
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1);

        for (int i = 3; i >= 0; i--)
        {
            RPC_Countdown(i);
            yield return new WaitForSeconds(1);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Countdown(int i)
    {
        countdownImage[i].SetActive(true);
        if (i == 0)
        {
            UpdateAllPlayersState(PlayerState.Playing);
            gameState = GameState.InGame;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdatePoint(PlayerRef target, int point)
    {
        hudManager.AddPoint(target == Runner.LocalPlayer, point);
    }

    public void UpdatePointComplete()
    {
        RPC_PlayerValueChange(Runner.LocalPlayer);
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

        GetReadyCount();
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

