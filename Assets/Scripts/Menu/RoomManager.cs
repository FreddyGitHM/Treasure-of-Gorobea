using UnityEngine;


public class RoomManager : MonoBehaviour
{
    [SerializeField]
    string gameVersion = "0.0.1";
    [SerializeField]
    int maxPlayersNumber = 8;
    [SerializeField]
    int minPlayersNumber = 2;
    [SerializeField]
    float countdown = 10f;

    public RoomManager() {}

    public string GameVersion()
    {
        return gameVersion;
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
