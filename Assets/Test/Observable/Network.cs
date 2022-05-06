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


public class Network : MonoBehaviour
{

    GameObject player;

    void Start()
    {
        Debug.Log("Connected: " + PhotonNetwork.IsConnected);
        Debug.Log("Nickname: " + PhotonNetwork.NickName);
        Debug.Log("Host: " + PhotonNetwork.IsMasterClient);

        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        mainCamera.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            player = PhotonNetwork.Instantiate("Man", new Vector3(2f, 1f, 2f), Quaternion.identity);
        }
        else
        {
            player = PhotonNetwork.Instantiate("Man", new Vector3(5f, 1f, 5f), Quaternion.identity);
        }

        player.GetComponent<vShooterMeleeInput>().enabled = true;
        player.GetComponent<vThirdPersonController>().enabled = true;
        player.GetComponent<vShooterManager>().enabled = true;
        player.GetComponent<vAmmoManager>().enabled = true;
        player.GetComponent<vHeadTrack>().enabled = true;
        player.GetComponent<vCollectShooterMeleeControl>().enabled = true;
        player.GetComponent<vGenericAction>().enabled = true;
        player.GetComponent<EventsCall>().enabled = true;
        player.transform.Find("Invector Components").Find("vThirdPersonCamera").gameObject.SetActive(true);
    }

}