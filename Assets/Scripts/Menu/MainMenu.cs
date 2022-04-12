using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class MainMenu : MonoBehaviourPunCallbacks
{
    public Dropdown resolutionDropdown;
    public Toggle fullScreenToggle;
    public AudioMixer audioMixer;
    public Dropdown qualityDropdown;
    public Slider volumeSlider;
    public TextMeshProUGUI roomText;

    bool loading;
    GameStatus gameStatus;
    Resolution[] resolutions;

    //quick-match
    RoomManager roomManager;
    bool roomJoined;
    bool starting;
    float countdown;

    void Awake()
    {
        Application.targetFrameRate = 60;

        loading = true;
        SaveSystem.Load();

        gameStatus = GameObject.FindWithTag("GameController").GetComponent<GameStatus>();
        resolutions = Screen.resolutions;

        roomManager = GameObject.FindWithTag("GameController").GetComponent<RoomManager>();
        roomJoined = false;
        starting = false;
        countdown = roomManager.Countdown();
    }

    void Start()
    {
        List<string> options = new List<string>();
        for(int i=0; i<resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = gameStatus.resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullScreenToggle.isOn = gameStatus.fullScreen;

        qualityDropdown.value = gameStatus.qualityIndex;
        qualityDropdown.RefreshShownValue();

        volumeSlider.value = gameStatus.volume;

        loading = false;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, gameStatus.fullScreen);
        if(!loading)
        {
            gameStatus.resolutionIndex = resolutionIndex;
            SaveSystem.Save();
        }
    }

    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        if(!loading)
        {
            gameStatus.fullScreen = isFullScreen;
            SaveSystem.Save();
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        if(!loading)
        {
            gameStatus.qualityIndex = qualityIndex;
            SaveSystem.Save();
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        if(!loading)
        {
            gameStatus.volume = volume;
            SaveSystem.Save();
        }
    }

    //Quickmatch functions
    public void PlayGame()
    {
        Debug.Log("Connecting to server...");
        int n = Random.Range(0, 9999);
        PhotonNetwork.NickName = gameStatus.username + "_" + n.ToString();
        PhotonNetwork.GameVersion = roomManager.GameVersion();
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
        roomOptions.MaxPlayers = (byte)roomManager.MaxPlayersNumber();
        string roomName = PhotonNetwork.NickName + "_room";
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    void Update()
    {
        if(roomJoined)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount >= (byte)roomManager.MinPlayersNumber())
            {
                roomText.text = "MATCH STARTS IN: " + (int)countdown + " s.";
                countdown -= Time.deltaTime;
                if(countdown <= 0f)
                {
                    if(starting == false && PhotonNetwork.IsMasterClient)
                    {
                        starting = true;
                        StartMatch();
                    }
                }
            }
            else
            {
                roomText.text = "WAITING FOR OTHER PLAYERS...";
                countdown = roomManager.Countdown();
            }
        }
    }

    private void StartMatch()
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


    public void QuitGame ()
    {
        Debug.Log ("QUIT");
        Application.Quit();
    }
    
}
