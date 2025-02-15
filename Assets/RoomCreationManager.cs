using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RoomCreationManager : MonoBehaviourPunCallbacks
{


    private int requiredPlayers = 0; // Number of players required to proceed.

    public Button ConnectBtn;
    public Button CloseBtn;
    public Button BotBtn;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI SearchingText;
    public Text MultiText;

    public static RoomCreationManager data;
    public GameObject Loadingpanel;
    public GameObject RoomWaitPanel;
    public TextMeshProUGUI RoomWaitText;

    public List<RoomInfo> roomList = new List<RoomInfo>();
    public string MapName;
    public bool isLobbyJoined;
    public void Awake()
    {
        data = this;
    }
    public void Start()
    {
        MapName = PlayerPrefs.GetInt("Stage").ToString();

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1.0";
        }
    }
    // Called when connected to the Photon server
    public override void OnConnectedToMaster()
    {

        JoinLobby(MapName, LobbyType.Default);
        Debug.Log("Connected to Photon Master Server");
        //LoadingScreen.SetActive(false);
        ConnectBtn.interactable = true;
        Loadingpanel.SetActive(false);
        ConnectBtn.gameObject.SetActive(true);

    }


    public void JoinLobby(string lobbyName, LobbyType lobbyType)
    {
        isLobbyJoined = true;
        TypedLobby selectedLobby = new TypedLobby(lobbyName, lobbyType);
        PhotonNetwork.JoinLobby(selectedLobby);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"Joined Custom Lobby: {PhotonNetwork.CurrentLobby.Name}");
    }

    // Create a room
    public void CreateRoom()
    {
        requiredPlayers = 2; // Set this to the desired number of players for the room
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)requiredPlayers

        };
        roomOptions.IsVisible = true; // Room is visible in the lobby
        roomOptions.IsOpen = true;
        PhotonNetwork.CreateRoom("Room" + Random.Range(1000, 9999), roomOptions);

    }

    public void TryJoinRandomRoom()
    {
        if (isLobbyJoined)
        {
            JoinRoom();
        }
        else
        {
            PhotonNetwork.LeaveLobby();
            Debug.LogWarning("Not in lobby yet. Trying again in 2 seconds...");
            JoinLobby(MapName, LobbyType.Default);
            Invoke(nameof(JoinRoom), 2f); // Retry after 2 seconds
        }
    }
    // Join a random room
    public void JoinRoom()
    {
        ///JoinLobby(MapName, LobbyType.Default);
        PhotonNetwork.NickName = PlayerName.text;
        RoomWaitPanel.SetActive(true);
        OnlyData.Data.Playername = PlayerName.text;
        PhotonNetwork.JoinRandomRoom();
        BotBtn.interactable = false;
        ConnectBtn.interactable = false;
        MultiText.gameObject.SetActive(false);
        SearchingText.gameObject.SetActive(true);
        Debug.Log("__ROOM JOIN");

    }

    // Called when the room is successfully created
    public override void OnCreatedRoom()
    {
        Debug.Log("__ROOM CREATE__");
        CloseBtn.gameObject.SetActive(true);
    }

    // Called when a player joins the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomWaitText.text = $"Player {newPlayer.NickName} joined. Players in room: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        Debug.Log($"Player {newPlayer.NickName} joined. Players in room: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        // Check if all players are in the room
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            LoadGameScene();
        }
    }

    // Called when the local player successfully joins a room
    public override void OnJoinedRoom()
    {
        RoomWaitText.text = $"Joined room: {PhotonNetwork.CurrentRoom.Name}. Players in room: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}. Players in room: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        // Check if the room is full
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            RoomWaitPanel.SetActive(false);
            LoadGameScene();
        }
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

    // Called if joining a random room fails
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("__JOIN ROOM FAIL");
        CreateRoom();
        //ConnectBtn.interactable = true;
    }

    // Load the game scene
    private void LoadGameScene()
    {
        Debug.Log("All players have joined. Loading the game scene...");
        Debug.Log("AllPlayerEnter");
        PhotonNetwork.LoadLevel(1);

        //SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }

    public void MultiLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        BotBtn.interactable = true;
        MultiText.gameObject.SetActive(true);
        SearchingText.gameObject.SetActive(false);
        CloseBtn.gameObject.SetActive(false);
        ConnectBtn.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
        RoomWaitPanel.SetActive(false);

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Debug.Log("Remaining Players: " + PhotonNetwork.CurrentRoom.PlayerCount);
        RoomWaitText.text = $"Player Left: {otherPlayer.NickName} . Players in room: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        Debug.Log($"Player Left: {otherPlayer.NickName} (ID: {otherPlayer.ActorNumber})");
    }
}
