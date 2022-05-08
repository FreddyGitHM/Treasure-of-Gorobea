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
using System.Collections;

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


        player.transform.Find("Invector Components").Find("UI").gameObject.GetComponent<Canvas>().enabled = true;

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
                    vHUDController vHUDController = damagedPlayer.transform.Find("Invector Components").Find("UI").Find("HUD").GetComponent<vHUDController>();
                    vHUDController.damaged = true;
                }

                break;
            case Codes.DEATH:
                object[] data5 = (object[])eventData.CustomData;
                GameObject deathPlayer = PhotonNetwork.GetPhotonView((int)data5[0]).gameObject;

                deathPlayer.GetComponent<Rigidbody>().useGravity = false;
                deathPlayer.transform.Find("HealthController").GetComponent<CapsuleCollider>().enabled = false;

                if (deathPlayer.GetComponent<PhotonView>().IsMine)
                {
                    player.GetComponent<vShooterMeleeInput>().enabled = false;
                    player.GetComponent<vThirdPersonController>().enabled = false;
                    player.GetComponent<vShooterManager>().enabled = false;
                    player.GetComponent<vAmmoManager>().enabled = false;
                    player.GetComponent<vHeadTrack>().enabled = false;
                    player.GetComponent<vCollectShooterMeleeControl>().enabled = false;
                    player.GetComponent<vGenericAction>().enabled = false;

                    SaveSystem.Save();
                }

                Animator animator = deathPlayer.GetComponent<Animator>();
                animator.SetBool("isDead", true);

                if(deathPlayer.GetComponent<PhotonView>().IsMine)
                {
                    StartCoroutine(LoadDeathMenu());
                }
                else
                {
                    Debug.Log(deathPlayer.GetComponent<PhotonView>().ViewID);
                }

                break;


            //someone has shot
            case Codes.SHOT:
                object[] data6 = (object[])eventData.CustomData;
                Debug.Log("Player " + (int)data6[0] + " shoot");
                GameObject shootingPlayer = PhotonNetwork.GetPhotonView((int)data6[0]).gameObject;

                AudioSource audioSource = shootingPlayer.GetComponent<EventsCall>().weapon.transform.Find("renderer").Find("AudioSource").GetComponent<AudioSource>();
                AudioClip shotClip = shootingPlayer.GetComponent<EventsCall>().weapon.GetComponent<vShooterWeapon>().fireClip;
                audioSource.PlayOneShot(shotClip);

                ParticleSystem[] particleSystems = shootingPlayer.GetComponent<EventsCall>().weapon.transform.Find("renderer").Find("Particles").GetComponentsInChildren<ParticleSystem>();
                foreach(ParticleSystem ps in particleSystems)
                {
                    ps.Play();
                }

                break;
        }
    }

    IEnumerator ShowDamageImage()
    {
        Image damageImage = player.transform.Find("Invector Components").Find("UI").Find("HUD").Find("damageImage").GetComponent<Image>();
        damageImage.enabled = true;

        yield return new WaitForSecondsRealtime(0.5f);

        damageImage.enabled = false;
    }

    IEnumerator LoadDeathMenu()
    {
        yield return new WaitForSecondsRealtime(5f);
        GameObject.Find("vThirdPersonCamera").SetActive(false);
        deathCanvas.GetComponent<Canvas>().enabled = true;
        mainCamera.GetComponent<Camera>().enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

}