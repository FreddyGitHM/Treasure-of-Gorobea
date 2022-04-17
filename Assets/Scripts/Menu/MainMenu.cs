using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


using ExitGames.Client.Photon;
using EventCodes;


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

    // Loading Bar stuff
    int clientsReady = 0;
    // The scene to be loaded
    public string nextScene;
    //The previous panel of the loading screen in oreder to hide it
    public GameObject previousLoadingScreen;
    // The panel to be loaded where it is a loading screen with the slider
    public GameObject loadingScreen;
    // The slider object in order to perform changes in its status
    public Slider slider;
    public TextMeshProUGUI loadingScreenText;
    

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
        
        PhotonNetwork.AddCallbackTarget(this);
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
                // roomText.text = "MATCH STARTS IN: " + (int)countdown + " s.";
                // countdown -= Time.deltaTime;
                // if(countdown <= 0f)
                // {
                //     if(starting == false && PhotonNetwork.IsMasterClient)
                //     {
                //         starting = true;
                //         StartMatch();
                //     }
                // }

                if(starting == false && PhotonNetwork.IsMasterClient)
                {
                    starting = true;
                    StartMatch();
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

        PhotonNetwork.LoadLevel(nextScene);

        PhotonNetwork.AsyncLevelLoading.allowSceneActivation = false;
        
        // Hiding previous panel of loading screen
        previousLoadingScreen.SetActive(false);

        // Set the loadingScreen canvas visible
        loadingScreen.SetActive(true);  

        StartCoroutine(LoadingLevel());    
    }

    private IEnumerator LoadingLevel(){

        bool isDone = PhotonNetwork.AsyncLevelLoading.isDone;

        while(!isDone){
            // In Unity the loading process is misuread with a float number between 0 and 0.9, in order to have it in the range [0, 1] it is calculated the progress value 
            float progress = Mathf.Clamp(PhotonNetwork.AsyncLevelLoading.progress, 0, .8f);

            Debug.Log("Progress level: " + PhotonNetwork.AsyncLevelLoading.progress  +  " Clamp progress" + progress);

            // This is a placeholder if in order to see the loading bar charging a bit over time. It is needed for the scenes that are too simple to be charged, otherwise the loading process will be too 
            // fast and it is not possible to see it. When we will load a real scene, heavy enough, it is not needed because the process will take a time and during this the slider bar will be updated as well
            if(PhotonNetwork.AsyncLevelLoading.progress == 0.9f){
                while(slider.value < progress){
                    slider.value += 0.1f;
                    yield return new WaitForSecondsRealtime(.2f);
                }

                isDone = true;
            }

            // Setting the slider value to the process value, in order to have an animation of the slider and the loading process
            // slider.value = progress;
        }

        if(PhotonNetwork.IsMasterClient){
            StartCoroutine(WaitingForClients());
        }
        else{
            // Invio messaggio al master
            object[] data = new object[] { }; 

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
            raiseEventOptions.Receivers = ReceiverGroup.MasterClient;
            raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

            SendOptions sendOptions = new SendOptions();
            sendOptions.Reliability = true;

            PhotonNetwork.RaiseEvent(Codes.CLIENTREADY, data, raiseEventOptions, sendOptions);  
        }
    }

    private IEnumerator WaitingForClients(){
        loadingScreenText.text = "WAITING FOR CLIENTS BE READY...";

        yield return new WaitUntil( () => clientsReady == PhotonNetwork.CurrentRoom.PlayerCount - 1);

        object[] data = new object[] { }; 

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        while(slider.value < 1){
            slider.value += 0.1f;
            yield return new WaitForSecondsRealtime(.2f);
        }

        Debug.Log("Send scene activation code");
        PhotonNetwork.RaiseEvent(Codes.SCENEACTIVATION, data, raiseEventOptions, sendOptions);
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData eventData){
        Debug.Log("Event Data: " + eventData);

        if(eventData.Code == Codes.CLIENTREADY){
            clientsReady ++;
        }
        else if(eventData.Code == Codes.SCENEACTIVATION){
            PhotonNetwork.AsyncLevelLoading.allowSceneActivation = true;
        }
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
