using UnityEngine;
using Invector.vCharacterController;

public class Map : MonoBehaviour
{
    private GameObject player;
    private GameObject FullscreenMapImage;
    private Canvas MapCanvas;
    private Camera camera;

    public float scroolSpeed;

    // Initial size of the camera
    float CameraSize;

    private void Awake()
    {
        FullscreenMapImage = GameObject.Find("FullscreenMap");
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();

        CameraSize = GetComponent<Camera>().orthographicSize;
        Debug.Log("Camera size: " + CameraSize);

        MapCanvas = GameObject.Find("MapCanvas").GetComponent<Canvas>();

        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            MapCanvas.enabled = !MapCanvas.enabled;

            // player.GetComponent<vShooterMeleeInput>().enabled = !player.GetComponent<vShooterMeleeInput>().enabled;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        if (!MapCanvas.enabled)
        {
            camera.orthographicSize = CameraSize;
        }
        else
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                if (camera.orthographicSize + Input.GetAxis("Mouse ScrollWheel") * scroolSpeed <= CameraSize)
                {
                    camera.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * scroolSpeed;
                }

            }
        }
    }

    void LateUpdate()
    {
        // Vector3 newPosition = player.transform.position;
        // newPosition.y = transform.position.y;
        // transform.position = newPosition;

        // transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
    }



}