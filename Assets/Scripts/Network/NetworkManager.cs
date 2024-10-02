using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


// NetworkManager Ŭ����
public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private NetworkRunner _runner;
    private NetworkInputHandler _inputHandler;
    private string _roomName = "TestRoom"; // �⺻ �� �̸�
    [SerializeField] private string _gameSceneName = "GameScene"; // ���� �� �̸�
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private void Awake()
    {
        _inputHandler = gameObject.AddComponent<NetworkInputHandler>();
    }

    private void OnGUI()
    {
        if (_runner == null)
        {
            // �� �̸� �Է� �ʵ�
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            GUILayout.Label("�� ��ȣ:");
            _roomName = GUILayout.TextField(_roomName);

            // ȣ��Ʈ ��ư
            if (GUILayout.Button("ȣ��Ʈ"))
            {
                StartGame(GameMode.Host);
            }
            // ���� ��ư
            if (GUILayout.Button("����"))
            {
                StartGame(GameMode.Client);
            }
            GUILayout.EndArea();
        }
    }

    async void StartGame(GameMode mode)
    {
        // Fusion ���ʸ� �����ϰ� ����� �Է��� ������ ������ �˸�
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

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
            SessionName = _roomName, // ����ڰ� �Է��� �� �̸� ���
            Scene = gameScene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // �÷��̾� ���� ��ġ ����� ���� ���ο� ����
            Vector3 spawnPosition = GetNextSpawnPosition();
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
        Debug.Log($"�÷��̾� ����: {player}");
    }

    private Vector3 GetNextSpawnPosition()
    {
        // ������ ��: �÷��̾� ���� ���� �������� ��ġ
        float angle = _spawnedCharacters.Count * (360f / 8); // �ִ� 8���� �÷��̾� ����
        float radius = 5f; // ���� �ݰ�
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        return new Vector3(x, 2, z);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
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
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
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