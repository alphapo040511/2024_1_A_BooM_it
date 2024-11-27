using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class NetworkRoomLoader : MonoBehaviour
{
    public List<RoomButton> roomButtons;
    public Button JoinButton;

    public TMP_InputField roomName;
    public string _roomName;
    private int playerCount = 2;

    private List<SessionInfo> sessions;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < roomButtons.Count; i++)
        {
            roomButtons[i].selectRoom += SelectRoom;
        }

        for (int i = 0; i < roomButtons.Count; i++)
        {
            roomButtons[i].SetData(i + 1, false);
        }

        JoinLobby();
        NetworkManager.Instance.updateSessions += UpdateRooms;
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
                NetworkManager.Instance.StartGame(GameMode.Host, roomName.text, playerCount);
            }
            else
            {
                Debug.Log("동일한 이름의 방이 이미 있습니다.");
            }
        }
    }

    private bool IsRoomNameDuplicate(string targetName)
    {
        if (sessions == null) return true;

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
        if (_roomName != null)
        {
            NetworkManager.Instance.StartGame(GameMode.Client, _roomName, playerCount);
        }
    }

    public void UpdateRooms(List<SessionInfo> sessions = default)
    {
        this.sessions = sessions;
        int count = Mathf.Min(sessions.Count, roomButtons.Count);
        string temp = "";
        for(int i = 0; i < roomButtons.Count; i++)
        {
            if(i < count)
            {
                roomButtons[i].SetData(i + 1, true, sessions[i]);
                if (sessions[i].Name == _roomName)
                {
                    temp = _roomName;
                }
            }
            else
            {
                roomButtons[i].SetData(i + 1, false);
            }
        }

        _roomName = temp;
        JoinButton.interactable = (_roomName != "");
    }
    private void SelectRoom(SessionInfo info)
    {
        _roomName = info.Name;
        playerCount = info.MaxPlayers;

        JoinButton.interactable = (_roomName != "");
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
        await NetworkManager.Instance.JoinLobby();
    }

  
}
