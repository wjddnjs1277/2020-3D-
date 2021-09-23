using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NPCAttack : MonoBehaviour
{
    
    [SerializeField] private int hp;//동물의 체력
    //GameObject obj = GameObject.Find("Pig");
    PhotonView pv;
    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();
      //  tr = GetComponent<Transform>();
        //spown = tr.position + new Vector3(0, 2, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      /*(  if (hp <= 0)
        {
            DropItem();
        }
        */
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Axe"))
        {
            hp -= 2;
        }
        if (other.transform.CompareTag("NomalWP"))
        {
            hp--;
        }
    }

    /*void DropItem()
    {
        pv.RPC("DestroyRPC", RpcTarget.AllBuffered);
        PhotonNetwork.Instantiate("Wood", spown, Quaternion.identity);
    }*/

    
}
