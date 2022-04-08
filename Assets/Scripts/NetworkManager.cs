using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    const byte SPAWN_POSITION = 0;

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
                Vector3 spawnPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
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
                } else
                {
                    //choose the Prefab to spawn
                    //PhotonNetwork.Instantiate("Prefab_name", spawnPos, Quaternion.identity);
                }
            }
        }
    }

    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void OnEvent(EventData eventData)
    {
        switch(eventData.Code)
        {
            case SPAWN_POSITION:
                object[] data = (object[])eventData.CustomData;
                Vector3 spawnPos = (Vector3)data[0];
                //PhotonNetwork.Instantiate("Prefab_name", spawnPos, Quaternion.identity);
                break;
        }
    }

}
