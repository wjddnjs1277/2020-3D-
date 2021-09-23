using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DropWood : MonoBehaviourPunCallbacks
{
    public int bonusCount;
    public GameObject bonusGO;
    int hp = 8;
    public bool createBonus = false;
    bool firstHit = false;
    GameObject Go;

    Transform tr;
    Vector3 spown;
    bool isHit = false;
    float delayTime = 2.0f;
    float timer = 0;
    int hitDamage = 0;


    Vector3 bonusTr;

    PhotonView pv;
    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();
        tr = GetComponent<Transform>();
        spown = tr.position + new Vector3(0, 2, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (hp <= 0)
        {
            DropItem();
        }
        if(timer > delayTime && isHit == true)
        {           
            isHit = false;
            hp -= hitDamage;
            timer = 0;
        }
        if(createBonus == true)
        {
            CreateBonusMark();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Axe"))
        {
            isHit = true;
            hitDamage = 2;
            bonusTr = new Vector3(tr.position.x + 0.3f, other.transform.position.y+ 0.2f, tr.position.z + 0.2f);
            if (firstHit == false)
            {
                firstHit = true;
                createBonus = true;
            }
        }
        if (other.transform.CompareTag("NomalWP"))
        {
            isHit = true;
            hitDamage = 1;
        }
    }
    public void CreateBonusMark()
    {
        Instantiate(bonusGO, bonusTr, Quaternion.Euler(0, 90, 0), this.gameObject.transform);
    }

    void DropItem()
    {
        Go = PhotonNetwork.Instantiate("Wood", spown, Quaternion.identity);
        Go.GetComponent<QuantumItem>().quantity += bonusCount;
        pv.RPC("DestroyRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestroyRPC() => Destroy(this.gameObject);
}