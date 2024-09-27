using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider cc;
    [SerializeField] NavMeshAgent agent;


    public enum AgentStatus { Physics, NavMesh }
    public AgentStatus status;
    public bool statusSwitched;

    private void Awake()
    {
        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }



    void Start()
    {
        
    }


    public void SwitchStatus(AgentStatus status)
    {
        this.status = status;
        statusSwitched = true;
    }
    // Update is called once per frame
    void Update()
    {
        switch (status)
        {
            case AgentStatus.Physics:
                PhysicsBehaviour();
                break;
            case AgentStatus.NavMesh:
                NavMeshBehaviour();
                break;
            default:
                break;
        }
    }

    private void PhysicsBehaviour()
    {
        if (statusSwitched)
        {
            statusSwitched = false;
        }
        if (Physics.Raycast(transform.position, Vector3.down, .2f))
        {
            SwitchStatus(AgentStatus.NavMesh);
            rb.useGravity = false;
            agent.enabled = true;
        }
    }

    private void NavMeshBehaviour()
    {
        if (statusSwitched)
        {
            statusSwitched = false;
        }

        if(!Physics.Raycast(transform.position, Vector3.down, .2f))
        {
            SwitchStatus(AgentStatus.Physics);
            rb.useGravity = true;
            agent.enabled = false;
        }
    }
}
