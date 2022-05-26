using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSystem
{

    public static void Load()
    {
        Debug.Log("Loading...");

        GameStatus gameStatus = GameObject.FindWithTag("GameController").GetComponent<GameStatus>();

        SaveData saveData = LoadFile();
        if(saveData == null) //no save data file found
        {
            saveData = new SaveData(gameStatus.username);
            SaveFile(saveData);
        }

        gameStatus.username = saveData.username;
        gameStatus.level = saveData.level;
        gameStatus.xp = saveData.xp;
        gameStatus.coins = saveData.coins;
        gameStatus.skinSelected = saveData.skinSelected;
        gameStatus.skinsAvailable = saveData.skinsAvailable;
        gameStatus.weaponSelected = saveData.weaponSelected;
        gameStatus.weaponsAvailable = saveData.weaponsAvailable;
        gameStatus.resolutionIndex = saveData.resolutionIndex;
        gameStatus.fullScreen = saveData.fullScreen;
        gameStatus.qualityIndex = saveData.qualityIndex;
        gameStatus.volume = saveData.volume;
        gameStatus.HeroSelected = saveData.HeroSelected;

        Debug.Log("Loaded");
    }

    public static void Save()
    {
        Debug.Log("Saving...");

        SaveData saveData = new SaveData();

        GameStatus gameStatus = GameObject.FindWithTag("GameController").GetComponent<GameStatus>();
        saveData.username = gameStatus.username;
        saveData.level = gameStatus.level;
        saveData.xp = gameStatus.xp;
        saveData.coins = gameStatus.coins;
        saveData.skinSelected = gameStatus.skinSelected;
        saveData.skinsAvailable = gameStatus.skinsAvailable;
        saveData.weaponSelected = gameStatus.weaponSelected;
        saveData.weaponsAvailable = gameStatus.weaponsAvailable;
        saveData.resolutionIndex = gameStatus.resolutionIndex;
        saveData.fullScreen = gameStatus.fullScreen;
        saveData.qualityIndex = gameStatus.qualityIndex;
        saveData.volume = gameStatus.volume;
        saveData.HeroSelected = gameStatus.HeroSelected;

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
