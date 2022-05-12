using UnityEngine;

public class Minimap : MonoBehaviour
{
    private GameObject player;
    private GameObject MinimapCamera;
    private Vector3 TreeMapPosition;
    private GameObject MinimapTreeMapSprite;

    public float MinimapSize;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();
        MinimapCamera = player.transform.Find("Minimap/MinimapCamera").gameObject;

        TreeMapPosition = GameObject.Find("PF_Oak_big Terrain(Clone)").transform.position;
        MinimapTreeMapSprite = GameObject.Find("Minimap TreeMap Sprite");
    }

    private void Update()
    {
        MinimapTreeMapSprite.transform.position = TreeMapPosition;
    }

    void LateUpdate()
    {
        MinimapTreeMapSprite.transform.position = ClampTreeMapPosition(MinimapTreeMapSprite.transform.position);
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
}
