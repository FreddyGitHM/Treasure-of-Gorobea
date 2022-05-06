using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;
using System.Collections.Generic;
using UnityEditor;


public class EventsCall : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        /*
        // Searching for all objects in scene
        List<GameObject> GetAllObjectsOnlyInScene()
        {
            List<GameObject> objectsInScene = new List<GameObject>();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                    objectsInScene.Add(go);
            }

            return objectsInScene;
        }

        // Searching for death canvas object
        foreach (GameObject gameObject in GetAllObjectsOnlyInScene())
        {
            if (gameObject.name == "DeathCanvas")
            {
                canvas = gameObject;
                Debug.Log("Death canvas find: " + gameObject.name);
            }
            else if (gameObject.name == "Main Camera")
            {
                mainCamera = gameObject;
                Debug.Log("Main Camera found: " + gameObject.name);
            }
        }*/
    }

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
