using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class DeathMenu : MonoBehaviourPun
{
    public void OnClick(){
        PhotonNetwork.Disconnect();

        DestroyImmediate(GameObject.FindWithTag("GameController"));
        DestroyImmediate(GameObject.Find("Large Map"));

        SceneManager.LoadScene("Menu");
    }
    
}
