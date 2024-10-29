using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NetworkRoomLoader : MonoBehaviour
{
    public TMP_InputField roomName;
    private string room;

    private NetworkManager _networkManager;
    private NetworkRunner runner;

    // Start is called before the first frame update
    void Start()
    {
        _networkManager = NetworkManager.Instance;   
        if( _networkManager._runner == null )
        {
            runner = _networkManager.NewRunner();
        }
        else
        {
            runner = _networkManager._runner;
        }
        JoinLobby();
        _networkManager.updateSessions += UpdateRooms;
    }

    public void CreateRoom()
    {
        _networkManager.StartGame(GameMode.Host, roomName.text);
    }

    public void JoinGame()
    {
        _networkManager.StartGame(GameMode.Client, "test");
    }

    public void UpdateRooms(List<SessionInfo> sessions)
    {
        if (sessions.Count > 0)
        {
            room = sessions[0].Name;
            Debug.Log(room);
        }
    }

    public async void JoinLobby()
    {
        await _networkManager.JoinLobby();
    }
}
