using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_AI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }
    public State state = State.PATROL;
    [SerializeField]
    Transform playertr;
    [SerializeField]
    private Transform enemyTr;

    public float attackDist = 5.0f;
    public float traceDist = 10.0f;
    public bool isDie = false;
    private bool isAttack = false;

    private float damping = 1.0f;
    private MoveAgent1 moveagent;
    private WaitForSeconds ws;
    private Animator ani;
    GameObject player;
    float dist;
    Quaternion rot;

    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
   
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

  
    private void Awake()
    {
        //var player = GameObject.FindGameObjectWithTag("Player");

        //if (player != null) playertr = player.GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        ws = new WaitForSeconds(0.3f);
        moveagent = GetComponent<MoveAgent1>();
        ani = GetComponent<Animator>();
        ani.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        ani.SetFloat(hashWalkSpeed, Random.Range(1.0f, 1.2f));

        Invoke("ActionStart", 3f);
    }
    
    /* private void OnEnable()
     {
         StartCoroutine(CheckState());
     }*/
    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return ws;
            switch (state)
            {
                case State.PATROL:
                    isAttack = false;
                    moveagent.patrolling = true;
                    ani.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    moveagent.traceTarget = playertr.position;
                    isAttack = false;
                    ani.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    moveagent.Stop();
                    ani.SetBool(hashMove, false);
                    isAttack = true;
                    if(!isAttack)
                    {
                        rot = Quaternion.LookRotation(playertr.position - enemyTr.position);
                        enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
                        enemyTr.Translate(Vector3.forward * Time.deltaTime * hashSpeed);
                    }
                    break;
                case State.DIE:
                    moveagent.Stop();
                    break;
            }
        }
    }
    IEnumerator CheckState()
    {
        while (!isDie)
        {
            if (state == State.DIE) yield break;

            dist = Vector3.Distance(playertr.transform.position, enemyTr.position);

            if (dist <= attackDist)
            {
                state = State.ATTACK;
            }
            else if(dist <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }
            ani.SetBool("IsAttack", isAttack);
            yield return ws;
        }
    }
        
    // Update is called once per frame
    void FixedUpdate()
    {/*
        StartCoroutine(CheckState());
        StartCoroutine(Action());
        player = GameObject.FindGameObjectWithTag("Player");*/
        if (player != null) playertr = player.GetComponent<Transform>();
        ani.SetFloat(hashSpeed, moveagent.speed);
    }
    void ActionStart()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
