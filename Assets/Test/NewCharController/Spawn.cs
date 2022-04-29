using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        player = GameObject.Instantiate(player, new Vector3(10f, 0.5f, 10f), Quaternion.identity);
    }

}
