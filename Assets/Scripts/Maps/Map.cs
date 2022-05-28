using UnityEngine;
using Invector.vCharacterController;

public class Map : MonoBehaviour
{
    private GameObject player;
    private GameObject FullscreenMapImage;
    private Canvas MapCanvas;
    private Camera MapCamera;
    private Terrain terrain;
    private Canvas MinimapCanvas;
    private Vector3 TreasurePosition;
    private GameObject TreasureMapSprite;

    // Zoom parameters
    private float MaxZoom;
    public float MinZoom = 100f;
    public float scroolSpeed;

    // Map movement parameter
    private Vector3 dragOrigin;
    private float MinMovement;
    private float MaxMovement;

    public static float RandomX;
    public static float RandomY;

    private void Awake()
    {
        FullscreenMapImage = GameObject.Find("FullscreenMap");
        player = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();
        MapCanvas = GameObject.Find("MapCanvas").GetComponent<Canvas>();
        MapCamera = GetComponent<Camera>();
        terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
        MinimapCanvas = player.transform.Find("Minimap/Minimap Canvas").GetComponent<Canvas>();
        TreasurePosition = GameObject.Find("TreasureChest(Clone)").transform.position;
        TreasureMapSprite = GameObject.Find("Map Treasure Sprite");

        Vector3 SpriteSize = TreasureMapSprite.transform.localScale * .5f;
        RandomX = Random.Range(-SpriteSize.x, SpriteSize.x);
        RandomY = Random.Range(-SpriteSize.z, SpriteSize.z);
    }

    private void Start()
    {
        // Setting camera position according to terrain size
        MapCamera.transform.position = terrain.GetPosition() + Vector3.one * terrain.terrainData.size.x * .5f;

        // Setting ortographic size to the half of terrain width and height
        MapCamera.orthographicSize = terrain.terrainData.size.x * .5f;

        // Setting values for zoom
        MaxZoom = MapCamera.orthographicSize;

        MinMovement = MapCamera.orthographicSize;
        MaxMovement = terrain.terrainData.size.x - MapCamera.orthographicSize;

        TreasureMapSprite.transform.position = RandomTreasureZonePosition();
    }

    private void Update()
    {
        // Close or open the map
        OpenCloseMap();

        if (MapCanvas.enabled)
        {
            DisableMinimap();

            DisableShooterInput();

            MapZoom();

            MapMovement();
        }
        else
        {
            EnableMinimap();

            EnableShooterInput();
        }
    }

    private Vector3 RandomTreasureZonePosition()
    {
        return new Vector3(TreasurePosition.x + RandomX, MapCamera.transform.position.y - 50, TreasurePosition.z + RandomY);
    }

    private void OpenCloseMap()
    {
        // Open or close the map
        if (Input.GetButtonDown("Back"))
        {
            MapCanvas.enabled = !MapCanvas.enabled;
            // If the map is close reset camera parameter
            if(!MapCanvas.enabled)
            {
                MapCamera.orthographicSize = MaxZoom;
                MapCamera.transform.position = terrain.GetPosition() + Vector3.one * terrain.terrainData.size.x * .5f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }

    private void DisableMinimap()
    {
        MinimapCanvas.enabled = false;
    }

    private void EnableMinimap()
    {
        MinimapCanvas.enabled = true;
    }

    private void DisableShooterInput()
    {
        player.GetComponent<vShooterMeleeInput>().SetLockShooterInput(true);
        player.GetComponent<vShooterMeleeInput>().SetLockCameraInput(true);
    }

    private void EnableShooterInput()
    {
        player.GetComponent<vShooterMeleeInput>().SetLockShooterInput(false);
        player.GetComponent<vShooterMeleeInput>().SetLockCameraInput(false);
    }

    private void MapZoom()
    {
        // Change zoom in and zoom out according to the mouse scroll wheel direction
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            MapCamera.orthographicSize = ClampMapZoom(MapCamera.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * scroolSpeed);

            MinMovement = MapCamera.orthographicSize;
            MaxMovement = terrain.terrainData.size.x - MapCamera.orthographicSize;

            // Clamp the camera position in order to avoid exiting from bound
            MapCamera.transform.position = ClampCameraPosition(MapCamera.transform.position);
        }
    }

    private float ClampMapZoom(float zoom)
    {
        return Mathf.Clamp(zoom, MinZoom, MaxZoom);
    }

    private void MapMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = MapCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - MapCamera.ScreenToWorldPoint(Input.mousePosition);
            MapCamera.transform.position = ClampCameraPosition(MapCamera.transform.position + difference);
        }
    }

    private Vector3 ClampCameraPosition(Vector3 camPosition)
    {
        float newX = Mathf.Clamp(camPosition.x, MinMovement, MaxMovement);
        float newY = Mathf.Clamp(camPosition.z, MinMovement, MaxMovement);

        return new Vector3(newX, camPosition.y, newY);
    }
}