using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


// NetworkManager 클래스
public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    public NetworkRunner _runner;
    private NetworkInputHandler _inputHandler;
    [SerializeField] private string _gameSceneName = "GameScene"; // 게임 씬 이름
    public Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public Action<List<SessionInfo>> updateSessions;

    public event Action<bool, int> onPlayerCount;

    private static NetworkManager _instance;
    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkManager>();
                if (_instance == null)
                {
                    GameObject newManager = new GameObject("NetworkManager");
                    _instance = newManager.AddComponent<NetworkManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 NetworkManager가 유지되도록 함
        }
        else if (_instance != this)
        {
            Destroy(gameObject);  // 기존 인스턴스가 있으면 새로 생성된 인스턴스를 파괴
        }

        _inputHandler = gameObject.AddComponent<NetworkInputHandler>();
    }

    public NetworkRunner NewRunner()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        return _runner;
    }

    public async Task JoinLobby()
    {
        var result = await _runner.JoinSessionLobby(SessionLobby.ClientServer);
        if(result.Ok)
        {
            Debug.Log("로비에 입장");
        }
    }

    public async void StartGame(GameMode mode, string roomName, int playerCount)
    {
        // 게임 씬의 빌드 인덱스 찾기
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(_gameSceneName);
        if (sceneIndex == -1)
        {
            Debug.LogError($"{_gameSceneName}을(를) 빌드 설정에서 찾을 수 없습니다!");
            return;
        }

        // Load the target scene
        var gameScene = SceneRef.FromIndex(sceneIndex);

        // Check if the scene was found
        if (gameScene == SceneRef.None)
        {
            Debug.LogError("GameScene could not be found in the available scenes!");
            return;
        }

        // 세션 시작 또는 참가
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName, // 사용자가 입력한 방 이름 사용
            Scene = gameScene,
            PlayerCount = playerCount,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // 플레이어 스폰 위치 계산을 위한 새로운 로직
            Vector3 spawnPosition = GetNextSpawnPosition();
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player,
                            (runner, o) => o.GetComponent<Player>().Init());
            _spawnedCharacters.Add(player, networkPlayerObject);
            BattleManager.Instance?.PlayerJoin(player);
        }
        Debug.Log($"플레이어 참가: {player}");
    }

    private Vector3 GetNextSpawnPosition()
    { 
        int playerCount = _spawnedCharacters.Count;
        return new Vector3(-playerCount * 5 - 5, 0, -10);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
            onPlayerCount(false, player.GetHashCode());
            BattleManager.Instance?.PlayerLeft(player);
        }
        Debug.Log($"플레이어 퇴장: {player}");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(_inputHandler.GetNetworkInput());
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"네트워크 셧다운: {shutdownReason}");
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("서버에 연결됨");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"서버와의 연결이 끊김: {reason}");
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log($"연결 실패: {reason}");
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
    {
        updateSessions(sessionList);
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("씬 로드 완료");
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("씬 로드 시작");
    }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"오브젝트가 AOI를 벗어남: {obj.Id}");
    }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"오브젝트가 AOI에 들어옴: {obj.Id}");
    }
}