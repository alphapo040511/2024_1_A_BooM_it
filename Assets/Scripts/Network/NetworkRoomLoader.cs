using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class NetworkRoomLoader : MonoBehaviour
{
    public Transform roomListContainer;
    public RoomButton roomButtonPrefab;

    public Button JoinButton;
    public FollowImage followImage;

    public TMP_InputField roomName;
    public TMP_InputField searchText;
    public string _roomName;
    private int playerCount = 2;

    private List<SessionInfo> sessions;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            GenerateNoneDataButton(i + 1);
        }
        JoinLobby();
        NetworkManager.Instance.updateSessions += UpdateRooms;
    }

    public void InputName(TMP_InputField input)
    {
        if (input.text.Length > 12)
        {
            input.text = input.text.Substring(0, 12);
        }

        if(input.text.Contains(" "))
        {
            input.text = input.text.Replace(" ", "");
        }

        UpdateRooms(sessions);
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
        int createCount = 0;
        string temp = "";

        if (roomListContainer != null)
        {
            if (roomListContainer.childCount > 0)
            {
                foreach (Transform Obj in roomListContainer)
                {
                    if (Obj != null)
                    {
                        Destroy(Obj.gameObject);
                    }
                }
            }
        }

        for(int i = 0; i < sessions.Count; i++)
        {

            if (searchText.text == "" || sessions[i].Name.Contains(searchText.text) == true)
            {
                createCount++;
                RoomButton button = Instantiate(roomButtonPrefab, roomListContainer);
                button.SetData(i + 1, true, sessions[i]);
                EventTrigger.Entry entry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                entry.callback.AddListener((evenetData) => followImage.Follow(button.GetComponent<RectTransform>()));
                button.GetComponent<EventTrigger>().triggers.Add(entry);
                button.selectRoom += SelectRoom;
            }
        }


        for (int i = createCount; i < 5; i++)
        {
            GenerateNoneDataButton(i + 1);
        }


        _roomName = temp;
        JoinButton.interactable = (_roomName != "");
    }

    private void GenerateNoneDataButton(int i)
    {
        RoomButton button = Instantiate(roomButtonPrefab, roomListContainer);
        button.SetData(i, false);
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((evenetData) => followImage.Follow(button.GetComponent<RectTransform>()));
        button.GetComponent<EventTrigger>().triggers.Add(entry);
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
