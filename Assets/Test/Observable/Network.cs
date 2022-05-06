using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;
using Invector.vCharacterController;
using Invector.vShooter;
using Invector.vItemManager;
using Invector.vMelee;
using Invector.vCharacterController.vActions;
using UnityEngine.UI;


public class Network : MonoBehaviourPunCallbacks
{

    GameObject player;
    GameObject mainCamera;
    GameObject deathCanvas;

    void Awake()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        deathCanvas = GameObject.FindWithTag("DeathCanvas");
    }

    void Start()
    {
        Debug.Log("Connected: " + PhotonNetwork.IsConnected);
        Debug.Log("Nickname: " + PhotonNetwork.NickName);
        Debug.Log("Host: " + PhotonNetwork.IsMasterClient);

        mainCamera.GetComponent<Camera>().enabled = false;

        if (PhotonNetwork.IsMasterClient)
        {
            player = PhotonNetwork.Instantiate("Man", new Vector3(2f, 1f, 2f), Quaternion.identity);
        }
        else
        {
            player = PhotonNetwork.Instantiate("Man", new Vector3(5f, 1f, 5f), Quaternion.identity);
        }

        player.GetComponent<vShooterMeleeInput>().enabled = true;
        player.GetComponent<vThirdPersonController>().enabled = true;
        player.GetComponent<vShooterManager>().enabled = true;
        player.GetComponent<vAmmoManager>().enabled = true;
        player.GetComponent<vHeadTrack>().enabled = true;
        player.GetComponent<vCollectShooterMeleeControl>().enabled = true;
        player.GetComponent<vGenericAction>().enabled = true;
        player.transform.Find("Invector Components").Find("vThirdPersonCamera").gameObject.SetActive(true);

        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            // someone (i or another player) received a damage, update his health
            case Codes.DAMAGE:
                Debug.Log("hit");
                object[] data4 = (object[])eventData.CustomData;
                GameObject damagedPlayer = PhotonNetwork.GetPhotonView((int)data4[0]).gameObject;
                float newHealth = (float)data4[1];

                damagedPlayer.transform.Find("HealthController").GetComponent<Invector.vHealthController>().currentHealth = newHealth;
                damagedPlayer.GetComponent<vThirdPersonController>().currentHealth = newHealth;

                if(damagedPlayer.GetComponent<PhotonView>().IsMine)
                {
                    Slider damagedPlayerHealthSlider = damagedPlayer.transform.Find("Invector Components").Find("UI").Find("HUD").Find("health").gameObject.GetComponent<Slider>();
                    damagedPlayerHealthSlider.value = newHealth;
                }

                break;
            case Codes.DEATH:
                object[] data5 = (object[])eventData.CustomData;
                GameObject deathPlayer = PhotonNetwork.GetPhotonView((int)data5[0]).gameObject;

                if(deathPlayer.GetComponent<PhotonView>().IsMine)
                {
                    player.GetComponent<vShooterMeleeInput>().enabled = false;
                    player.GetComponent<vThirdPersonController>().enabled = false;
                    player.GetComponent<vShooterManager>().enabled = false;
                    player.GetComponent<vAmmoManager>().enabled = false;
                    player.GetComponent<vHeadTrack>().enabled = false;
                    player.GetComponent<vCollectShooterMeleeControl>().enabled = false;
                    player.GetComponent<vGenericAction>().enabled = false;
                    GameObject.Find("vThirdPersonCamera").SetActive(false);

                    mainCamera.GetComponent<Camera>().enabled = true;

                    SaveSystem.Save();

                    Destroy(GameObject.FindWithTag("GameController"));
                    Debug.Log("Canvas Find");
                    deathCanvas.GetComponent<Canvas>().enabled = true;

                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;

                    PhotonNetwork.Disconnect();
                }
                else
                {
                    Debug.Log(deathPlayer.GetComponent<PhotonView>().ViewID);
                }

                break;
        }
    }

}