using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public float MoveSpeed = 1.0f;
    public Camera fpscam;
    public CharacterController charcon;
    private Quaternion latestRot;
    private Vector3 latestPos;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        charcon.GetComponent<CharacterController>();
        if (!photonView.IsMine)
        {
            Destroy(charcon);
        }
    }
    void Update()
    {
        if (photonView.IsMine)
        {
            Move();
        }
        else
        {
            // Synchronize position and rotation
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 10);
        }
        
    }
    void Move()
    {
        float hmov = Input.GetAxis("Horizontal");
        float vmov = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(hmov, 0, vmov);
        charcon.Move(movement);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to other players
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Receive data from other players
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
