using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSystem
{

    public static void Load()
    {
        Debug.Log("Loading...");

        PlayerStats playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();

        SaveData saveData = LoadFile();
        if(saveData == null) //no save data file found
        {
            saveData = new SaveData(playerStats.username);
            SaveFile(saveData);
        }

        playerStats.username = saveData.username;
        playerStats.xp = saveData.xp;
        playerStats.coins = saveData.coins;
        playerStats.skinSelected = saveData.skinSelected;
        playerStats.skinsAvailable = saveData.skinsAvailable;
        playerStats.weaponSelected = saveData.weaponSelected;
        playerStats.weaponsAvailable = saveData.weaponsAvailable;

        Debug.Log("Loaded");
    }

    public static void Save()
    {
        Debug.Log("Saving...");

        SaveData saveData = new SaveData();

        PlayerStats playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
        saveData.username = playerStats.username;
        saveData.xp = playerStats.xp;
        saveData.coins = playerStats.coins;
        saveData.skinSelected = playerStats.skinSelected;
        saveData.skinsAvailable = playerStats.skinsAvailable;
        saveData.weaponSelected = playerStats.weaponSelected;
        saveData.weaponsAvailable = playerStats.weaponsAvailable;

        SaveFile(saveData);

        Debug.Log("Saved!");
    }

    private static void SaveFile(SaveData saveData)
    {
        string path = Application.persistentDataPath + "/save.bin";
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path, FileMode.Create);

        binaryFormatter.Serialize(fileStream, saveData);

        fileStream.Close();
    }

    private static SaveData LoadFile()
    {
        string path = Application.persistentDataPath + "/save.bin";
        if(File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(path, FileMode.Open);

            SaveData saveData = (SaveData)binaryFormatter.Deserialize(fileStream);

            fileStream.Close();

            return saveData;
        }
        else
        {
            Debug.Log("No save file found!");
            return null;
        }
    }

}
