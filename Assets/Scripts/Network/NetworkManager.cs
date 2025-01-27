using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;
using Invector.vCharacterController;
using Invector.vShooter;
using Invector.vItemManager;
using Invector.vMelee;
using Invector.vCharacterController.vActions;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Invector;


public class NetworkManager : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    public int xpKill;
    public int xpMap;
    public int xpChest;
    public float minDistance;
    public float spotTime;

    GameObject player; //local player
    public GameObject MapTree; // MapTree object
    public GameObject MapCamera;
    GameObject TreasureChest;
    bool instantiated;
    bool ready;
    GameObject mainCamera;
    Vector3 playerPos;
    float distanceCovered;
    GameObject damageImage;

    //match info
    TextMeshProUGUI timeText;
    TextMeshProUGUI playerText;
    TextMeshProUGUI killText;
    int matchLength;
    float timeLeft;
    List<int> playersIdList;
    int playerIdLastShot;
    int kills;
    List<int> playersKilledList;

    //kill canvas
    TextMeshProUGUI killMsgText;
    float killMessageTimer;
    string killedPlayer;
    string killedByText;
    string killerPlayer;
    bool endGameMenuLoaded;

    //endgame info
    GameObject endGameCanvas;
    bool matchEnded;
    bool mapTaken;
    bool chestOpened;
    float exitCountdown;

    //fps counter
    public bool showFps;
    TextMeshProUGUI fpsText;

    GameObject pauseCanvas;

    bool showHeadshotCanvas;
    
    private GameObject TreasureMapSprite;
    private Vector3 SpriteTreasureZonePosition;
    public static float RandomX;
    public static float RandomY;

    void Awake()
    {
        instantiated = false;
        ready = false;
        mainCamera = GameObject.FindWithTag("MainCamera");
        distanceCovered = 0f;
        playersIdList = new List<int>();

        killMsgText = GameObject.Find("KillCanvas/Text").GetComponent<TextMeshProUGUI>();
        killMessageTimer = 5f;
        killedPlayer = "";
        killedByText = " killed by ";
        killerPlayer = "";
        endGameMenuLoaded = false;
        playersKilledList = new List<int>();

        endGameCanvas = GameObject.FindWithTag("EndGameCanvas");
        matchEnded = false;
        mapTaken = false;
        chestOpened = false;
        exitCountdown = 35f;

        fpsText = GameObject.Find("FPSCanvas/Text").GetComponent<TextMeshProUGUI>();

        pauseCanvas = GameObject.FindWithTag("PauseCanvas");

        showHeadshotCanvas = false;
    }

    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I'm the master");
        }
        else
        {
            Debug.Log("I'm a client");
        }

        //initialize players list
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            playersIdList.Add(p.ActorNumber);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 TreeMapPosition = RandomTreeMapGenerator.Instance.spawnTreeMap();

            // Spawning the treasure chest
            Vector3 TreasureChestPosition = TreasureSpawn.Instance.getTreasurePosition();
            Quaternion TreasureChestRotation = TreasureSpawn.Instance.getTreasureRotation();
            TreasureChest = PhotonNetwork.InstantiateRoomObject("TreasureChest", TreasureChestPosition, TreasureChestRotation);

            SpawnPosition.Instance.calculateSpawnPositions(PhotonNetwork.CurrentRoom.PlayerCount);
            
            // Sprite treasure zone position
            TreasureMapSprite = GameObject.Find("Map Treasure Sprite");
            Vector3 SpriteSize = TreasureMapSprite.transform.localScale * .5f;
            RandomX = Random.Range(-SpriteSize.x, SpriteSize.x);
            RandomY = Random.Range(-SpriteSize.z, SpriteSize.z);
            SpriteTreasureZonePosition = new Vector3(TreasureChestPosition.x + RandomX, GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>().terrainData.size.y - 10, TreasureChestPosition.z + RandomY);

            //send random position for players
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                //set here the spawn position
                Vector3 spawnPos = SpawnPosition.Instance.getSpawnPosition();

                // getting random rotation for hero
                Quaternion spawnRot = SpawnPosition.Instance.getLookDirection(spawnPos);

                if (PhotonNetwork.LocalPlayer != p)
                {
                    object[] data = new object[] { spawnPos, spawnRot, TreeMapPosition, SpriteTreasureZonePosition };

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                    int[] reveivers = { p.ActorNumber };
                    raiseEventOptions.TargetActors = reveivers;
                    raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

                    SendOptions sendOptions = new SendOptions();
                    sendOptions.Reliability = true;

                    PhotonNetwork.RaiseEvent(Codes.SPAWN_POSITION, data, raiseEventOptions, sendOptions);
                }
                else
                {
                    mainCamera.GetComponent<Camera>().enabled = false;

                    player = PhotonNetwork.Instantiate(HeroManager.selectedHero, spawnPos, spawnRot);
                    playerPos = spawnPos;

                    MapTree = Instantiate(MapTree, TreeMapPosition, Quaternion.identity);
                    GameObject MinimapTreeMapSprite = MapTree.transform.Find("Minimap TreeMap Sprite").gameObject;
                    MinimapTreeMapSprite.transform.parent = MapTree.transform.parent;

                    MapCamera = Instantiate(MapCamera, MapCamera.transform.position, MapCamera.transform.rotation);
                    
                    Map.RandomTreasureZonePosition(SpriteTreasureZonePosition);
                    
                    Minimap.SetTreasurePosition(SpriteTreasureZonePosition);

                    instantiated = true;
                }
            }
        }

        //add this class for EventsHandler and IPunOwnershipCallbacks
        PhotonNetwork.AddCallbackTarget(this);
    }

    void Update()
    {
        if(instantiated == true && ready == false)
        {
            EnableComponents();
            InitMatchInfo();
            StartCoroutine(VictoryCheck());
            StartCoroutine(CampingCheck());
            ready = true;
            Debug.Log("Ready!");
        }

        if(ready && matchEnded == false)
        {
            distanceCovered += (player.transform.position - playerPos).magnitude;
            playerPos = player.transform.position;

            timeLeft -= Time.deltaTime;
            if(timeLeft <= 0)
            {
                StartCoroutine(LoadEndGameMenu("TIME UP"));
                matchEnded = true;
                timeLeft = 0f;
            }
            timeText.text = "TIME REMAINING: " + GetTimer(timeLeft);

            List<int> currentPlayersInRoom = new List<int>();
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                currentPlayersInRoom.Add(p.ActorNumber);
            }
            foreach(int id in playersIdList)
            {
                if (!currentPlayersInRoom.Contains(id))
                {
                    playersIdList.Remove(id);
                    break;
                }
            }
            playerText.text = "PLAYER(S) ALIVE: " + playersIdList.Count;

            killText.text = "KILL(S): " + kills;

            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start"))
            {
                pauseCanvas.GetComponent<PauseMenu>().Call();
            }
        }

        if(matchEnded)
        {
            exitCountdown -= Time.deltaTime;
            if(exitCountdown <= 0f)
            {
                endGameCanvas.GetComponent<EndGameMenu>().OnClick();
                exitCountdown = 0f;
            }
            endGameCanvas.transform.Find("Background/Countdown").GetComponent<TextMeshProUGUI>().text = GetTimer(exitCountdown);
        }

        if(showFps)
        {
            fpsText.text = ((int)(Time.frameCount / Time.time)).ToString();
        }
        else
        {
            fpsText.text = "";
        }

        if(showHeadshotCanvas)
        {
            StartCoroutine(ShowDamageImage());
            showHeadshotCanvas = false;
        }

        if(endGameMenuLoaded == false)
        {
            killMessageTimer -= Time.deltaTime;
            if(killMessageTimer >= 0f && !killedPlayer.Equals(""))
            {
                killMsgText.text = killedPlayer + killedByText + killerPlayer;
            }
            else
            {
                killMsgText.text = "";
            }
        }
        else
        {
            killMsgText.text = "";
        }
        
    }

    IEnumerator ShowDamageImage()
    {
        damageImage.GetComponent<Image>().enabled = true;
        yield return new WaitForSecondsRealtime(0.3f);
        damageImage.GetComponent<Image>().enabled = false;
    }

    void EnableComponents()
    {
        player.GetComponent<vShooterMeleeInput>().enabled = true;
        player.GetComponent<vThirdPersonController>().enabled = true;
        player.GetComponent<vShooterManager>().enabled = true;
        player.GetComponent<vAmmoManager>().enabled = true;
        player.GetComponent<vHeadTrack>().enabled = true;
        player.GetComponent<vCollectShooterMeleeControl>().enabled = true;
        player.GetComponent<vGenericAction>().enabled = true;
        player.GetComponent<Skills>().enabled = true;
        player.transform.Find("Invector Components/UI").gameObject.GetComponent<Canvas>().enabled = true;
        player.transform.Find("Invector Components/vThirdPersonCamera").gameObject.SetActive(true);
        player.transform.Find("Minimap/MinimapCamera").GetComponent<Camera>().enabled = true;
        player.transform.Find("Minimap/MinimapCamera").GetComponent<Minimap>().enabled = true;
        player.transform.Find("Minimap/Player Marker").GetComponent<SpriteRenderer>().enabled = true;
        player.transform.Find("Minimap/Minimap Canvas").GetComponent<Canvas>().enabled = true;
        player.transform.Find("Invector Components/AimCanvas/AimCanvas/AimID_2_AssaultRifle").GetComponent<vAimCanvas>().enabled = true;
        player.transform.Find("Invector Components/AimCanvas/AimCanvas/AimID_2_AssaultRifle/SimpleAimGroupe/AimCenter").GetComponent<Image>().enabled = true;
        player.transform.Find("Invector Components/AimCanvas/AimCanvas/AimID_2_AssaultRifle/SimpleAimGroupe/AimTarget/valid/AimCenter (1)").GetComponent<Image>().enabled = true;
        player.transform.Find("Invector Components/AimCanvas/AimCanvas/AimID_2_AssaultRifle/SimpleAimGroupe/AimTarget/valid/AimCenter (2)").GetComponent<Image>().enabled = true;
        player.transform.Find("Invector Components/AimCanvas/AimCanvas/AimID_2_AssaultRifle/SimpleAimGroupe/AimTarget/valid/AimCenter (3)").GetComponent<Image>().enabled = true;
        player.transform.Find("Invector Components/AimCanvas/AimCanvas/AimID_2_AssaultRifle/SimpleAimGroupe/AimTarget/valid/AimCenter (4)").GetComponent<Image>().enabled = true;
        player.transform.Find("Invector Components/AimCanvas/AimCanvas/AmmoDisplay").GetComponent<AmmoDisplay>().enabled = true;
        player.transform.Find("Invector Components/AimCanvas/AimCanvas/AmmoDisplay").GetComponent<AmmoDisplay>().enabled = true;
    }

    void InitMatchInfo()
    {
        GameObject infoMatch = player.transform.Find("Minimap/Minimap Canvas/Minimap/MatchInfo").gameObject;
        timeText = infoMatch.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();
        timeText.enabled = true;
        playerText = infoMatch.transform.Find("PlayerText").GetComponent<TextMeshProUGUI>();
        playerText.enabled = true;
        killText = infoMatch.transform.Find("KillText").GetComponent<TextMeshProUGUI>();
        killText.enabled = true;
        matchLength = 1200;
        timeLeft = matchLength;
        kills = 0;

        timeText.text = "TIME REMAINING: " + GetTimer(timeLeft);
        playerText.text = "PLAYER(S) ALIVE: " + playersIdList.Count;
        killText.text = "KILL(S): " + kills;

        damageImage = player.transform.Find("Invector Components/UI/HUD/damageImage").gameObject;
    }

    string GetTimer(float seconds)
    {
        int sec = (int)seconds;
        string minString = (sec / 60).ToString();
        if (minString.Length == 1)
        {
            minString = "0" + minString;
        }
        string secString = (sec % 60).ToString();
        if (secString.Length == 1)
        {
            secString = "0" + secString;
        }
        return minString + ":" + secString;
    }

    IEnumerator VictoryCheck()
    {
        while(matchEnded == false)
        {
            if(/*playersIdList.Count == 1 ||*/ chestOpened)
            {
                if(playersIdList.Count > 1)
                {
                    //tell to the others that i won
                    object[] data = new object[] { };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                    raiseEventOptions.Receivers = ReceiverGroup.Others;
                    raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

                    SendOptions sendOptions = new SendOptions();
                    sendOptions.Reliability = true;

                    PhotonNetwork.RaiseEvent(Codes.MATCH_FINISHED, data, raiseEventOptions, sendOptions);
                }

                StartCoroutine(LoadEndGameMenu("YOU WIN!"));
                matchEnded = true;
            }
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    IEnumerator LoadEndGameMenu(string text)
    {
        player.GetComponent<vShooterMeleeInput>().enabled = false;
        player.GetComponent<vThirdPersonController>().enabled = false;
        player.GetComponent<vShooterManager>().enabled = false;
        player.GetComponent<vAmmoManager>().enabled = false;
        player.GetComponent<vHeadTrack>().enabled = false;
        player.GetComponent<vCollectShooterMeleeControl>().enabled = false;
        player.GetComponent<vGenericAction>().enabled = false;
        player.GetComponent<Skills>().enabled = false;

        Animator animator = player.GetComponent<Animator>();
        animator.SetFloat("InputHorizontal", 0f);
        animator.SetFloat("InputVertical", 0f);
        animator.SetFloat("InputMagnitude", 0f);
        animator.SetFloat("RotationMagnitude", 0f);
        animator.SetBool("IsStrafing", false);
        animator.SetBool("IsSprinting", false);
        animator.SetBool("IsAiming", false);

        yield return new WaitForSecondsRealtime(5f);

        DestroyImmediate(GameObject.Find("vThirdPersonCamera"));

        endGameCanvas.transform.Find("Background/Text").GetComponent<TextMeshProUGUI>().text = text;
        endGameCanvas.transform.Find("Background/Kills").GetComponent<TextMeshProUGUI>().text = "KILL(S): " + kills;

        int xp = kills * xpKill;
        if (mapTaken)
        {
            xp += xpMap;
        }
        if (chestOpened)
        {
            xp += xpChest;
        }
        endGameCanvas.transform.Find("Background/XP").GetComponent<TextMeshProUGUI>().text = "XP: " + xp;

        GameStatus gameStatus = GameObject.FindWithTag("GameController").GetComponent<GameStatus>();
        gameStatus.xp += xp;
        endGameCanvas.transform.Find("Background/Total XP").GetComponent<TextMeshProUGUI>().text = "TOTAL XP: " + gameStatus.xp;

        SaveSystem.Save();

        endGameCanvas.GetComponent<Canvas>().enabled = true;
        mainCamera.GetComponent<Camera>().enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        endGameMenuLoaded = true;
    }

    public override void OnPlayerLeftRoom(Player disconnectedPlayer)
    {
        if(playersIdList.Contains(disconnectedPlayer.ActorNumber)) //check if the player was alive before disconnection
        {
            killedPlayer = disconnectedPlayer.NickName;
            killedByText = " disconnected";
            killerPlayer = "";
            killMessageTimer = 5f;
        }
    }

    IEnumerator CampingCheck()
    {
        while(matchEnded == false)
        {
            yield return new WaitForSecondsRealtime(10f);
            if(distanceCovered < minDistance && matchEnded == false)
            {
                //send a message to the others for showing my player in the map
                object[] data = new object[] { player.GetComponent<PhotonView>().ViewID };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                raiseEventOptions.Receivers = ReceiverGroup.Others;
                raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

                SendOptions sendOptions = new SendOptions();
                sendOptions.Reliability = true;

                PhotonNetwork.RaiseEvent(Codes.SPOT_PLAYER, data, raiseEventOptions, sendOptions);
            }
            distanceCovered = 0f;
        }
    }

    public GameObject GetPlayer()
    {
        return player;
    }
    
    public bool GetMapTaken()
    {
        return mapTaken;
    }

    public void SetMapTaken(bool b)
    {
        mapTaken = b;
    }

    public void SetChestOpened(bool b)
    {
        chestOpened = b;
    }

    public bool GetMatchEnded()
    {
        return matchEnded;
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
            //received the spawn position from masterClient
            case Codes.SPAWN_POSITION:
                mainCamera.GetComponent<Camera>().enabled = false;

                object[] data0 = (object[])eventData.CustomData;

                Vector3 TreeMapPosition = (Vector3)data0[2];
                MapTree = Instantiate(MapTree, TreeMapPosition, Quaternion.identity);
                
                Vector3 spawnPos = (Vector3)data0[0];
                Quaternion spawnRot = (Quaternion)data0[1];
                player = PhotonNetwork.Instantiate(HeroManager.selectedHero, spawnPos, spawnRot);
                playerPos = spawnPos;

                GameObject MinimapTreeMapSprite = MapTree.transform.Find("Minimap TreeMap Sprite").gameObject;
                MinimapTreeMapSprite.transform.parent = MapTree.transform.parent;
                
                MapCamera = Instantiate(MapCamera, MapCamera.transform.position, MapCamera.transform.rotation);
                Map.RandomTreasureZonePosition((Vector3) data0[3]);

                Minimap.SetTreasurePosition((Vector3) data0[3]);

                instantiated = true;
                break;

            //received the viewId of the pick-up gameObject from it
            case Codes.PICK_UP:
                object[] data1 = (object[])eventData.CustomData;
                GameObject pickUp = PhotonNetwork.GetPhotonView((int)data1[0]).gameObject;

                //copy info from pickup;

                PhotonNetwork.Destroy(pickUp);
                break;

            //received the viewId of a player that it's hiding somewhere
            case Codes.HIDE:
                object[] data2 = (object[])eventData.CustomData;
                GameObject otherPlayerGraphics = PhotonNetwork.GetPhotonView((int)data2[0]).gameObject.transform.GetChild(0).gameObject;
                GameObject hidingPlace = PhotonNetwork.GetPhotonView((int)data2[1]).gameObject;
                if (otherPlayerGraphics.activeSelf)
                {
                    otherPlayerGraphics.SetActive(false);
                    //hidingPlace.GetComponent<HidingPlace>().SetBusy(true);
                }
                else
                {
                    otherPlayerGraphics.SetActive(true);
                    //hidingPlace.GetComponent<HidingPlace>().SetBusy(false);
                }
                break;

            // received tree ids to destroy
            case Codes.TREE_DESTROY:
                object[] data3 = (object[])eventData.CustomData;
                int[] treesID = (int[])data3[0];

                GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");

                foreach (GameObject tree in trees)
                {
                    if (treesID.Contains(tree.GetComponent<TreeID>().treeID))
                    {
                        Destroy(tree);
                    }
                }
                break;

            // someone (i or another player) received a damage, update his health
            case Codes.DAMAGE:
                object[] data4 = (object[])eventData.CustomData;
                GameObject damagedPlayer = PhotonNetwork.GetPhotonView((int)data4[0]).gameObject;
                float newHealth = (float)data4[1];

                damagedPlayer.transform.Find("HealthController").GetComponent<Invector.vHealthController>().currentHealth = newHealth;
                damagedPlayer.GetComponent<vThirdPersonController>().currentHealth = newHealth;

                if (damagedPlayer.GetComponent<PhotonView>().IsMine)
                {
                    Slider damagedPlayerHealthSlider = damagedPlayer.transform.Find("Invector Components/UI/HUD/health").gameObject.GetComponent<Slider>();
                    damagedPlayerHealthSlider.value = newHealth;

                    StartCoroutine(ShowDamageImage());

                    playerIdLastShot = (int)data4[2];
                }
                break;

            // someone (i or another player) is dead
            case Codes.DEATH:
                object[] data5 = (object[])eventData.CustomData;
                GameObject deathPlayer = PhotonNetwork.GetPhotonView((int)data5[0]).gameObject;

                //remove killed player from the list
                playersIdList.Remove(deathPlayer.GetComponent<PhotonView>().OwnerActorNr);

                deathPlayer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
                deathPlayer.GetComponent<ColliderSync>().enabled = false;
                deathPlayer.transform.Find("HealthController").GetComponent<CapsuleCollider>().enabled = false;
                deathPlayer.transform.Find("HealthController/Head").GetComponent<SphereCollider>().enabled = false;

                if(deathPlayer.GetComponent<PhotonView>().IsMine)
                {
                    showHeadshotCanvas = true;

                    //tell to my killer that he has killed me
                    object[] data = new object[] { PhotonNetwork.LocalPlayer.NickName, PhotonNetwork.LocalPlayer.ActorNumber };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                    int[] receivers = { playerIdLastShot };
                    raiseEventOptions.TargetActors = receivers;
                    raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

                    SendOptions sendOptions = new SendOptions();
                    sendOptions.Reliability = true;

                    PhotonNetwork.RaiseEvent(Codes.KILL, data, raiseEventOptions, sendOptions);
                }

                Animator animator = deathPlayer.GetComponent<Animator>();
                animator.SetBool("isDead", true);

                if(deathPlayer.GetComponent<PhotonView>().IsMine && matchEnded == false)
                {
                    StartCoroutine(LoadEndGameMenu("YOU DIED"));
                    matchEnded = true;
                }
                break;

            //someone has shot
            case Codes.SHOT:
                object[] data6 = (object[])eventData.CustomData;
                GameObject shootingPlayer = PhotonNetwork.GetPhotonView((int)data6[0]).gameObject;

                AudioSource audioSource = shootingPlayer.GetComponent<EventsCall>().weapon.transform.Find("renderer/AudioSource").GetComponent<AudioSource>();
                AudioClip shotClip = shootingPlayer.GetComponent<EventsCall>().weapon.GetComponent<vShooterWeapon>().fireClip;
                audioSource.PlayOneShot(shotClip);

                ParticleSystem[] particleSystems = shootingPlayer.GetComponent<EventsCall>().weapon.transform.Find("renderer/Particles").GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.Play();
                }
                break;

            //i have killed someone, update my stat
            case Codes.KILL:
                object[] data9 = (object[])eventData.CustomData;
                int killedId = (int)data9[1];
                if(!playersKilledList.Contains(killedId))
                {
                    playersKilledList.Add(killedId);

                    killedPlayer = (string)data9[0];
                    killerPlayer = PhotonNetwork.LocalPlayer.NickName;
                    killMessageTimer = 5f;

                    //tell to the others
                    object[] data10 = new object[] { killedPlayer, killerPlayer };
                    RaiseEventOptions raiseEventOptions1 = new RaiseEventOptions();
                    raiseEventOptions1.Receivers = ReceiverGroup.Others;
                    raiseEventOptions1.CachingOption = EventCaching.AddToRoomCache;

                    SendOptions sendOptions1 = new SendOptions();
                    sendOptions1.Reliability = true;

                    PhotonNetwork.RaiseEvent(Codes.KILL_MSG, data10, raiseEventOptions1, sendOptions1);

                    kills++;
                    player.GetComponent<Skills>().IncrChargingLevel(25);
                }    
                
                break;

            //someone used the skill "silent footsteps"
            case Codes.SILENT_FOOTSTEPS:
                object[] data7 = (object[])eventData.CustomData;
                GameObject silentPlayer = PhotonNetwork.GetPhotonView((int)data7[0]).gameObject;
                float silentStepsVolume = (float)data7[1];
                int runningTime = (int)data7[2];
                StartCoroutine(ReduceFootstepNoise(silentPlayer, silentStepsVolume, runningTime));
                break;

            //someone is camping, show him in the map
            case Codes.SPOT_PLAYER:
                object[] data8 = (object[])eventData.CustomData;
                GameObject spottedPlayer = PhotonNetwork.GetPhotonView((int)data8[0]).gameObject;
                StartCoroutine(SpotPlayer(spottedPlayer));
                break;

            //someone has opened the chest
            case Codes.MATCH_FINISHED:
                StartCoroutine(LoadEndGameMenu("YOU LOSE!"));
                matchEnded = true;
                break;

            //someone was killed, print the message
            case Codes.KILL_MSG:
                object[] data11 = (object[])eventData.CustomData;
                killedPlayer = (string)data11[0];
                killedByText = " killed by ";
                killerPlayer = (string)data11[1];
                killMessageTimer = 5f;
                break;
        }
    }

    IEnumerator ReduceFootstepNoise(GameObject player, float silentStepsVolume, int runningTime)
    {
        FootStepVolumes footStepVolumes = player.GetComponent<FootStepVolumes>();
        footStepVolumes.SetSilentStepsVolume(silentStepsVolume);
        footStepVolumes.SetSilentStepsActive(true);
        yield return new WaitForSecondsRealtime(runningTime);
        footStepVolumes.SetSilentStepsActive(false);
    }

    IEnumerator SpotPlayer(GameObject spottedPlayer)
    {
        spottedPlayer.transform.Find("Minimap/Spotted Player").GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSecondsRealtime(spotTime);
        spottedPlayer.transform.Find("Minimap/Spotted Player").GetComponent<SpriteRenderer>().enabled = false;
    }



    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Ownership handler:                                                                                                         //
    // called when a player ask for the ownership of an object / after ownership changed (this code is executed by the old owner) //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer) //Request
    {
        throw new System.NotImplementedException();
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner) //Request / Takeover
    {
        string tag = targetView.gameObject.tag;

        switch (tag)
        {
            //sends to the new pick-up owner the viewID to handle the pick-up gameobject
            case "Pickable":
                object[] data = new object[] { targetView.ViewID };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                int[] reveivers = { targetView.OwnerActorNr };
                raiseEventOptions.TargetActors = reveivers;
                raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

                SendOptions sendOptions = new SendOptions();
                sendOptions.Reliability = true;

                PhotonNetwork.RaiseEvent(Codes.PICK_UP, data, raiseEventOptions, sendOptions);
                break;
        }
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest) //Fixed
    {
        throw new System.NotImplementedException();
    }

}
