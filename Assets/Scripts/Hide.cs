using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;


public class Hide : MonoBehaviourPun
{
    GameObject graphics;
    Rigidbody rigidbody;

    void Awake()
    {
        graphics = transform.GetChild(0).gameObject;
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && gameObject.GetComponent<PhotonView>().IsMine)
        {
            if(graphics.activeSelf)
            {
                graphics.SetActive(false);
                gameObject.GetComponent<MoveBehaviour>().enabled = false;
                gameObject.GetComponent<PickUp>().enabled = false;

                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                graphics.SetActive(true);
                gameObject.GetComponent<MoveBehaviour>().enabled = true;
                gameObject.GetComponent<PickUp>().enabled = true;

                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            object[] data = new object[] { gameObject.GetComponent<PhotonView>().ViewID };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
            raiseEventOptions.Receivers = ReceiverGroup.Others;
            raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

            SendOptions sendOptions = new SendOptions();
            sendOptions.Reliability = true;

            PhotonNetwork.RaiseEvent(Codes.HIDE, data, raiseEventOptions, sendOptions);
        }
    }
    
}
