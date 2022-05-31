using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using EventCodes;
using ExitGames.Client.Photon;


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

    // Hero selection before the match
    public GameObject mainCamera;
    public GameObject canvasCamera;
    public GameObject menuCanvas;
    public GameObject heroCanvas;
    public TextMeshProUGUI countdownText;
    public GameObject exitButton;

    //quick-match
    RoomManager roomManager;
    bool roomJoined;
    bool starting;
    float countdown;
    
    // Matchmaking timers
    private float matchmakingTimer;
    private float resetTimer;
    private float heroSelectionTimer;
    private bool canStart = false;

    /*
    // Loading Bar stuff
    int clientsReady = 0;
    //The previous panel of the loading screen in oreder to hide it
    public GameObject previousLoadingScreen;
    // The panel to be loaded where it is a loading screen with the slider
    public GameObject loadingScreen;
    // The slider object in order to perform changes in its status
    public Slider slider;
    public TextMeshProUGUI loadingScreenText;
    */

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

        matchmakingTimer = roomManager.MatchMakingTimer();
        resetTimer = roomManager.MatchMakingTimer();
        heroSelectionTimer = roomManager.MatchMakingTimer();
    }

    void Start()
    {
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
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

        //add this class for EventsHandler and IPunOwnershipCallbacks
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, gameStatus.fullScreen);
        if (!loading)
        {
            gameStatus.resolutionIndex = resolutionIndex;
            SaveSystem.Save();
        }
    }

    public void SetFullscreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        if (!loading)
        {
            gameStatus.fullScreen = isFullScreen;
            SaveSystem.Save();
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        if (!loading)
        {
            gameStatus.qualityIndex = qualityIndex;
            SaveSystem.Save();
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        if (!loading)
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

    // ReSharper disable Unity.PerformanceAnalysis
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
        if (roomJoined)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= (byte)roomManager.MinPlayersNumber() && canStart)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (heroSelectionTimer <= 0)
                    {
                        DisableHeroCanvas();
                        SendMessage(1);

                        SendMessage("MATCH STARTS IN: " + (int)countdown + " s.", Codes.TIMER);
                        countdown -= Time.deltaTime;
                    }
                    else
                    {
                        ShowHeroConvas();
                        SendMessage(0);

                        SendMessage("HERO SELECTION END IN " + (int)heroSelectionTimer + " s.", Codes.HEROTIMER);
                        heroSelectionTimer -= Time.deltaTime;
                    }
                }

                if (countdown <= 0f)
                {
                    if (starting == false && PhotonNetwork.IsMasterClient)
                    {
                        starting = true;
                        StartMatch();
                    }
                }
            }
            else
            {
                if (matchmakingTimer <= 0f)
                {
                    if (resetTimer <= 0)
                    {
                        matchmakingTimer = roomManager.MatchMakingTimer();
                        resetTimer = roomManager.ResetTimer();
                    }
                    else if (PhotonNetwork.CurrentRoom.PlayerCount >= (byte)roomManager.MinPlayersNumber())
                    {
                        canStart = true;
                    }
                    else if (PhotonNetwork.IsMasterClient)
                    {
                        SendMessage("NOT ENOUGH PLAYER FOUND, RESTARTING TIMER...", Codes.TIMER);
                        resetTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        SendMessage("WAITING FOR OTHER PLAYERS..." + "\n" + (int)matchmakingTimer + " s.", Codes.TIMER);
                        matchmakingTimer -= Time.deltaTime;
                    }
                }

                // countdown = roomManager.Countdown();
            }
        }
    }

    private void ShowHeroConvas()
    {
        menuCanvas.GetComponent<Canvas>().enabled = false;
        mainCamera.SetActive(false);

        heroCanvas.GetComponent<Canvas>().enabled = true;
        canvasCamera.SetActive(true);

        // Disable return to main menu button
        exitButton.SetActive(false);

        // Activate hero selection countdown text
        countdownText.enabled = true;
    }

    private void DisableHeroCanvas()
    {
        menuCanvas.GetComponent<Canvas>().enabled = true;
        mainCamera.SetActive(true);
        
        GameObject backButton = GameObject.Find("Back Button");
        backButton.GetComponent<Image>().enabled = false;
        backButton.GetComponent<Button>().enabled = false;
        backButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().enabled = false;
        
        heroCanvas.GetComponent<Canvas>().enabled = false;
        canvasCamera.SetActive(false);

        // Disable hero selection countdown text
        countdownText.enabled = false;
    }

    private void SendMessage(int choose)
    {
        object[] data = { choose };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(Codes.CANVASSWITCH, data, raiseEventOptions, sendOptions);
    }

    private void SendMessage(string text, byte message)
    {
        if (message == Codes.HEROTIMER)
        {
            countdownText.text = text;
        }
        else
        {
            roomText.text = text;
        }

        object[] data = { text };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(message, data, raiseEventOptions, sendOptions);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void StartMatch()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;

        if (PhotonNetwork.CurrentRoom.PlayerCount > 8)
        {
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            PhotonNetwork.LoadLevel(2);
        }

        // Hiding previous panel of loading screen
        // previousLoadingScreen.SetActive(false);

        // Set the loadingScreen canvas visible
        // loadingScreen.SetActive(true);  
    }

    public void ExitRoom()
    {
        matchmakingTimer = roomManager.MatchMakingTimer();
        resetTimer = roomManager.ResetTimer();
        heroSelectionTimer = roomManager.HeroSelectionTimer();
        canStart = false;

        roomJoined = false;
        starting = false;
        countdown = roomManager.Countdown();
        PhotonNetwork.Disconnect();
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
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

    ////////////////////
    // Events handler // 
    ////////////////////

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case Codes.TIMER:
                object[] data0 = (object[])eventData.CustomData;
                roomText.text = (string)data0[0];
                break;

            case Codes.HEROTIMER:
                object[] data2 = (object[])eventData.CustomData;
                countdownText.text = (string)data2[0];
                break;

            case Codes.CANVASSWITCH:
                object[] data1 = (object[])eventData.CustomData;
                if ((int)data1[0] == 0)
                {
                    ShowHeroConvas();
                }
                else
                {
                    DisableHeroCanvas();
                }

                break;
        }
    }
}