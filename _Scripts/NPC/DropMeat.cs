using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Networking;

public class DropMeat : MonoBehaviour
{
    public int hp = 1;
    GameObject Go;

    Transform tr;
    Vector3 spown;
    bool isHit = false;
    float delayTime = 2.0f;
    float timer = 0;
    int hitDamage = 0;

    PhotonView pv;
    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();
        tr = GetComponent<Transform>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        spown = tr.position + new Vector3(0, 2, 0);
        timer += Time.deltaTime;
        if (hp <= 0)
        {
            DropItem();
        }
        if (timer > delayTime && isHit == true)
        {
            isHit = false;
            hp -= hitDamage;
            timer = 0;
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Axe"))
        {
            isHit = true;
            hitDamage = 2;
        }
        if (other.transform.CompareTag("NomalWP"))
        {
            isHit = true;
            hitDamage = 1;
        }
    }

    void DropItem()
    {
        Go = PhotonNetwork.Instantiate("Meat", spown, Quaternion.identity);
        Go.GetComponent<QuantumItem>().quantity = 1;
        //Go.GetComponent<QuantumItem>().quantity += bonusCount;
        //pv.RPC("DestroyRPC", RpcTarget.AllBuffered);
        Destroy(gameObject);
    }

    [PunRPC]
    void DestroyRPC() => Destroy(this.gameObject);
}
