using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class NetworkRoomLoader : MonoBehaviour
{
    public List<RoomButton> roomButtons;

    public TMP_InputField roomName;
    private string _roomName;
    private int playerCount = 2;

    private NetworkManager _networkManager;
    private NetworkRunner runner;

    private List<SessionInfo> sessions;

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

        for (int i = 0; i < roomButtons.Count; i++)
        {
            roomButtons[i].selectRoom += SelectRoom;
        }

        JoinLobby();
        _networkManager.updateSessions += UpdateRooms;
    }

    public void InputName()
    {
        if(roomName.text.Contains(" "))
        {
            roomName.text = roomName.text.Replace(" ", "");
        }
    }

    public void CreateRoom()
    {
        if (roomName.text !=  null)
        {
            if (IsRoomNameDuplicate(roomName.text))
            {
                _networkManager.StartGame(GameMode.Host, roomName.text, playerCount);
            }
            else
            {
                Debug.Log("동일한 이름의 방이 이미 있습니다.");
            }
        }
    }

    private bool IsRoomNameDuplicate(string targetName)
    {
        foreach(var info in sessions)
        {
            if(info.Name == targetName)
            {
                return false;
            }
        }

        return true;
    }

    public void JoinGame()
    {
        _networkManager.StartGame(GameMode.Client, _roomName, playerCount);
    }

    public void UpdateRooms(List<SessionInfo> sessions = null)
    {
        this.sessions = sessions;
        int count = Mathf.Min(sessions.Count, roomButtons.Count);
        for(int i = 0; i < roomButtons.Count; i++)
        {
            if(i < count)
            {
                roomButtons[i].SetData(i + 1, sessions[i]);
            }
            else
            {
                roomButtons[i].SetData(i + 1);
            }
        }    
    }
    private void SelectRoom(SessionInfo info)
    {
        _roomName = info.Name;
        playerCount = info.MaxPlayers;
    }

    public void OnClickPlayerCount(int count)
    {
        if(count == 0)
        {
            //팀전
            playerCount = 4;
        }
        else
        {
            playerCount = count;
        }

        playerCount = Mathf.Clamp(playerCount, 2, 4);
    }

    private async void JoinLobby()
    {
        await _networkManager.JoinLobby();
    }

  
}
