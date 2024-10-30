using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RoomButton : MonoBehaviour
{
    public TextMeshProUGUI roomNumber;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI playerCount;

    public event Action<SessionInfo> selectRoom;

    private bool dataExists = false;
    private SessionInfo sessionInfo = default;


    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickButton);
    }

    public void SetData(int number, bool dataExists, SessionInfo newSession = default)
    {
        roomNumber.text = number.ToString();
        this.dataExists = dataExists;
        if (dataExists)
        {
            sessionInfo = newSession;
            roomName.text = sessionInfo.Name;
            playerCount.text = $"{sessionInfo.PlayerCount}/{sessionInfo.MaxPlayers}";
        }
        else
        {
            sessionInfo = null;
            roomName.text = "";
            playerCount.text = " / ";
        }
    }

    public void OnClickButton()
    {
        if (dataExists)
        {
            Debug.Log(sessionInfo.Name + "이 선택됨");
            selectRoom(sessionInfo);
        }
        else
        {
            Debug.Log("현제 데이터가 없습니다.");
        }
    }
}
