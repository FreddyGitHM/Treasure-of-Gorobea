using UnityEngine;
using Photon.Pun;


public class Pickable : MonoBehaviourPun
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("PICK UP");
            other.gameObject.GetComponent<PickUp>().AddPickUp(gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("exit pick up zone");
            other.gameObject.GetComponent<PickUp>().RemovePickUp(gameObject.GetComponent<PhotonView>().ViewID);
        }
    }

}
