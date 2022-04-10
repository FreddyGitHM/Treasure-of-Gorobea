using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PickUp : MonoBehaviourPun
{
    List<int> pickableObjects;

    void Start()
    {
        pickableObjects = new List<int>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && gameObject.GetComponent<PhotonView>().IsMine && pickableObjects.Count > 0)
        {
            int id = pickableObjects[pickableObjects.Count-1];
            RemovePickUp(id);
            GameObject pickUp = PhotonNetwork.GetPhotonView(id).gameObject;
            if (!PhotonNetwork.GetPhotonView(id).IsMine)
            {
                pickUp.GetComponent<PhotonView>().RequestOwnership();
            }

            //copy info from pickup;

            PhotonNetwork.Destroy(pickUp);
        }
    }

    public void AddPickUp(int id)
    {
        pickableObjects.Add(id);
    }

    public void RemovePickUp(int id)
    {
        if(pickableObjects.IndexOf(id) != -1)
        {
            pickableObjects.Remove(id);
        }
    }

}
