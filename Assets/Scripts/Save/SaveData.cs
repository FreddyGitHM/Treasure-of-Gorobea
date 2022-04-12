using System.Collections.Generic;


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
    }

}
