using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;
using Invector.vCharacterController;


public class DamageEvent : MonoBehaviourPunCallbacks
{
    public AudioClip hitFeedback;

    GameObject player; //local player
    GameObject aimCenter;
    Color originalAimColor;
    Color hitAimColor;

    void Start()
    {
        player = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();
        //player = GameObject.FindWithTag("NetworkManager").GetComponent<Network>().GetPlayer(); //debug
        originalAimColor = new Color(1f, 1f, 1f, 0.73f);
        hitAimColor = new Color(1f, 0f, 0f, 0.73f);
        Debug.Log(player.activeSelf);
    }

    public void OnReceiveDamage()
    {
        int id = gameObject.transform.parent.GetComponent<PhotonView>().ViewID;
        float health = gameObject.GetComponent<Invector.vHealthController>().currentHealth;

        gameObject.transform.parent.GetComponent<vThirdPersonController>().currentHealth = health;

        object[] data = new object[] { id, health, player.GetComponent<PhotonView>().OwnerActorNr };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(Codes.DAMAGE, data, raiseEventOptions, sendOptions);

        AudioSource audioSource = player.GetComponent<EventsCall>().weapon.transform.Find("renderer").Find("AudioSource").GetComponent<AudioSource>();
        audioSource.PlayOneShot(hitFeedback);

        aimCenter = player.transform.Find("Invector Components/AimCanvas/AimCanvas/AimID_2_AssaultRifle/SimpleAimGroupe/AimCenter").gameObject;
        if(aimCenter != null)
        {
            StartCoroutine(ChangeAimCenterColor());
        }
        
    }

    IEnumerator ChangeAimCenterColor()
    {
        if(aimCenter != null)
        {
            aimCenter.GetComponent<Image>().color = hitAimColor;
        }
        
        yield return new WaitForSecondsRealtime(0.5f);

        if(aimCenter != null)
        {
            aimCenter.GetComponent<Image>().color = originalAimColor;
        }
    }

}
