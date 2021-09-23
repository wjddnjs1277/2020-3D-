using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LigthMove : MonoBehaviourPunCallbacks, IPunObservable
{
    Transform tr;

    private Vector3 currPos;
    private Quaternion currRot;

    void Awake()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tr.RotateAround(Vector3.zero, Vector3.right, 0.25f * Time.deltaTime);
        tr.LookAt(Vector3.zero);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
