using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEditor;
using Invector.vCharacterController;
using Invector.vShooter;
using Invector.vItemManager;
using Invector.vMelee;
using Invector.vCharacterController.vActions;


public class EventsCall : MonoBehaviourPunCallbacks
{
    GameObject canvas;
    GameObject mainCamera;

    void Awake()
    {
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
        }
    }

    public void OnDeath()
    {
        Debug.Log("[On Death]");
        if (canvas != null)
        {
            gameObject.GetComponent<vShooterMeleeInput>().enabled = false;
            gameObject.GetComponent<vThirdPersonController>().enabled = false;
            gameObject.GetComponent<vShooterManager>().enabled = false;
            gameObject.GetComponent<vAmmoManager>().enabled = false;
            gameObject.GetComponent<vHeadTrack>().enabled = false;
            gameObject.GetComponent<vCollectShooterMeleeControl>().enabled = false;
            gameObject.GetComponent<vGenericAction>().enabled = false;
            GameObject.Find("vThirdPersonCamera").SetActive(false);

            mainCamera.SetActive(true);

            SaveSystem.Save();

            Destroy(GameObject.FindWithTag("GameController"));
            Debug.Log("Canvas Find");
            canvas.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        PhotonNetwork.Disconnect();
    }

}
