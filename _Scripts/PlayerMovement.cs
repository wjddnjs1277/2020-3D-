using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Threading;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{

    [SerializeField] float speed = 3f;
    [SerializeField] float gravity = -9.81f;
    //[SerializeField] float jumpHigh = 2f;
    [SerializeField] float groundDistance = 0.4f;

   
    public static string nickName;
    CharacterController controller;
    Camera myCam;
    Transform tr;
    PhotonView pv;
    

    Vector3 velocity;
    bool isGround;
    public bool attackPossible = true;

    Text myNick;

    Transform groundCheck;
    int groundLayer;

    Animator ani;
    StatusController myStatus;

    private Vector3 currPos;
    private Quaternion currRot;

    QuantumInventory inven;

    float timer = 0;
    float delayTime = 0.2f;

    float hitTimer = 10f;
    float hitDelayTime = 2.5f;
    bool ishit = false;

    public void Awake()
    {
        pv = GetComponent<PhotonView>();
        controller = GetComponent<CharacterController>();
        tr = GetComponent<Transform>();
        myNick = GetComponentInChildren<Text>();
        groundCheck = GetComponent<Transform>().Find("GroundCheck");
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        ani = GetComponent<Animator>();
        myCam = GetComponentInChildren<Camera>();
        myStatus = FindObjectOfType<StatusController>();
        inven = FindObjectOfType<QuantumInventory>();

        myNick.text = nickName;
        myNick.text = pv.IsMine ? PhotonNetwork.NickName : pv.Owner.NickName;
        if (pv.IsMine) 
        {
            //this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            myCam.gameObject.SetActive(true);
        }
        else
        {
            //this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            myCam.gameObject.SetActive(false);
        }
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

    // Update is called once per frame
    void FixedUpdate()
    {
        if (pv.IsMine)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
            if(isGround && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            IsDamage();

            if (SPCheck() == true && myStatus.currentSP < 1000)
            {
                if(timer > delayTime)
                {
                    myStatus.currentSP += 20;
                    timer = 0;
                }
                if (myStatus.currentSP > 1000)
                {
                    myStatus.currentSP = 1000;
                }
                if(myStatus.currentSP < 0)
                {
                    myStatus.currentSP = 0;
                }
                timer += Time.deltaTime;
            }

            if (UnityEngine.Cursor.visible == false && attackPossible == true && Input.GetMouseButtonDown(0))
            {
                inven.currentWp.GetComponent<BoxCollider>().enabled = true;
                attackPossible = false;
                ani.SetBool("walk", false);
                ani.SetBool("backwalking", false);
                Attack();
            }
            else if (ani.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack_1") && ani.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f)
            {
                ani.SetBool("Attack", false);
                if (Input.GetButtonDown("Jump"))
                {
                    if (z > 0)// 앞
                    {
                        Rolling();
                    }
                    else if (z < 0) //뒤
                    {
                        BackRolling();
                    }
                    else if (z == 0 && x > 0)
                    {
                        RightRolling();
                    }
                    else if (z == 0 && x < 0)
                    {
                        LeftRolling();
                    }
                }
            }
            else if(ani.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack_1") && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                if (ani.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Attack_1") && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
                {
                    inven.currentWp.GetComponent<BoxCollider>().enabled = false;
                }
                ani.SetBool("Attack", false);
                ani.SetInteger("Condition", 0);
                attackPossible = true;
            }
            else if(ani.GetBool("Attack") == false)
            {
                inven.currentWp.GetComponent<BoxCollider>().enabled = false;
                Vector3 move = transform.right * x + transform.forward * z;
                if (z > 0)// 앞
                {
                    attackPossible = false;
                    if (myStatus.currentSP > 100 && Input.GetKey(KeyCode.LeftShift))
                    {
                        if (Input.GetButtonDown("Jump"))
                        {
                            ani.SetBool("walk", false);
                            ani.SetBool("running", false);
                            Rolling();
                        }
                        else
                        {
                            ani.SetBool("rolling", false);
                            Run();
                            attackPossible = true;
                        }
                    }
                    else if (myStatus.currentSP > 200 && Input.GetButtonDown("Jump"))
                    {
                        ani.SetBool("walk", false);
                        Rolling();
                    }
                    else if (ani.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Rolling") && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        ani.SetBool("rolling", false);
                        speed = 3f;
                        WalkMove();
                        attackPossible = true;
                    }
                    else
                    {
                        if(ani.GetBool("rolling") == false)
                        {
                            speed = 3f;
                            attackPossible = true;
                        }
                        WalkMove();
                    }
                }
                else if (z < 0) //뒤
                {
                    attackPossible = false;
                    if (myStatus.currentSP > 200 && Input.GetButtonDown("Jump"))
                    {
                        BackRolling();
                    }
                    else if (ani.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.BackRolling") && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        ani.SetBool("backrolling", false);
                        BackwalkMove();
                        attackPossible = true;
                    }
                    else
                    {
                        if (ani.GetBool("backrolling") == false)
                        {
                            speed = 3f;
                            attackPossible = true;
                        }
                        BackwalkMove();
                    }
                }
                else if (z == 0 && x > 0)
                {
                    attackPossible = false;
                    if (myStatus.currentSP > 200 && Input.GetButtonDown("Jump"))
                    {
                        RightRolling();
                    }
                    else if (ani.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.RightRolling") && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        ani.SetBool("rightrolling", false);
                        right();
                        attackPossible = true;
                    }
                    else
                    {
                        if (ani.GetBool("rightrolling") == false)
                        {
                            speed = 3f;
                            attackPossible = true;
                        }
                        right();
                    }
                }
                else if (z == 0 && x < 0)
                {
                    attackPossible = false;
                    if (myStatus.currentSP > 200 && Input.GetButtonDown("Jump"))
                    {
                        LeftRolling();
                    }
                    else if (ani.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.LeftRolling") && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f)
                    {
                        ani.SetBool("leftrolling", false);
                        Left();
                        attackPossible = true;
                    }
                    else
                    {
                        if (ani.GetBool("leftrolling") == false)
                        {
                            speed = 3f;
                            attackPossible = true;
                        }
                        Left();
                    }
                }
                else
                {
                    inven.currentWp.GetComponent<BoxCollider>().enabled = false;
                    ani.SetBool("walk", false);
                    ani.SetBool("backwalking", false);
                    ani.SetBool("running", false);
                    ani.SetBool("rolling", false);
                    ani.SetBool("backrolling", false);
                    ani.SetBool("rightrolling", false);
                    ani.SetBool("leftrolling", false);
                    ani.SetBool("Attack", false);
                    ani.SetInteger("Condition", 0);
                    //attackPossible = true;
                }
                controller.Move(move * speed * Time.deltaTime);
            }

            /*if (Input.GetButtonDown("Jump") && isGround)
            {
                //velocity.y = Mathf.Sqrt(jumpHigh * -2f * gravity);
            }*/

            //pv.RPC("ChangeWeapon", RpcTarget.AllBuffered, weaponNum);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else
        {
            tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10.0f);
            tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 10.0f);
        }
    }

    void Rolling()
    {
        speed = 1.5f;
        myStatus.currentSP -= 100;
        attackPossible = false;
        ani.SetBool("rolling", true);
        ani.SetBool("Attack", false);
        ani.SetInteger("Condition", 8);
    }
    void BackRolling()
    {
        speed = 1.5f;
        myStatus.currentSP -= 100;
        attackPossible = false;
        ani.SetBool("backrolling", true);
        ani.SetBool("Attack", false);
        ani.SetInteger("Condition", 9);
    }
    void RightRolling()
    {
        speed = 1.5f;
        myStatus.currentSP -= 100;
        attackPossible = false;
        ani.SetBool("rightrolling", true);
        ani.SetBool("Attack", false);
        ani.SetInteger("Condition", 10);
    }
    void LeftRolling()
    {
        speed = 1.5f;
        myStatus.currentSP -= 100;
        attackPossible = false;
        ani.SetBool("leftrolling", true);
        ani.SetBool("Attack", false);
        ani.SetInteger("Condition", 11);
    }
    void Run()
    {
        speed = 6f;
        myStatus.currentSP -= 5;
        ani.SetBool("running", true);
        ani.SetBool("walk", false);
        ani.SetInteger("Condition", 2);
    }
    void Attack()
    {
        ani.SetBool("Attack", true);
        ani.SetInteger("Condition", 7);
    }
    void Jump()
    {
        ani.SetBool("jump", true);
        ani.SetInteger("Condition", 4);
    }
    void WalkMove()
    {
        ani.SetBool("walk", true);
        ani.SetBool("running", false);
        ani.SetInteger("Condition", 1);
    }
    
    void Left()
    {
        speed = 1.5f;
        ani.SetInteger("Condition", 6);
        ani.SetBool("walk", true);

    }
    void right()
    {
        speed = 1.5f;
        ani.SetInteger("Condition", 5);
        ani.SetBool("walk", true);
        
    }
    void BackwalkMove()
    {
        speed = 1.5f;
        ani.SetBool("backwalking", true);
        ani.SetInteger("Condition", 3);
    }

    public void AniReSet()
    {
        ani.SetBool("walk", false);
        ani.SetBool("backwalking", false);
        ani.SetBool("running", false);
        ani.SetBool("rolling", false);
        ani.SetBool("backrolling", false);
        ani.SetBool("rightrolling", false);
        ani.SetBool("leftrolling", false);
        ani.SetBool("Attack", false);
        ani.SetInteger("Condition", 0);
        attackPossible = true;
    }

    bool SPCheck()
    {
        if (ani.GetBool("running") == true)
            return false;
        if (ani.GetBool("rolling") == true)
            return false;
        if (ani.GetBool("backrolling") == true)
            return false;
        if (ani.GetBool("rightrolling") == true)
            return false;
        if (ani.GetBool("leftrolling") == true)
            return false;

        return true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Attack_AI")
        {
            if(ishit == true)
            {
                ishit = false;
                myStatus.currentHP -= 10;
            }
        }
    }
    void IsDamage()
    {
        if (hitTimer > hitDelayTime)
        {
            ishit = true;
            hitTimer = 0;
        }
        if (myStatus.currentHP < 0)
        {
            PhotonNetwork.Disconnect();
        }
        if(ishit == false)
            hitTimer += Time.deltaTime;
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Tree")
        {
            Destroy(collision.gameObject);
            if (pv.IsMine)
            {
                item++;
                Debug.Log(item);
            }
        }
    }*/
    /*
    [PunRPC]
    void ChangeWeapon(int num) 
    {
        for(int i = 0; i < weaponTr.transform.childCount; i++)
        {
            weaponTr.transform.GetChild(i).gameObject.SetActive(false);
            if(num == i)
            {
                weaponTr.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }*/
}
