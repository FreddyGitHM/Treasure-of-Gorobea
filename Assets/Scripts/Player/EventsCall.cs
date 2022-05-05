using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;

using System.Collections.Generic;
using UnityEditor;


public class EventsCall : MonoBehaviourPunCallbacks
{
    public GameObject player;

    private GameObject canvas;

    private void Awake() {
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
        }
    }
    
    public void OnReceiveDamage()
    {
        int id = player.GetComponent<PhotonView>().ViewID;
        float health = player.transform.Find("HealthController").GetComponent<Invector.vHealthController>().currentHealth;

        object[] data = new object[] { id, health };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCache;

        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true;

        PhotonNetwork.RaiseEvent(Codes.DAMAGE, data, raiseEventOptions, sendOptions);
    }

    public void OnDeath()
    {
        Debug.Log("[On Death]");
        if (canvas != null)
        {
            Debug.Log("Canvas Find");
            canvas.SetActive(true);
        }

        PhotonNetwork.Disconnect();
    }

}
