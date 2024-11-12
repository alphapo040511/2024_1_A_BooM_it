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
    Standby,        // 대기 중
    MapLoading,     // 맵 생성 중
    Countdown,      // 카운트다운 중
    InGame,         // 게임 중
    RoundOver,      // 라운드 종료
    GameOver        // 게임 종료
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

    const int minPlayer = 1;        //최소 플레이어 수
    public int maxPlayer = 2;       //최대 플레이어 수

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

    // 처음 시작시 실행
    public override void Spawned()
    {
        readyButton.onClick.AddListener(OnClickReadyButton);            //레디 버튼에 리스너 등록
        if(Runner.IsServer)                                             //플레이어가 호스트 일때
        {
            startButton.gameObject.SetActive(true);                     //게임 시작 버튼 활성화 및 리스너 등록
            startButton.onClick.AddListener(OnClickStartButton);
        }    
        else
        {
            startButton.gameObject.SetActive(false);
        }

        GetReadyCount();
    }


    //레디 버튼을 눌렀을 때
    public void OnClickReadyButton()
    {
        PlayerRef player = Runner.LocalPlayer;
        if (players.ContainsKey(player))                //해당 키 값이 있을때
        {
            bool nowState = players.Get(player);        //해당 플레이어의 준비 값 
            RPC_PlayerValueChange(player, !nowState);   //플레이어 딕셔너리의 bool(준비) 값 변경
        }
    }

    //플레이어 딕셔너리의 bool 값 변경
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_PlayerValueChange(PlayerRef player, bool value = true)
    {
        if (players.ContainsKey(player))            //해당 키 값이 있을때
        {
            players.Set(player, value);             //플레이어 상태를 변경 (기본은 true)
        }
        GetReadyCount();                            //true 값에 해당하는 인원 수 체크
    }

    //게임 시작 버튼을 눌렀을 때
    public void OnClickStartButton()
    {
        RPC_GameStart();                            //게임 시작 RPC 호출
        AllPlayerValueReset();                      //플레이어 준비 상태 false로 초기화
    }



    //게임 시작 RPC
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_GameStart()
    {
        gameState = GameState.MapLoading;               //게임 상태 맵 로딩중으로 변경
        UpdateAllPlayersState(PlayerState.Loading);     //모든 플레이어의 상태를 '로딩중'으로 변경
        StartCoroutine(FadeCanvas(menuCanvas, false));
        MapLoad();                                      //맵 로딩 시작
    }


    

    private IEnumerator FadeCanvas(CanvasGroup target, bool fadeIn)
    {
        yield return new WaitForSeconds(0.5f);
        hudManager.finish.position = new Vector3(-960, 255, 0);             //종료 UI 초기화 (나중에 다시 정리)
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

    //맵 로딩 시작
    public void MapLoad()
    {
        if (!nowLoading)
        {
            nowLoading = true;
            levelGenerator.MapLoading();
        }
    }

    //맵 로딩 완료
    public void MapLoadDone()
    {
        nowLoading = false;
        RPC_PlayerValueChange(Runner.LocalPlayer);
    }


    //레디 상태 체크
    private void GetReadyCount()
    {
        int count = 0;                                          //준비 (Value == true) 인원을 파악할 int
        List<PlayerRef> alivePlayers = new List<PlayerRef>();   //살아있는 인원을 저장할 List (추후 3인 이상 플레이 시 사용)

        foreach (var kvp in players)
        {
            if(players.Get(kvp.Key))        //플레이어의 상태값이 true 일 경우
            {
                count++;                    //인원 수 + 1
            }
            else                            //false인 경우 (살아있는 경우)
            {
                alivePlayers.Add(kvp.Key);
            }
        }
        if (HasStateAuthority)
        {
            RPC_ReadyCountUI(count);
        }

        if (isPlayAble == false) return;                        //플레이 가능할 인원이 아닐 경우 리턴

        switch (gameState)                                                      //현재 게임 상태
        {
            case GameState.Standby:                                             //준비중 일때
                startButton.interactable = (count >= players.Count);            //모든 플레이어가 준비 하였다면 시작버튼 활성화
                break;

            case GameState.MapLoading:                                          //맵 로딩중 일때
                if (count >= players.Count)                                     //모든 플레이어가 로딩 완료 했다면
                {
                    PlayerPositionChange();
                    gameState = GameState.Countdown;                            //카운트다운 중으로 상태 변경
                    UpdateAllPlayersState(PlayerState.Standby);                 //모든 플레이어의 상태를 대기중(카운트다운)으로 변경
                    StartCoroutine(Countdown());
                    AllPlayerValueReset();                                      //모든 플레이어의 Value 값 false로 변경
                }
                break;

            case GameState.InGame:                                              //게임 진행중 일때
                if (count >= players.Count - 1)                                 //살아남은 인원이 1명일 때(1명 빼고 모두 죽었을 때)
                {
                    gameState = GameState.RoundOver;                            //게임 상태를 라운드 종료로 변경
                    foreach (var kvp in players)                                //살아있는 플레이어 검색(2인 이하 플레이시)
                    {
                        if (players[kvp.Key] == false)                          //살아있는 경우
                        {
                            int point = playerPoints.Get(kvp.Key);
                            playerPoints.Set(kvp.Key, point + 1);               //해당 플레이어의 점수 + 1
                            Debug.Log(kvp.Key + "플레이어 +1 점");
                            if (playerPoints[kvp.Key] >= 3)                     //플레이어의 점수가 3점을 달성 했다면
                            {
                                gameState = GameState.GameOver;                 //게임 상태 게임 종료로 변경
                                Debug.Log(kvp.Key + "플레이어 승리");
                            }
                            RPC_UpdatePoint(kvp.Key, playerPoints[kvp.Key]);
                        }
                    }
                    AllPlayerValueReset();
                }
                break;
            case GameState.RoundOver:                                           //라운드 종료 상태일 경우
                if (count >= players.Count)                                     //모든 연출이 종료 되었다면
                {
                    AllPlayerValueReset();
                    RPC_GameStart();                                            //게임 재시작
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
        Debug.Log("게임종료");
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

    //플레이어 참가
    public void PlayerJoin(PlayerRef player)
    {
        if (Runner.IsServer)                            //플레이어가 호스트라면
        {
            players.Add(player, false);                 //플레이어 딕셔너리에 추가
            playerPoints.Add(player, 0);                //플레이어 포인트 딕셔너리에 추가
            CheckPlayerCount();                         //플레이어 숫자 확인
        }
    }

    //플레이어 퇴장
    public void PlayerLeft(PlayerRef player)
    {
        if (Runner.IsServer)                            //플레이어가 호스트라면
        {
            players.Remove(player);                     //플레이어 딕셔너리에서 제거
            playerPoints.Remove(player);                //플레이어 포인트 딕셔너리에서 제거
            CheckPlayerCount();                         //플레이어 숫자 확인
        }
    }

    //플레이어 숫자 확인
    private void CheckPlayerCount()
    {
        if (minPlayer <= players.Count && players.Count <= maxPlayer)       //현제 플레이어 숫자가 최소 인원 ~ 최대 인원 사이인 경우
        {
            isPlayAble = true;
        }
        else
        {
            isPlayAble = false;
        }

        GetReadyCount();
    }


    //모든 플레이어의 상태를 바꿔주는 메서드
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

    //PlayerRef를 통해 Player 클래스를 반환
    private Player GetPlayerObject(PlayerRef player)
    {
        if (NetworkManager.Instance._spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            return networkObject.GetComponent<Player>();
        }
        return null;
    }

    //모든 플레이어의 Value 값 false로 변경
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

