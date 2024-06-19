using UnityEngine;
using Photon.Pun;

public class AnimationSync : MonoBehaviourPun, IPunObservable
{
    private Animator animator;
    private bool isPlaying;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Update the animation state of the object
            isPlaying = animator.GetBool("IsPlaying");
        }
        else
        {
            // Synchronize the animation state of the object for remote players
            animator.SetBool("IsPlaying", isPlaying);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the current animation state of the object to other players
            stream.SendNext(animator.GetBool("IsPlaying"));
        }
        else
        {
            // Receive the animation state of the object from the owner player
            isPlaying = (bool)stream.ReceiveNext();
        }
    }
}