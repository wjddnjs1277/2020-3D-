using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAttack_AI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        FLEE,
        DIE
    }
    public State state = State.PATROL;
    [SerializeField]
    Transform playertr;
    [SerializeField]
    private Transform enemyTr;

    public float FleeDist = 4.0f;
    public float traceDist = 10.0f;
    public bool isDie = false;
    private bool isAttack = false;

    private float damping = 1.0f;
    private Moveagent move;
    private WaitForSeconds ws;
    private Animator ani;

    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashFlee = Animator.StringToHash("IsFlee");
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
        move = GetComponent<Moveagent>();
        ani = GetComponent<Animator>();
        ani.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        ani.SetFloat(hashWalkSpeed, Random.Range(1.0f, 1.2f));
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
                    move.patrolling_NoA = true;
                    ani.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                   // move.traceTarget = playertr.position;
                    isAttack = false;
                   // ani.SetBool(hashMove, true);
                    break;
                case State.FLEE:
                    Vector3 dirToPlayer = enemyTr.position - playertr.position;
                    Vector3 newPos = enemyTr.position + dirToPlayer;
                    move.agent_noA.SetDestination(newPos);
                    ani.SetBool(hashFlee, true);
                    break;
                case State.DIE:
                    move.Stop();
                    break;
            }
        }
    }
    IEnumerator CheckState()
    {
        while (!isDie)
        {
            if (state == State.DIE) yield break;

            float dist = Vector3.Distance(playertr.transform.position, enemyTr.position);

            if (dist <= FleeDist)
            {
                state = State.FLEE;
            }
            else if (dist <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }
           
            yield return ws;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playertr = player.GetComponent<Transform>();
        ani.SetFloat(hashSpeed, move.speed);
    }
}
