using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestChangeColor : MonoBehaviour
{
    GameObject aimCenter;
    Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        aimCenter = GameObject.Find("Canvas/AimHud/AimCenter");
        Debug.Log(aimCenter.name);
        originalColor = aimCenter.GetComponent<Image>().color;
        Debug.Log(originalColor);
    }

    // Update is called once per frame
    void Update()
    {
        Color actualColor = aimCenter.GetComponent<Image>().color;
        if (Input.GetKeyDown(KeyCode.A))
        {
            if(actualColor == originalColor)
            {
                aimCenter.GetComponent<Image>().color = new Color(1, 0, 0);
            }
            else
            {
                aimCenter.GetComponent<Image>().color = originalColor;
            }
        }
    }
}
