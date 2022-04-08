using UnityEngine;


public class MasterManager : MonoBehaviour
{
    [SerializeField]
    string gameVersion = "0.0.1";
    [SerializeField]
    string playerName = "shanga_93";
    [SerializeField]
    int maxPlayersNumber = 8;
    [SerializeField]
    int minPlayersNumber = 2;
    [SerializeField]
    float countdown = 10f;

    public MasterManager() {}

    public string GameVersion()
    {
        return gameVersion;
    }

    public string PlayerName()
    {
        return playerName;
    }

    public int MaxPlayersNumber()
    {
        return maxPlayersNumber;
    }

    public int MinPlayersNumber()
    {
        return minPlayersNumber;
    }

    public float Countdown()
    {
        return countdown;
    }

}
