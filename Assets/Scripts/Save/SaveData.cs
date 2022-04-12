using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData
{
    public string username;
    public int xp;
    public int coins;
    public int skinSelected;
    public List<int> skinsAvailable;
    public int weaponSelected;
    public List<int> weaponsAvailable;
    public int resolutionIndex;
    public bool fullScreen;
    public int qualityIndex;
    public float volume;

    public SaveData() {}

    //new save data
    public SaveData(string username)
    {
        this.username = username;
        xp = 0;
        coins = 0;
        skinSelected = 0;
        skinsAvailable = new List<int>();
        skinsAvailable.Add(0);
        weaponSelected = 0;
        weaponsAvailable = new List<int>();
        weaponsAvailable.Add(0);
        Resolution[] resolutions = Screen.resolutions;
        for(int i=0; i<resolutions.Length; i++)
        {
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                resolutionIndex = i;
                break;
            }
        }
        fullScreen = Screen.fullScreen;
        qualityIndex = 1;
        volume = 0.5f;
    }

}
