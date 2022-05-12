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
        if(Input.GetKeyDown(KeyCode.E) && pickable && opened == false && networkManager.GetVictory() == false)
        {
            opened = true;
            GameObject.FindWithTag("TreasureChest").GetComponent<Animator>().SetBool("Opening", true);
            networkManager.SetChestOpened(true);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Enemy") && other.gameObject.transform.parent.GetComponent<PhotonView>().IsMine && opened == false && networkManager.GetMapTaken())
        {
            pickable = true;
            player.transform.Find("Invector Components/UI/HUD/treasureMap/takeText").GetComponent<TextMeshProUGUI>().enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Enemy") && other.gameObject.transform.parent.GetComponent<PhotonView>().IsMine && opened == false && networkManager.GetMapTaken())
        {
            pickable = false;
            player.transform.Find("Invector Components/UI/HUD/treasureMap/takeText").GetComponent<TextMeshProUGUI>().enabled = false;
        }
    }

}
