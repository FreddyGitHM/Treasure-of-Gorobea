using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;
using Invector.vCharacterController;


public class DamageEvent : MonoBehaviourPunCallbacks
{
    public void OnReceiveDamage()
    {
        int id = gameObject.transform.parent.GetComponent<PhotonView>().ViewID;
        float health = gameObject.GetComponent<Invector.vHealthController>().currentHealth;

        gameObject.transform.parent.GetComponent<vThirdPersonController>().currentHealth = health;

        object[] data = new object[] { id, health };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(Codes.DAMAGE, data, raiseEventOptions, sendOptions);
    }

}
