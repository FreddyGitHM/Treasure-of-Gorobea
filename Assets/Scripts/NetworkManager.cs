using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    const byte SPAWN_POSITION = 0;

    GameObject player;
    bool instantiated;
    bool ready;

    private void Awake()
    {
        instantiated = false;
        ready = false;
    }

    void Start()
    {
        Debug.Log("Connected: " + PhotonNetwork.IsConnected);
        Debug.Log("Nickname: " + PhotonNetwork.NickName);
        Debug.Log("Host: " + PhotonNetwork.IsMasterClient);

        if(PhotonNetwork.IsMasterClient)
        {
            //send random position for players
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                //set here the spawn position
                Vector3 spawnPos = new Vector3(Random.Range(10f, 20f), 0.5f, Random.Range(10f, 20f));
                Debug.Log("Position for player " + p.NickName + ": " + spawnPos);

                if (PhotonNetwork.LocalPlayer != p)
                {
                    object[] data = new object[] { spawnPos };

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                    raiseEventOptions.Receivers = ReceiverGroup.Others;
                    raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

                    SendOptions sendOptions = new SendOptions();
                    sendOptions.Reliability = true;

                    Debug.Log("send SPAWN_POSITION to player: " + p.NickName);
                    PhotonNetwork.RaiseEvent(SPAWN_POSITION, data, raiseEventOptions, sendOptions);
                }
                else
                {
                    //choose the Prefab to spawn
                    player = PhotonNetwork.Instantiate("Man", spawnPos, Quaternion.identity);
                    instantiated = true;
                }
            }
        }
    }

    private void Update()
    {
        if(instantiated == true && ready == false)
        {
            EnableComponents();
            ready = true;
            Debug.Log("Ready!");
        }
    }

    private void EnableComponents()
    {
        player.GetComponent<BasicBehaviour>().enabled = true;
        player.GetComponent<MoveBehaviour>().enabled = true;
        GameObject camera = player.transform.GetComponentInChildren<Camera>().gameObject;
        camera.GetComponent<Camera>().enabled = true;
        camera.GetComponent<AudioListener>().enabled = true;
        camera.GetComponent<ThirdPersonOrbitCamBasic>().enabled = true;
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData eventData)
    {
        switch(eventData.Code)
        {
            case SPAWN_POSITION:
                Debug.Log("event SPAWN_POSITION received");
                object[] data = (object[])eventData.CustomData;
                Vector3 spawnPos = (Vector3)data[0];
                player = PhotonNetwork.Instantiate("Man", spawnPos, Quaternion.identity);
                instantiated = true;
                break;
        }
    }

}
