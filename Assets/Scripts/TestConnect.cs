using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class TestConnect : MonoBehaviourPunCallbacks
{
    public Text text; //debug

    MasterManager masterManager;

    void Awake()
    {
        Application.targetFrameRate = 60;
        masterManager = gameObject.GetComponent<MasterManager>();
    }

    void Start()
    {
        Debug.Log("Connecting to server...");
        int n = Random.Range(0, 9999);
        PhotonNetwork.NickName = masterManager.PlayerName() + "_" + n.ToString();
        PhotonNetwork.GameVersion = masterManager.GameVersion();
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

        //debug text on screen
        string toPrint = "Nickname: " + PhotonNetwork.NickName + "\n\n";
        toPrint += "Room name: " + PhotonNetwork.CurrentRoom.Name + " \n";
        toPrint += "Number of players: " + PhotonNetwork.CurrentRoom.PlayerCount + "\n";
        toPrint += "Max number of players: " + PhotonNetwork.CurrentRoom.MaxPlayers;
        text.text = toPrint;
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

    /*
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        string toPrint = "Rooms list: \n";
        Debug.Log("Rooms list:");
        foreach(RoomInfo info in roomList)
        {
            Debug.Log(info.Name.Substring(0,10) + " - players: " + info.PlayerCount + " - max: " + info.MaxPlayers);
            toPrint += info.Name.Substring(0, 10) + " - players: " + info.PlayerCount + " - max: " + info.MaxPlayers + "\n";
        }
        //text.text = toPrint;
    }*/



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
