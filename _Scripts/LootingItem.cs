using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LootingItem : MonoBehaviourPunCallbacks
{

    PhotonView pv;
    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void DestroyItem()
    {
        pv.RPC("DestroyItemRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestroyItemRPC() => Destroy(this.gameObject);
}
