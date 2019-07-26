using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Launcher : Photon.PunBehaviour
{

    #region Public Variables

    public GameObject connectUI;
    public GameObject lobbyUI;
    public GameObject roomUI;

    //Layout Groups
    public GameObject roomContent;
    public GameObject roomButtonPrefab;
    public GameObject playerContent;
    public GameObject playerTextPrefab;
    public GameObject startGameButton;

    public Text roomText, nameText;

    [Tooltip("The maximum number of players per room")]
    public byte maxPlayersPerRoom = 4;

    #endregion

    #region Private Variables

    string _gameVersion = "1";

    #endregion

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        PhotonNetwork.autoJoinLobby = false;

        PhotonNetwork.automaticallySyncScene = true;

        connectUI.SetActive(true);
        lobbyUI.SetActive(false);
        roomUI.SetActive(false);
    }

    #endregion


    #region Public Methods

    public void Connect()
    {
        if (!PhotonNetwork.connected)
        {
            print("Connecting to Server..");
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public void CreateRoom()
    {
        if (PhotonNetwork.JoinOrCreateRoom(roomText.text, new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = this.maxPlayersPerRoom }, TypedLobby.Default))
        {
            print("Room created");
        }

    }

    public void JoinRoom(){
        if (PhotonNetwork.JoinRoom(roomText.text)){
            print("Joined Room: " + roomText.text);
        }
    }

    public void RefreshRoomList()
    {
        GenerateRoomList();
    }

    public void StartGame(){
        if(PhotonNetwork.isMasterClient){
            PhotonNetwork.LoadLevel("Game");
        }
    }

    #endregion

    #region Private Methods

    private void GenerateRoomList()
    {
        foreach (Transform child in roomContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (RoomInfo room in PhotonNetwork.GetRoomList())
        {
            GameObject newRoom = Instantiate(roomButtonPrefab, roomContent.transform);
            string roomName = room.Name;
            newRoom.GetComponentInChildren<Text>().text = "Join: " + roomName;
            newRoom.GetComponent<Button>().onClick.AddListener(delegate { JoinRoomButton(roomName); });
        }
    }

    private void GeneratePlayerList()
    {
        foreach (Transform child in playerContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            GameObject newPlayer = Instantiate(playerTextPrefab, playerContent.transform);
            string playerName = player.NickName;
            newPlayer.GetComponentInChildren<Text>().text = "Player: " + playerName;
        }
    }

    private void JoinRoomButton(string roomName)
    {
        if (PhotonNetwork.JoinRoom(roomName)){
            print("Joined Room " + roomName);
        }
    }

    #endregion


    #region Photon.PunBehaviour CallBacks

    public override void OnConnectedToMaster()
    {
        print("Connected to Master");
        PhotonNetwork.playerName = nameText.text;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        print("Joined Lobby");
        connectUI.SetActive(false);
        lobbyUI.SetActive(true);
    }

    public override void OnLobbyStatisticsUpdate()
    {
        GenerateRoomList();
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        GeneratePlayerList();
    }


    public override void OnDisconnectedFromPhoton()
    {
        connectUI.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        print("Joined Room " + PhotonNetwork.room.Name);
        lobbyUI.SetActive(false);
        roomUI.SetActive(true);
        GeneratePlayerList();
        startGameButton.SetActive(PhotonNetwork.isMasterClient);
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        print("Join Room Failed");
    }

    public override void OnReceivedRoomListUpdate()
    {
        GenerateRoomList();
    }

    #endregion
}