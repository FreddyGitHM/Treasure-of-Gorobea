using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;
using Invector.vCharacterController;


public class EventsCall : MonoBehaviourPunCallbacks
{
    public GameObject weapon;

    public void OnDeath()
    {
        gameObject.transform.Find("HealthController").GetComponent<vDamageReceiver>().enabled = false;

        int id = gameObject.GetComponent<PhotonView>().ViewID; //death player id

        object[] data = new object[] { id };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(Codes.DEATH, data, raiseEventOptions, sendOptions);
    }

    public void OnShot()
    {
        int id = gameObject.GetComponent<PhotonView>().ViewID; //player who shot

        object[] data = new object[] { id };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(Codes.SHOT, data, raiseEventOptions, sendOptions);
    }

    public void OnStartSprinting()
    {
        gameObject.GetComponent<vShooterMeleeInput>().IsCrouching = false;
    }

}
