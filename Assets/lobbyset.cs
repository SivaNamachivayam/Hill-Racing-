using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class lobbyset : MonoBehaviourPunCallbacks
{
    public string Test;
    public static string selectedMap;
    public List<RoomInfo> roomList = new List<RoomInfo>();

    private void Start()
    {
        selectedMap = Test;
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public void JoinLobby(string lobbyName, LobbyType lobbyType)
    {
        TypedLobby selectedLobby = new TypedLobby(lobbyName, lobbyType);
        PhotonNetwork.JoinLobby(selectedLobby);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon! Joining custom lobby...");
        JoinLobby(Test,LobbyType.Default);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"Joined Custom Lobby: {PhotonNetwork.CurrentLobby.Name}");
    }

    public void CreateRoomInCustomLobby()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom("Room" + Random.Range(1000, 9999), roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("CreateRoom");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.roomList = roomList;
        Debug.Log($"Updated Room List in {PhotonNetwork.CurrentLobby.Name}:{roomList.Count}");

        foreach (RoomInfo room in roomList)
        {
            Debug.Log($"Room: {room.Name} | Players: {room.PlayerCount}/{room.MaxPlayers}");
        }
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("__JOIN ROOM FAIL");
        CreateRoomInCustomLobby();
        //ConnectBtn.interactable = true;
    }
    //public void JoinRoomWithFilter()
    //{
    //    string sqlLobbyFilter = "Map = 'Forest' AND MaxScore >= 50";
    //    PhotonNetwork.GetCustomRoomList(customLobby, sqlLobbyFilter);
    //}
}
