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

using System.Collections.Generic;


public class NetworkManager : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    GameObject player; //local player
    GameObject MapTree; // MapTree object
    GameObject TreasureChest;
    bool instantiated;
    bool ready;

    void Awake()
    {
        instantiated = false;
        ready = false;
    }

    void Start()
    {
        Debug.Log("Connected: " + PhotonNetwork.IsConnected);
        Debug.Log("Nickname: " + PhotonNetwork.NickName);
        Debug.Log("Host: " + PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 TreeMapPosition = RandomTreeMapGenerator.Instance.spawnTreeMap();
            Debug.Log("Calculating tree map poistion: " + TreeMapPosition);

            Debug.Log("Calculating players spawn position with " + PhotonNetwork.CurrentRoom.PlayerCount + " players...");
            SpawnPosition.Instance.calculateSpawnPositions(PhotonNetwork.CurrentRoom.PlayerCount);

            //send random position for players
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                //set here the spawn position
                Vector3 spawnPos = SpawnPosition.Instance.getSpawnPosition();
                Debug.Log("Position for player " + p.NickName + ": " + spawnPos);

                // getting random rotation for hero
                Quaternion spawnRot = SpawnPosition.Instance.getLookDirection(spawnPos);

                if (PhotonNetwork.LocalPlayer != p)
                {
                    object[] data = new object[] { spawnPos, TreeMapPosition };

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                    int[] reveivers = { p.ActorNumber };
                    raiseEventOptions.TargetActors = reveivers;
                    raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

                    SendOptions sendOptions = new SendOptions();
                    sendOptions.Reliability = true;

                    Debug.Log("send SPAWN_POSITION and TreeMapPosition to player: " + p.NickName);
                    PhotonNetwork.RaiseEvent(Codes.SPAWN_POSITION, data, raiseEventOptions, sendOptions);
                }
                else
                {
                    //choose the Prefab to spawn
                    player = PhotonNetwork.Instantiate("Man", spawnPos, spawnRot);
                    MapTree = PhotonNetwork.Instantiate("TreeMap", TreeMapPosition, Quaternion.identity);
                    instantiated = true;
                }
            }

            TreasureChest = PhotonNetwork.InstantiateRoomObject("TreasureChest", TreasureSpawn.Instance.getTreasurePosition(), TreasureSpawn.Instance.getTreasureRotation());
            TreasureChest.transform.rotation = Quaternion.LookRotation((TreeMapPosition - TreasureSpawn.Instance.getTreasurePosition()).normalized, Vector3.up);
        }

        //add this class for EventsHandler and IPunOwnershipCallbacks
        PhotonNetwork.AddCallbackTarget(this);
    }

    void Update()
    {
        if (instantiated == true && ready == false)
        {
            EnableComponents();
            ready = true;
            Debug.Log("Ready!");
        }
    }

    void EnableComponents()
    {
        /*
        player.GetComponent<BasicBehaviour>().enabled = true;
        player.GetComponent<MoveBehaviour>().enabled = true;
        player.GetComponent<PickUp>().enabled = true;
        player.GetComponent<Hide>().enabled = true;

        GameObject camera = player.transform.GetComponentInChildren<Camera>().gameObject;
        camera.GetComponent<Camera>().enabled = true;
        camera.GetComponent<AudioListener>().enabled = true;
        camera.GetComponent<ThirdPersonOrbitCamBasic>().enabled = true;
        */

        player.GetComponent<vShooterMeleeInput>().enabled = true;
        player.GetComponent<vShooterManager>().enabled = true;
        player.GetComponent<vAmmoManager>().enabled = true;
        player.GetComponent<vHeadTrack>().enabled = true;
        player.GetComponent<vCollectShooterMeleeControl>().enabled = true;
        player.GetComponent<vGenericAction>().enabled = true;
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
                object[] data0 = (object[])eventData.CustomData;
                Vector3 TreeMapPosition = (Vector3)data0[1];
                MapTree = PhotonNetwork.Instantiate("TreeMap", TreeMapPosition, Quaternion.identity);
                Vector3 spawnPos = (Vector3)data0[0];
                player = PhotonNetwork.Instantiate("Man", spawnPos, SpawnPosition.Instance.getLookDirection(spawnPos));
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
                    hidingPlace.GetComponent<HidingPlace>().SetBusy(true);
                }
                else
                {
                    otherPlayerGraphics.SetActive(true);
                    hidingPlace.GetComponent<HidingPlace>().SetBusy(false);
                }
                break;

            // received trees to destroy
            case Codes.TREE_DESTROY:
                object[] data3 = (object[])eventData.CustomData;
                List<int> treesID = (List<int>)data3[0];

                GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");

                foreach (GameObject tree in trees)
                {
                    if (treesID.Contains(tree.GetInstanceID()))
                    {
                        Destroy(tree);
                    }
                }

                break;
        }
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
