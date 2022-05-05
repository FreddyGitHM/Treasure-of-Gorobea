using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    public void OnClick(){
        // Destroy terrain
        GameObject terrain = GameObject.Find("Large Map");
        DestroyImmediate(terrain);

        SceneManager.LoadScene("Menu");
    }    
}
