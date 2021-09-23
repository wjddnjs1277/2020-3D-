using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Moveagent : MonoBehaviour
{
    public List<Transform> waypoint;
    public int nextidx;

    public NavMeshAgent agent_noA;
    private Transform enemyTr;
    private float damping = 1.0f;
    private readonly float patrollSpeed = 1.5f;
    private readonly float traceSpeed = 4.5f;
    private bool __patrolling;
    private bool _Flee;

  
    public bool patrolling_NoA
    {
        get { return __patrolling; }
        set
        {
            __patrolling = value;
            if (__patrolling)
            {
                agent_noA.speed = patrollSpeed;
                damping = 1.0f;
                MoveWayPoint();
            }
        }
    }
    private Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent_noA.speed = traceSpeed;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }
    void TraceTarget(Vector3 pos)
    {
        if (agent_noA.isPathStale) return;
        agent_noA.destination = pos;
        agent_noA.isStopped = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyTr = GetComponent<Transform>();
        agent_noA = GetComponent<NavMeshAgent>();
        agent_noA.autoBraking = false;
        agent_noA.speed = patrollSpeed;
        var group = GameObject.Find("WayPointGroup");
        if (group != null)
        {
            group.GetComponentsInChildren<Transform>(waypoint);
            waypoint.RemoveAt(0);
            nextidx = Random.Range(0, waypoint.Count);
        }
        MoveWayPoint();
    }

    void MoveWayPoint()
    {
        if (agent_noA.isPathStale)
            return;
        agent_noA.destination = waypoint[nextidx].position;
        agent_noA.isStopped = false;

    }
  
    public void Stop()
    {
        agent_noA.isStopped = true;
        agent_noA.velocity = Vector3.zero;
        __patrolling = false;
    }
    public float speed
    {
        get { return agent_noA.velocity.magnitude; }
    }
    // Update is called once per frame
    void Update()
    {
        if (agent_noA.isStopped == false)
        {
            Quaternion rot = Quaternion.LookRotation(agent_noA.desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot,
                Time.deltaTime * damping);
        }


        if (!__patrolling) return;

        if (agent_noA.velocity.sqrMagnitude >= 0.2f * 0.2f && agent_noA.remainingDistance <= 0.5f)
        {
            // nextidx = ++nextidx % waypoint.Count;
            nextidx = Random.Range(0, waypoint.Count);
            MoveWayPoint();
        }
    }
}
