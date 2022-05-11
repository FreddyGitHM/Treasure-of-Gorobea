using UnityEngine;
using Invector.vCharacterController;

public class Map : MonoBehaviour
{
    private GameObject player;
    private GameObject FullscreenMapImage;
    private Canvas MapCanvas;
    private Camera MapCamera;
    private Terrain terrain;

    // Zoom parameters
    private float MaxZoom;
    public float MinZoom = 100f;
    public float scroolSpeed;

    // Map movement parameter
    private Vector3 dragOrigin;
    private float MinMovement;
    private float MaxMovement;

    private void Awake()
    {
        FullscreenMapImage = GameObject.Find("FullscreenMap");
        player = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();
        MapCanvas = GameObject.Find("MapCanvas").GetComponent<Canvas>();
        MapCamera = GetComponent<Camera>();
        terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
    }

    private void Start()
    {
        // Setting values for zoom
        MaxZoom = MapCamera.orthographicSize;

        MinMovement = MapCamera.orthographicSize;
        MaxMovement = terrain.terrainData.size.x - MapCamera.orthographicSize;
    }

    private void Update()
    {
        // Close or open the map
        OpenCloseMap();

        if (MapCanvas.enabled)
        {
            DisableShooterInput();

            // Map zoom
            MapZoom();

            MapMovement();
        }
        else
        {
            EnableShooterInput();
        }
    }

    private void OpenCloseMap()
    {
        // Open or close the map
        if (Input.GetButtonDown("Back"))
        {
            MapCanvas.enabled = !MapCanvas.enabled;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        // If the map is close reset camera parameter
        if (!MapCanvas.enabled)
        {
            MapCamera.orthographicSize = MaxZoom;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void DisableShooterInput()
    {
        player.GetComponent<vShooterMeleeInput>().SetLockShooterInput(true);
    }

    private void EnableShooterInput()
    {
        player.GetComponent<vShooterMeleeInput>().SetLockShooterInput(false);
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