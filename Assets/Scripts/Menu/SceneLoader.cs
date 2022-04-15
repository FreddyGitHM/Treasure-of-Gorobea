using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // The scene to be loaded
    public string nextScene;

    // The panel to be loaded where it is a loading screen with the slider
    public GameObject loadingScreen;

    // The slider object in order to perform changes in its status
    public Slider slider;

    public void LoadNextScene(){
        StartCoroutine(AsyncLoad()); 
    }

    private IEnumerator AsyncLoad(){

        yield return new WaitForEndOfFrame();

        //  This is needed in order to load a scene in background and set it active when the loading process will be finished
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);

        // Force the process to don't set the scene active immediately after it is ready, in order to have a better control over it
        operation.allowSceneActivation = false;

        // Set the loadingScreen canvas visible
        loadingScreen.SetActive(true);

        while(!operation.isDone){
            
            // In Unity the loading process is misuread with a float number between 0 and 0.9, in order to have it in the range [0, 1] it is calculated the progress value 
            float progress = Mathf.Clamp01(operation.progress / .9f);

            // This is a placeholder if in order to see the loading bar charging a bit over time. It is needed for the scenes that are too simple to be charged, otherwise the loading process will be too 
            // fast and it is not possible to see it. When we will load a real scene, heavy enough, it is not needed because the process will take a time and during this the slider bar will be updated as well
            // if(operation.progress == 0.9f){
            //     while(slider.value < progress){
            //         slider.value += 0.1f;
            //         yield return new WaitForEndOfFrame();
            //     }
            //     yield return new WaitForSecondsRealtime(1);
            // }

            // Setting the slider value to the process value, in order to have an animation of the slider and the loading process
            slider.value = progress;

            yield return new WaitForEndOfFrame();

            // Activate the loaded scene
            operation.allowSceneActivation = true;
        }

    }
    
}
