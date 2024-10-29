using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


// NetworkManager Ŭ����
public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    public NetworkRunner _runner;
    private NetworkInputHandler _inputHandler;
    [SerializeField] private string _gameSceneName = "GameScene"; // ���� �� �̸�
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
            DontDestroyOnLoad(gameObject);  // �� ��ȯ �ÿ��� NetworkManager�� �����ǵ��� ��
        }
        else if (_instance != this)
        {
            Destroy(gameObject);  // ���� �ν��Ͻ��� ������ ���� ������ �ν��Ͻ��� �ı�
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
            Debug.Log("�κ� ����");
        }
    }

    public async void StartGame(GameMode mode, string roomName, int playerCount)
    {
        // ���� ���� ���� �ε��� ã��
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath(_gameSceneName);
        if (sceneIndex == -1)
        {
            Debug.LogError($"{_gameSceneName}��(��) ���� �������� ã�� �� �����ϴ�!");
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

        // ���� ���� �Ǵ� ����
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName, // ����ڰ� �Է��� �� �̸� ���
            Scene = gameScene,
            PlayerCount = playerCount,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // �÷��̾� ���� ��ġ ����� ���� ���ο� ����
            Vector3 spawnPosition = GetNextSpawnPosition();
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player,
                            (runner, o) => o.GetComponent<Player>().Init());
            _spawnedCharacters.Add(player, networkPlayerObject);
            BattleManager.Instance?.PlayerJoin(player);
        }
        Debug.Log($"�÷��̾� ����: {player}");
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
        Debug.Log($"�÷��̾� ����: {player}");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(_inputHandler.GetNetworkInput());
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"��Ʈ��ũ �˴ٿ�: {shutdownReason}");
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("������ �����");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"�������� ������ ����: {reason}");
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log($"���� ����: {reason}");
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
        Debug.Log("�� �ε� �Ϸ�");
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("�� �ε� ����");
    }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"������Ʈ�� AOI�� ���: {obj.Id}");
    }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        Debug.Log($"������Ʈ�� AOI�� ����: {obj.Id}");
    }
}