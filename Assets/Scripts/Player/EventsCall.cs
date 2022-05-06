using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;

public class EventsCall : MonoBehaviourPunCallbacks
{
    public void OnDeath()
    {
        int id = gameObject.GetComponent<PhotonView>().ViewID; //death player id

        object[] data = new object[] { id };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(Codes.DEATH, data, raiseEventOptions, sendOptions);
    }

}
