using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class TreasureMap : MonoBehaviour
{
    GameObject player;
    bool pickable;
    bool taken;

    void Awake()
    {
        player = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();
        pickable = false;
        taken = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && pickable && taken == false)
        {
            taken = true;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            player.transform.Find("Invector Components/UI/HUD/treasureMap").GetComponent<RawImage>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            player.transform.Find("Invector Components/UI/HUD/treasureMap/takeText").GetComponent<TextMeshProUGUI>().enabled = false;
            GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>().SetMapTaken(true);

            GameObject.Find("Map Treasure Sprite").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("Minimap Treasure Sprite").GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag.Equals("Enemy") && other.gameObject.transform.parent.GetComponent<PhotonView>().IsMine && taken == false)
        {
            pickable = true;
            player.transform.Find("Invector Components/UI/HUD/treasureMap/takeText").GetComponent<TextMeshProUGUI>().enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag.Equals("Enemy") && other.gameObject.transform.parent.GetComponent<PhotonView>().IsMine && taken == false)
        {
            pickable = false;
            player.transform.Find("Invector Components/UI/HUD/treasureMap/takeText").GetComponent<TextMeshProUGUI>().enabled = false;
        }
    }

}
