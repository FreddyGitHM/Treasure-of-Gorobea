using UnityEngine;
using Photon.Pun;


public class AnimationSync : MonoBehaviourPunCallbacks, IPunObservable
{
    int id;
    Animator animator;

    bool idleRandomTrigger;
    bool weakAttack;
    bool strongAttack;
    bool triggerRecoil;
    bool triggerReaction;
    bool resetState;
    bool reload;
    bool cancelReload;
    bool shoot;

    void Awake()
    {
        id = GetComponent<PhotonView>().ViewID;
        animator = gameObject.GetComponent<Animator>();

        idleRandomTrigger = false;
        weakAttack = false;
        strongAttack = false;
        triggerRecoil = false;
        triggerReaction = false;
        resetState = false;
        reload = false;
        cancelReload = false;
        shoot = false;
    }

    void FixedUpdate()
    {
        if (idleRandomTrigger == false)
        {
            idleRandomTrigger = animator.GetBool("IdleRandomTrigger");
        }
        if (weakAttack == false)
        {
            weakAttack = animator.GetBool("WeakAttack");
        }
        if (strongAttack == false)
        {
            strongAttack = animator.GetBool("StrongAttack");
        }
        if (triggerRecoil == false)
        {
            triggerRecoil = animator.GetBool("TriggerRecoil");
        }
        if (triggerReaction == false)
        {
            triggerReaction = animator.GetBool("TriggerReaction");
        }
        if (resetState == false)
        {
            resetState = animator.GetBool("ResetState");
        }
        if (reload == false)
        {
            reload = animator.GetBool("Reload");
        }
        if (cancelReload == false)
        {
            cancelReload = animator.GetBool("CancelReload");
        }
        if (shoot == false)
        {
            shoot = animator.GetBool("Shoot");
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(id);
            stream.SendNext(idleRandomTrigger);
            stream.SendNext(weakAttack);
            stream.SendNext(strongAttack);
            stream.SendNext(triggerRecoil);
            stream.SendNext(triggerReaction);
            stream.SendNext(resetState);
            stream.SendNext(reload);
            stream.SendNext(cancelReload);
            stream.SendNext(shoot);

            if (idleRandomTrigger)
            {
                idleRandomTrigger = false;
            }
            if (weakAttack)
            {
                weakAttack = false;
            }
            if (strongAttack)
            {
                strongAttack = false;
            }
            if (triggerRecoil)
            {
                triggerRecoil = false;
            }
            if (triggerReaction)
            {
                triggerReaction = false;
            }
            if (resetState)
            {
                resetState = false;
            }
            if (reload)
            {
                reload = false;
            }
            if (cancelReload)
            {
                cancelReload = false;
            }
            if (shoot)
            {
                shoot = false;
            }
        }
        else
        {
            int id = (int)stream.ReceiveNext();
            Animator playerToUpdateAnimator = PhotonNetwork.GetPhotonView(id).gameObject.GetComponent<Animator>();

            playerToUpdateAnimator.SetBool("IdleRandomTrigger", (bool)stream.ReceiveNext());
            playerToUpdateAnimator.SetBool("WeakAttack", (bool)stream.ReceiveNext());
            playerToUpdateAnimator.SetBool("StrongAttack", (bool)stream.ReceiveNext());
            playerToUpdateAnimator.SetBool("TriggerRecoil", (bool)stream.ReceiveNext());
            playerToUpdateAnimator.SetBool("TriggerReaction", (bool)stream.ReceiveNext());
            playerToUpdateAnimator.SetBool("ResetState", (bool)stream.ReceiveNext());
            playerToUpdateAnimator.SetBool("Reload", (bool)stream.ReceiveNext());
            playerToUpdateAnimator.SetBool("CancelReload", (bool)stream.ReceiveNext());
            playerToUpdateAnimator.SetBool("Shoot", (bool)stream.ReceiveNext());
        }
    }

}
