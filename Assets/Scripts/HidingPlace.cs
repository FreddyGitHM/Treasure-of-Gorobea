using UnityEngine;
using Photon.Pun;


public class HidingPlace : MonoBehaviour
{
    bool busy;

    void Awake()
    {
        busy = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if(!busy)
            {
                Debug.Log("HIDE");
                other.gameObject.GetComponent<Hide>().SetHidingPlaceId(gameObject.GetComponent<PhotonView>().ViewID);
            }
            else
            {
                Debug.Log("place already occupied");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("exit possible hiding place");
            other.gameObject.GetComponent<Hide>().SetHidingPlaceId(-1);
        }
    }

    public bool GetBusy()
    {
        return busy;
    }

    public void SetBusy(bool state)
    {
        busy = state;
    }

}
