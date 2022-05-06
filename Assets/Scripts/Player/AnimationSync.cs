using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using EventCodes;
using Invector.vCharacterController;
using Invector.vShooter;
using Invector.vItemManager;
using Invector.vMelee;
using Invector.vCharacterController.vActions;


public class AnimationSync : MonoBehaviourPunCallbacks, IPunObservable
{
    Animator animator;
    int id;

    /*
    public bool idleRandomTrigger;
    public bool weakAttack;
    public bool strongAttack;
    public bool triggerRecoil;
    public bool triggerReaction;
    public bool resetState;
    public bool reload;
    public bool cancelReload;
    public bool shoot;*/

    void Awake()
    {
        id = GetComponent<PhotonView>().ViewID;
        animator = gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        /*
        idleRandomTrigger = animator.GetBool("IdleRandomTrigger");
        weakAttack = animator.GetBool("WeakAttack");
        strongAttack = animator.GetBool("StrongAttack");
        triggerRecoil = animator.GetBool("TriggerRecoil");
        triggerReaction = animator.GetBool("TriggerReaction");
        resetState = animator.GetBool("ResetState");
        reload = animator.GetBool("Reload");
        cancelReload = animator.GetBool("CancelReload");
        shoot = animator.GetBool("Shoot");*/
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(id);
            stream.SendNext(animator.GetBool("IdleRandomTrigger"));
            stream.SendNext(animator.GetBool("WeakAttack"));
            stream.SendNext(animator.GetBool("StrongAttack"));
            stream.SendNext(animator.GetBool("TriggerRecoil"));
            stream.SendNext(animator.GetBool("TriggerReaction"));
            stream.SendNext(animator.GetBool("ResetState"));
            stream.SendNext(animator.GetBool("Reload"));
            stream.SendNext(animator.GetBool("CancelReload"));
            stream.SendNext(animator.GetBool("Shoot"));
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
