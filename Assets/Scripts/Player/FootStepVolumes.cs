using Invector;
using UnityEngine;


public class FootStepVolumes : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float walkVolume;
    [Range(0.0f, 1.0f)]
    public float runVolume;
    [Range(0.0f, 1.0f)]
    public float crouchVolume;

    Animator animator;
    vFootStep footstep;
    float silentStepsVolume;
    bool silentStepsActive;


    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        footstep = gameObject.GetComponent<vFootStep>();
        silentStepsActive = false;
    }

    void Update()
    {
        if(silentStepsActive)
        {
            footstep.Volume = silentStepsVolume;
        }
        else
        {
            if(animator.GetBool("IsSprinting"))
            {
                footstep.Volume = runVolume;
            }
            else if(animator.GetBool("IsCrouching"))
            {
                footstep.Volume = crouchVolume;
            }
            else
            {
                footstep.Volume = walkVolume;
            }
        }
    }

    public void SetSilentStepsVolume(float volume)
    {
        silentStepsVolume = volume;
    }

    public void SetSilentStepsActive(bool b)
    {
        silentStepsActive = b;
    }

}
