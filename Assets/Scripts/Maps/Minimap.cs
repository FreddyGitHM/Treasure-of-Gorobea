using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private GameObject player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().GetPlayer();
    }

    void LateUpdate()
    {
        // Vector3 newPosition = player.transform.position;
        // newPosition.y = transform.position.y;
        // transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
