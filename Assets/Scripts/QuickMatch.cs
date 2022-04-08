using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;


public class QuickMatch : MonoBehaviourPunCallbacks
{
    public Text text; //debug

    MasterManager masterManager;
    bool roomJoined;
    bool starting;
    float countdown;

    void Awake()
    {
        Application.targetFrameRate = 60;

        masterManager = gameObject.GetComponent<MasterManager>();
        roomJoined = false;
        starting = false;
        countdown = masterManager.Countdown();
    }

    void Start()
    {
        Debug.Log("Connecting to server...");
        int n = Random.Range(0, 9999);
        PhotonNetwork.NickName = masterManager.PlayerName() + "_" + n.ToString();
        PhotonNetwork.GameVersion = masterManager.GameVersion();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server");
        Debug.Log("Connected as: " + PhotonNetwork.LocalPlayer.NickName);

        Debug.Log("Joining lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby joined");

        Debug.Log("Joining a room...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room joined");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log(PhotonNetwork.CurrentRoom.MaxPlayers);
        roomJoined = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Not available/empty room");

        Debug.Log("Room creation...");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)masterManager.MaxPlayersNumber();
        string roomName = PhotonNetwork.NickName + "_room";
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    void Update()
    {
        if(roomJoined)
        {
            //debug text on screen
            string toPrint = "Nickname: " + PhotonNetwork.NickName + "\n";
            toPrint += "Host: " + PhotonNetwork.IsMasterClient + "\n\n";

            toPrint += "Room name: " + PhotonNetwork.CurrentRoom.Name + " \n";
            toPrint += "Number of players: " + PhotonNetwork.CurrentRoom.PlayerCount + "\n";
            toPrint += "Max number of players: " + PhotonNetwork.CurrentRoom.MaxPlayers + "\n\n";

            toPrint += "Player in room:\n";
            Dictionary<int, Player> dictionary = PhotonNetwork.CurrentRoom.Players;
            foreach (Player p in dictionary.Values)
            {
                toPrint += "- " + p.NickName + "\n";
            }

            if (countdown < masterManager.Countdown() && countdown >= 0f)
            {
                toPrint += "\nCountdown: " + (int)countdown;
            }

            text.text = toPrint;


            if(PhotonNetwork.CurrentRoom.PlayerCount >= (byte)masterManager.MinPlayersNumber())
            {
                countdown -= Time.deltaTime;
                if(countdown <= 0f)
                {
                    if (starting == false && PhotonNetwork.IsMasterClient)
                    {
                        starting = true;
                        StartGame();
                    }
                }
            }
            else
            {
                countdown = masterManager.Countdown();
            }
        }
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(1);
    }


    //errors
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server: " + cause.ToString());
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room creation failed");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room joining failed");
    }

}
