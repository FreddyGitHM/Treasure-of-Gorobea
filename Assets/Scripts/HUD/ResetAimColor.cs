using UnityEngine;
using UnityEngine.UI;


public class ResetAimColor : MonoBehaviour
{
    void OnEnable()
    {
        gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.73f);
    }

}
