using UnityEngine;


public class ColliderSync : MonoBehaviour
{
    public Vector3 standingCenter;
    public float standingHeight;
    public Vector3 crouchCenter;
    public float crouchHeight;
    public Transform headBone;
    public Vector3 offsetSphereCollider;

    Animator animator;
    CapsuleCollider capsuleCollider;
    GameObject head;

    bool isCrouching;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        capsuleCollider = gameObject.transform.Find("HealthController").GetComponent<CapsuleCollider>();
        head = gameObject.transform.Find("HealthController/Head").gameObject;

        isCrouching = false;
    }

    void Update()
    {
        isCrouching = animator.GetBool("IsCrouching");
        if(isCrouching)
        {
            capsuleCollider.center = crouchCenter;
            capsuleCollider.height = crouchHeight;
        }
        else
        {
            capsuleCollider.center = standingCenter;
            capsuleCollider.height = standingHeight;
        }

        if(head.GetComponent<SphereCollider>().enabled == false)
        {
            head.GetComponent<SphereCollider>().enabled = true;
        }

        if(Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("B"))
        {
            head.GetComponent<SphereCollider>().enabled = false;
        }
        head.transform.position = headBone.position + offsetSphereCollider;
    }

}
