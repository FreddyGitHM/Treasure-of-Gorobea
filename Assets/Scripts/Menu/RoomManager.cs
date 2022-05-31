using UnityEngine;


public class RoomManager : MonoBehaviour
{
    [SerializeField]
    string gameVersion = "0.0.1";
    [SerializeField]
    int maxPlayersNumber = 16;
    [SerializeField]
    int minPlayersNumber = 4;
    [SerializeField]
    float countdown = 10f;
    [SerializeField] 
    private float matchmakingTimer = 61f;
    [SerializeField] 
    private float resetTimer = 3f;
    [SerializeField] 
    private float heroSelectionTimer = 16f;

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
    
    public float MatchMakingTimer()
    {
        return matchmakingTimer;
    }
    
    public float ResetTimer()
    {
        return resetTimer;
    }
    
    public float HeroSelectionTimer()
    {
        return heroSelectionTimer;
    }
}
