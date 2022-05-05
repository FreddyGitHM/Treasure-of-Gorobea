using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;


public class EventsCall : MonoBehaviourPunCallbacks
{
    public GameObject player;

    public void OnReceiveDamage()
    {
        int id = player.GetComponent<PhotonView>().ViewID;
        float health = player.transform.Find("HealthController").GetComponent<Invector.vHealthController>().currentHealth;

        object[] data = new object[] { id, health };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(Codes.DAMAGE, data, raiseEventOptions, sendOptions);
    }

}
