using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : MonoBehaviour
{
    //private NavMeshTriangulation triangulation;
    private NavMeshAgent agent;
    private Animator animator;

    public Transform target;
    public float attackRange = 2.5f;
    public float minAttackRange = 1.0f;

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;
    private LookAt lookAt;

    public enum AgentState { Idle, Chasing, Attacking, Knockback}
    public AgentState agentState = AgentState.Idle;

    int knockbackHash = Animator.StringToHash("Knockback");
    int knockbackTriggerHash = Animator.StringToHash("KnockbackTrigger");
    bool knockback;
    bool wasGrounded;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lookAt = GetComponent<LookAt>();
        //triangulation = NavMesh.CalculateTriangulation();
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
        Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = true;
        }

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SynchronizeAnimatorAndAgent();

        switch (agentState)
        {
            case AgentState.Idle:
                IdleBehaviour();
                break;
            case AgentState.Chasing:
                ChaseBehaviour();
                break;
            case AgentState.Attacking:
                AttackBehaviour();
                break;
            case AgentState.Knockback:
                KnockbackBehaviour();
                break;
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Attack();
        //}
    }
    public void SetAgentActive(bool active)
    {
        agent.enabled = active;
        if (active)
        {
            agent.isStopped = false;
        }
    }
    private void KnockbackBehaviour()
    {
        if (target)
        {
            agentState = AgentState.Attacking;
        }
        else
        {

            agentState = AgentState.Idle;
        }
    }

    private void IdleBehaviour()
    {
        if (target)
        {
            agentState = AgentState.Chasing;
        }
    }

    private void ChaseBehaviour()
    {
        if (target && agent.isOnNavMesh) 
        {
            if (!agent.pathEndPosition.InRangeOf(target.position, minAttackRange))
            {
                agent.SetDestination(target.position); 
            }
            if (transform.position.InRangeOf(target.position, attackRange))
            {
                agentState = AgentState.Attacking;
                agent.isStopped = true;
            }
        }
        else
        {
            //Debug.LogWarning("agent is not on NavMesh "); 
        }
    }

    private void SynchronizeAnimatorAndAgent()
    {
        if (!agent.isOnNavMesh)
        {
            //Debug.LogWarning("agent is not on NavMesh");
            return;
        }

        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;
        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);


        velocity = smoothDeltaPosition / Time.deltaTime;

        float t = Mathf.Max(dy/3, 1);
        float speed = t;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            velocity = Vector2.Lerp(Vector2.zero, velocity, agent.remainingDistance - agent.stoppingDistance);

            speed = (agent.remainingDistance - agent.stoppingDistance) / agent.stoppingDistance;

        }

        bool shouldMove = velocity.sqrMagnitude > 0.25f && agent.remainingDistance > agent.stoppingDistance;

        animator.SetBool("move", shouldMove);


        //this does nothing FIX!!!!
        //Vector3 d = agent.steeringTarget - transform.position;
        //float ddx = Vector3.Dot(transform.right, d);
        //animator.SetFloat("velx", ddx); 
        animator.SetFloat("vely", speed);

   
        

        lookAt.lookAtTargetPosition = agent.steeringTarget + transform.forward;

        //float deltaMagnitude = worldDeltaPosition.magnitude;
        //if (deltaMagnitude > agent.radius / 2)
        //{
        //    transform.position = Vector3.Lerp(animator.rootPosition, agent.nextPosition, smooth);
        //}
    }

    //public void Knockback(Vector3 from, Vector3 direction)
    //{
    //    //RagdollLims pedestrian = GetComponent<RagdollLims>();
    //    //pedestrian.TriggerRagdoll(direction * 10, transform.position);

    //    if (knockback) return;
    //    if (Vector3.Dot(transform.forward, direction) < 0)
    //    {
    //        animator.SetInteger(knockbackHash, 2);
    //    }
    //    else
    //    {
    //        animator.SetInteger(knockbackHash, 3);
    //    }
    //    knockback = true;
    //    animator.SetTrigger(knockbackTriggerHash);
    //    StartCoroutine(AnimationCooldown(knockbackHash, .9f));
    //}

    private void AttackBehaviour()
    {
        if (!target)
        {
            //swap state?
            agentState = AgentState.Idle;
            return;
        }
        if(target.position.InRangeOf(transform.position, attackRange))
        {
            Attack();
            Vector3 dir = target.position - transform.position;
            transform.forward = Vector3.MoveTowards(transform.forward, dir, agent.angularSpeed * Time.deltaTime).WithY();
            if(target.position.InRangeOf(transform.position, minAttackRange))
            {
                //we are too close = pushback self!
                transform.position -= transform.forward * Time.deltaTime;
            }
        }
        else
        {
            //Debug.Log("Switcfhing startte");
            agent.SetDestination(target.position);
            agent.isStopped = false;
            agentState = AgentState.Chasing;
        }
        
    }

    public void Attack()
    {
        animator.SetInteger("Attack", Random.Range(1, 4));
        StartCoroutine(AttackCooldown(.5f)); //wait for animation to end instead?
    }

    private IEnumerator AttackCooldown(float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetInteger("Attack", 0);
    }
    private IEnumerator AnimationCooldown(int idx, float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetInteger(idx, 0);
        knockback = false;
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;

        rootPosition.y = agent.nextPosition.y;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc)
        {
            bool grouned = cc.SimpleMove((rootPosition - transform.position) / Time.deltaTime);

            if (grouned)
            {
                if (!wasGrounded)
                {
                    //agent.nextPosition = transform.position;
                    Debug.Log("Warping!!!!!!!!!!!");
                    agent.Warp(transform.position);
                }
                else
                {
                    transform.position = transform.position.WithY(agent.nextPosition.y);
                    agent.nextPosition = transform.position;
                }
                wasGrounded = true;
            }
            else
            {
                wasGrounded = false;
                //agent.Warp(transform.position);
                //Debug.Log("NOT GROUNDED");
                agent.nextPosition = transform.position;
            }
        }
        else
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.MovePosition(rootPosition);
            agent.nextPosition = rootPosition;
        }
        
        //agent.Warp(transform.position);
        //agent.nextPosition = transform.position;
    }

    public void OnDrawGizmos()
    {
        if (!agent) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(agent.nextPosition, Vector3.one * 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, Vector3.one * .9f);
    }
}
