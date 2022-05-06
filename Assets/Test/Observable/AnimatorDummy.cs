using UnityEngine;


public class AnimatorDummy : MonoBehaviour
{
    public bool visible;

    void Start()
    {
        visible = true;
    }

    void Update()
    {
        if(visible)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

}
