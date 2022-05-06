using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;

public class Network : MonoBehaviour
{

    GameObject player;

    void Start()
    {
        Debug.Log("Connected: " + PhotonNetwork.IsConnected);
        Debug.Log("Nickname: " + PhotonNetwork.NickName);
        Debug.Log("Host: " + PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient)
        {
            player = PhotonNetwork.Instantiate("CubePlayer", new Vector3(0f, 0f, 0f), Quaternion.identity);
        }
        else
        {
            player = PhotonNetwork.Instantiate("CubePlayer", new Vector3(-2f, 0f, 0f), Quaternion.identity);
        }
    }

}