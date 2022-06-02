using Photon.Pun;
using UnityEngine;
using TMPro;


public class TreasureChest : MonoBehaviour
{
    GameObject player;
    NetworkManager networkManager;
    bool pickable;
    bool opened;

    void Awake()
    {
        networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
        pickable = false;
        opened = false;
    }

    void Update()
    {
        if(player == null)
        {
            player = networkManager.GetPlayer();
        }
        if(Input.GetKeyDown(KeyCode.E) && pickable && opened == false && networkManager.GetMatchEnded() == false)
        {
            opened = true;
            gameObject.GetComponent<Animator>().SetBool("Opening", true);
            networkManager.SetChestOpened(true);
            BoxCollider[] boxColliders = gameObject.GetComponents<BoxCollider>();
            foreach(BoxCollider bc in boxColliders)
            {
                bc.enabled = false;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag.Equals("Enemy") && other.gameObject.transform.parent.GetComponent<PhotonView>().IsMine && opened == false)
        {
            if(networkManager.GetMapTaken())
            {
                pickable = true;
                player.transform.Find("Invector Components/UI/HUD/treasureMap/openText").GetComponent<TextMeshProUGUI>().enabled = true;
            }
            else
            {
                player.transform.Find("Invector Components/UI/HUD/treasureMap/closeText").GetComponent<TextMeshProUGUI>().enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag.Equals("Enemy") && other.gameObject.transform.parent.GetComponent<PhotonView>().IsMine && opened == false)
        {
            if(networkManager.GetMapTaken())
            {
                pickable = false;
                player.transform.Find("Invector Components/UI/HUD/treasureMap/openText").GetComponent<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                player.transform.Find("Invector Components/UI/HUD/treasureMap/closeText").GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }
    }

}
