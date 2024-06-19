using UnityEngine;
using Photon.Pun;

public class VFXSync : MonoBehaviourPun, IPunObservable
{
    private ParticleSystem particleSystem;
    private bool isPlaying;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Update the VFX state of the object
            isPlaying = particleSystem.isPlaying;
        }
        else
        {
            // Synchronize the VFX state of the object for remote players
            if (isPlaying && !particleSystem.isPlaying)
                particleSystem.Play();
            else if (!isPlaying && particleSystem.isPlaying)
                particleSystem.Stop();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the current VFX state of the object to other players
            stream.SendNext(particleSystem.isPlaying);
        }
        else
        {
            // Receive the VFX state of the object from the owner player
            isPlaying = (bool)stream.ReceiveNext();
        }
    }
}