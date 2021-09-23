using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent1 : MonoBehaviour
{
    public List<Transform> waypoint;
    public int nextidx;

    private NavMeshAgent agent;
    private Transform enemyTr;
    private float damping = 1.0f;
    private readonly float patrollSpeed = 1.5f;
    private readonly float traceSpeed = 4.5f;
    private bool _patrolling;
    Quaternion rot;

    public bool patrolling
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
            if(_patrolling)
            {
                agent.speed = patrollSpeed;
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
            agent.speed = traceSpeed;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }
    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;
        agent.destination = pos;
        agent.isStopped = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed = patrollSpeed;
        var group = GameObject.Find("WayPointGroup");
        if(group!= null)
        {
            group.GetComponentsInChildren<Transform>(waypoint);
            waypoint.RemoveAt(0);
            nextidx = Random.Range(0, waypoint.Count);
        }
        MoveWayPoint();
    }

    void MoveWayPoint()
    {
        if (agent.isPathStale)
            return;
        agent.destination = waypoint[nextidx].position;
        agent.isStopped = false;
  
    }
    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
    public float speed
    {
        get { return agent.velocity.magnitude; }
    }
    // Update is called once per frame
    void Update()
    {
        if (agent.isStopped == false)
        {
            rot = Quaternion.LookRotation(agent.desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot,
                Time.deltaTime * damping);
        }


        if (!_patrolling) return;

        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f)
        {
            // nextidx = ++nextidx % waypoint.Count;
            nextidx = Random.Range(0, waypoint.Count);
            MoveWayPoint();
        }
    }
}
