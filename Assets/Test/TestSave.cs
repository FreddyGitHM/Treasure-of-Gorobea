using UnityEngine;


public class TestSave : MonoBehaviour
{

    void Awake()
    {
        Application.targetFrameRate = 60;
        SaveSystem.Load();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            SaveSystem.Save();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            SaveSystem.Load();
        }
    }

}
