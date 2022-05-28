using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Invector.vCharacterController;
using UnityEngine.UI;


public class PauseMenu : MonoBehaviour
{
    GameObject player;
    Button resumeButton;

    void Awake()
    {
        resumeButton = gameObject.transform.Find("Background/ResumeButton").GetComponent<Button>();
    }

    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();
        }
    }

    public void Call()
    {
        if(gameObject.GetComponent<Canvas>().enabled) // disable
        {
            OnResume();
        }
        else //enable
        {
            DisablePlayerController();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            gameObject.GetComponent<Canvas>().enabled = true;
        }
    }

    void EnablePlayerController()
    {
        player.GetComponent<vShooterMeleeInput>().enabled = true;
    }

    void DisablePlayerController()
    {
        player.GetComponent<vShooterMeleeInput>().enabled = false;

        Animator animator = player.GetComponent<Animator>();

        animator.SetFloat("InputHorizontal", 0f);
        animator.SetFloat("InputVertical", 0f);
        animator.SetFloat("InputMagnitude", 0f);
        animator.SetFloat("RotationMagnitude", 0f);
        animator.SetBool("IsStrafing", false);
        animator.SetBool("IsSprinting", false);
        animator.SetBool("IsAiming", false);
    }

    public void OnResume()
    {
        EnablePlayerController();
        gameObject.GetComponent<Canvas>().enabled = false;
        resumeButton.interactable = false;
        resumeButton.interactable = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnExit()
    {
        PhotonNetwork.Disconnect();

        DestroyImmediate(GameObject.FindWithTag("GameController"));
        DestroyImmediate(GameObject.FindWithTag("Terrain"));

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        SceneManager.LoadScene("Menu");
    }

}
