using UnityEngine;


public class PlayerStatus : MonoBehaviour
{
    [SerializeField]
    int hp = 100;
    [SerializeField]
    float speed = 1f;

    //from game status
    [SerializeField]
    string username;
    [SerializeField]
    int skinSelected;
    [SerializeField]
    int weaponSelected;

    void Awake()
    {
        GameStatus gameStatus = GameObject.FindWithTag("GameController").GetComponent<GameStatus>();
        username = gameStatus.username;
        skinSelected = gameStatus.skinSelected;
        weaponSelected = gameStatus.weaponSelected;
    }

}
