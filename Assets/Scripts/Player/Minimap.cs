using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    private GameObject player;
    private GameObject FullscreenMapImage;

    //Terrain data
    private Terrain terrain;
    private TerrainData td;
    private int x = 0;
    private int y = 0;
    private Vector2Int MapCenter;

    private void Awake()
    {
        FullscreenMapImage = GameObject.Find("FullscreenMap");
    }

    private void Start()
    {
        //Getting terrain information
        terrain = Terrain.activeTerrain;
        td = terrain.terrainData;
        x = td.heightmapResolution;
        y = td.heightmapResolution;

		// player = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().getPlayer();

        MapCenter = new Vector2Int((x - 1) / 2, (y - 1) / 2);
    }

    private void Update()
    {
		if(Input.GetKeyDown(KeyCode.M)){
			transform.GetComponent<Camera>().enabled = !transform.GetComponent<Camera>().enabled;

			FullscreenMapImage.GetComponent<RawImage>().enabled = !FullscreenMapImage.GetComponent<RawImage>().enabled;
		}
    }

    void LateUpdate()
    {
        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
    }



}