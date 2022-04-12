using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;


public class Hide : MonoBehaviourPun
{
    GameObject graphics;
    Rigidbody rigidbody;
    int hidingPlaceId;

    void Awake()
    {
        graphics = transform.GetChild(0).gameObject;
        rigidbody = gameObject.GetComponent<Rigidbody>();
        hidingPlaceId = -1;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && gameObject.GetComponent<PhotonView>().IsMine && hidingPlaceId != -1)
        {
            GameObject hidingPlace = PhotonNetwork.GetPhotonView(hidingPlaceId).gameObject;
            bool sendEvent = true;

            if(!graphics.activeSelf) //not visibile: i'm exiting the hiding place
            {
                hidingPlace.GetComponent<HidingPlace>().SetBusy(false);

                graphics.SetActive(true);
                gameObject.GetComponent<MoveBehaviour>().enabled = true;
                gameObject.GetComponent<PickUp>().enabled = true;

                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
            else if(hidingPlace.GetComponent<HidingPlace>().GetBusy()) //visible: check if the hiding place is still avaliable
            {
                Debug.Log("place already occupied");
                sendEvent = false;
            }
            else
            {
                hidingPlace.GetComponent<HidingPlace>().SetBusy(true);

                graphics.SetActive(false);
                gameObject.GetComponent<MoveBehaviour>().enabled = false;
                gameObject.GetComponent<PickUp>().enabled = false;

                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }

            if(sendEvent)
            {
                object[] data = new object[] { gameObject.GetComponent<PhotonView>().ViewID, hidingPlaceId }; //this player id, hiding place id
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                raiseEventOptions.Receivers = ReceiverGroup.Others;
                raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

                SendOptions sendOptions = new SendOptions();
                sendOptions.Reliability = true;

                PhotonNetwork.RaiseEvent(Codes.HIDE, data, raiseEventOptions, sendOptions);
            }
        }
    }

    public void SetHidingPlaceId(int id)
    {
        hidingPlaceId = id;
    }

}
