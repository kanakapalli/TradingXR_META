using UnityEngine;
using Photon.Pun;

public class ObjectSync : MonoBehaviourPun, IPunObservable
{
    private Vector3 latestPosition;
    private Quaternion latestRotation;
    private Vector3 latestScale;

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Update the position, rotation, and scale of the object
            latestPosition = transform.position;
            latestRotation = transform.rotation;
            latestScale = transform.localScale;
        }
        else
        {
            // Smoothly interpolate the position, rotation, and scale of the object for remote players
            transform.position = Vector3.Lerp(transform.position, latestPosition, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRotation, Time.deltaTime * 5);
            transform.localScale = Vector3.Lerp(transform.localScale, latestScale, Time.deltaTime * 5);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send the current position, rotation, and scale of the object to other players
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale);
        }
        else
        {
            // Receive the position, rotation, and scale of the object from the owner player
            latestPosition = (Vector3)stream.ReceiveNext();
            latestRotation = (Quaternion)stream.ReceiveNext();
            latestScale = (Vector3)stream.ReceiveNext();
        }
    }
}