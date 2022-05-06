using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;
using TMPro;


public class SyncVariable : MonoBehaviourPunCallbacks, IPunObservable
{
    int id;
    GameObject text;
    int line;

    void Start()
    {
        id = gameObject.GetComponent<PhotonView>().ViewID;
        text = GameObject.FindWithTag("PlayerUI");
        line = 0;
    }    

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(line == 10)
        {
            text.GetComponent<TextMeshProUGUI>().text = "";
            line = 0;
        }

        if(stream.IsWriting)
        {
            Debug.Log("send stream");
            text.GetComponent<TextMeshProUGUI>().text += "send stream\n";
            line++;
            stream.SendNext(id);
            stream.SendNext(gameObject.GetComponent<AnimatorDummy>().visible);
        }
        else
        {
            Debug.Log("received stream");
            text.GetComponent<TextMeshProUGUI>().text += "received stream\n";
            line++;
            GameObject receiverPlayer = PhotonNetwork.GetPhotonView((int)stream.ReceiveNext()).gameObject;
            receiverPlayer.GetComponent<AnimatorDummy>().visible = (bool)stream.ReceiveNext();
        }
    }

}
