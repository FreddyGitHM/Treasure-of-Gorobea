using UnityEngine;

public class Minimap : MonoBehaviour
{
    private GameObject player;
    private GameObject MinimapCamera;
    private Vector3 TreeMapPosition;
    private GameObject MinimapTreeMapSprite;
    private Vector3 TreasurePosition;
    private GameObject TreasureMinimapSprite;

    public float MinimapSize;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();
        MinimapCamera = player.transform.Find("Minimap/MinimapCamera").gameObject;

        TreeMapPosition = GameObject.Find("PF_Oak_big Terrain(Clone)").transform.position;
        MinimapTreeMapSprite = GameObject.Find("Minimap TreeMap Sprite");

        TreasurePosition = GameObject.Find("TreasureChest(Clone)").transform.position;
        TreasureMinimapSprite = GameObject.Find("Minimap Treasure Sprite");
        TreasureMinimapSprite.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void Update()
    {
        MinimapTreeMapSprite.transform.position = TreeMapPosition;
        TreasureMinimapSprite.transform.position = TreasurePosition;
    }

    void LateUpdate()
    {
        MinimapTreeMapSprite.transform.position = ClampTreeMapPosition(MinimapTreeMapSprite.transform.position);
        TreasureMinimapSprite.transform.position = ClampTreasurePosition(TreasureMinimapSprite.transform.position);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private Vector3 ClampTreeMapPosition(Vector3 TreeMapPos)
    {
        return
        new Vector3(
            Mathf.Clamp(TreeMapPos.x, MinimapCamera.transform.position.x - MinimapSize, MinimapCamera.transform.position.x + MinimapSize),
            MinimapCamera.transform.position.y - 10,
            Mathf.Clamp(TreeMapPos.z, MinimapCamera.transform.position.z - MinimapSize, MinimapCamera.transform.position.z + MinimapSize)
        );
    }

    private Vector3 ClampTreasurePosition(Vector3 TreasurePos){
        return
        new Vector3(
            Mathf.Clamp(TreasurePos.x, MinimapCamera.transform.position.x - MinimapSize, MinimapCamera.transform.position.x + MinimapSize),
            MinimapCamera.transform.position.y - 10,
            Mathf.Clamp(TreasurePos.z, MinimapCamera.transform.position.z - MinimapSize, MinimapCamera.transform.position.z + MinimapSize)
        );
    }
}
