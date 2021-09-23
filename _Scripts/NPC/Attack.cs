using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField]
    Transform playertr;
    private Transform enemyTr;
    private Animator ani;

    private readonly int hashAttack = Animator.StringToHash("Attack");
    private float nextAttack = 0.0f;
    private readonly float attackRate = 0.1f;
    private readonly float damping = 10.0f;

    public bool isAttack = false;
    // Start is called before the first frame update
    void Start()
    {
        enemyTr = GetComponent<Transform>();
        ani.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playertr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();  
    }

    private void EnemyAttack()
    {
        ani.SetTrigger(hashAttack);
    }
    private void Update()
    {
        if (isAttack)
        {
            if (Time.time >= nextAttack)
            {
                EnemyAttack();
                nextAttack = Time.time + attackRate + Random.Range(0.0f, 0.3f);
            }
            Quaternion rot = Quaternion.LookRotation(playertr.position - enemyTr.position);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
    }

}
