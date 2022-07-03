using UnityEngine;

public class Minimap : MonoBehaviour
{
    private GameObject player;
    private GameObject MinimapCamera;
    private Vector3 TreeMapPosition;
    private GameObject MinimapTreeMapSprite;
    private static Vector3 TreasurePosition;
    private GameObject TreasureMinimapSprite;

    public float MinimapTreeSize;
    public float MinimapTreasureSize;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();
        MinimapCamera = player.transform.Find("Minimap/MinimapCamera").gameObject;

        TreeMapPosition = GameObject.Find("TreeMap(Clone)").transform.position;
        MinimapTreeMapSprite = GameObject.Find("Minimap TreeMap Sprite");
        
        TreasureMinimapSprite = GameObject.Find("Minimap Treasure Sprite");
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

    public static void SetTreasurePosition(Vector3 pos)
    {
        TreasurePosition = pos;
    }

    private Vector3 ClampTreeMapPosition(Vector3 TreeMapPos)
    {
        return
        new Vector3(
            Mathf.Clamp(TreeMapPos.x, MinimapCamera.transform.position.x - MinimapTreeSize, MinimapCamera.transform.position.x + MinimapTreeSize),
            MinimapCamera.transform.position.y - 10,
            Mathf.Clamp(TreeMapPos.z, MinimapCamera.transform.position.z - MinimapTreeSize, MinimapCamera.transform.position.z + MinimapTreeSize)
        );
    }

    private Vector3 ClampTreasurePosition(Vector3 TreasurePos)
    {
        return
        new Vector3(
            Mathf.Clamp(TreasurePos.x, MinimapCamera.transform.position.x - MinimapTreasureSize, MinimapCamera.transform.position.x + MinimapTreasureSize),
            MinimapCamera.transform.position.y - 10,
            Mathf.Clamp(TreasurePos.z, MinimapCamera.transform.position.z - MinimapTreasureSize, MinimapCamera.transform.position.z + MinimapTreasureSize)
        );
    }
}
