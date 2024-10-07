using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static UnityEditor.FilePathAttribute;
public enum EOffmeshLinkStatus
{
    NotStarted,
    InProgress
}

public class FSM : MonoBehaviour
{
    //EOffmeshLinkStatus OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
    protected AwarenessSystem sensors;
    DetectableTarget currentTarget;
    [SerializeField] float detectedAwarness = 1.2f;
    private float nearestPointSearchRange = 7f;
    [SerializeField] float searchRange = 5f;
    private float idleTime;
    bool destinationSet = true;
    bool reachedDestination = false;
    public bool atDestination => reachedDestination;
    //private NavMeshTriangulation triangulation;
    private NavMeshAgent agent;
    private Animator animator;

    //public Transform target;
    public float attackRange = 2.5f;
    public float minAttackRange = 1.0f;

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;
    private LookAt lookAt;

    public enum AgentState { Idle, Wander ,Chasing, Attacking, Knockback, Investegate }
    public AgentState agentState = AgentState.Idle;
    public enum AgentHit { Crawl, Blind, Armless, Normal }
    public AgentHit agentStatehit = AgentHit.Normal;
    int knockbackHash = Animator.StringToHash("Knockback");
    int knockbackTriggerHash = Animator.StringToHash("KnockbackTrigger");
    //bool knockback;
    bool wasGrounded;
    public bool isCrawling;
    Vector3 soundLocation;
    CharacterController characterController;
    public bool run;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
     
        sensors = GetComponent<AwarenessSystem>();
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

    void Update()
    {
        
        if (!agent.pathPending && !agent.isOnOffMeshLink && destinationSet && (agent.remainingDistance <= agent.stoppingDistance))
        {
            destinationSet = false;
            reachedDestination = true;
            agentState = AgentState.Idle;
        }
        //SynchronizeAnimatorAndAgent();

        Found();
        UpdateState();

        //hearingSensor.OnHeardSound()

        //HeardSomthing(Sensors.soundLocation)
        if (isCrawling)
        {
            agentStatehit = AgentHit.Crawl;
        }
        else {
            agentStatehit = AgentHit.Normal;
        }

    }

    void UpdateState()
    {
        switch (agentState)
        {
            case AgentState.Idle:
                IdleBehaviour();
                break;
            case AgentState.Wander:
                WanderBehavior();
                break;
            case AgentState.Investegate:
                InvestegateBehavior();
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

        switch (agentStatehit)
        {
            case AgentHit.Normal:

                //characterController.height = 1.76f;
                
                SynchronizeAnimatorAndAgent();


                break;
            case AgentHit.Armless:
                // HandleArmless();
                // should be more carfull 
                break;
            case AgentHit.Blind:
                //if (isBlind)
                //{
                //    linkedAI._VisionConeRange = 5f;
                //    linkedAI._VisionConeAngle = 20f;
                //}
                //else
                //{
                //    linkedAI._VisionConeRange = 60f;
                //    linkedAI._VisionConeAngle = 30f;
                //}
                break;
            case AgentHit.Crawl:
                //characterController.height = 0.05f;
                HandleCrawling();

                break;
        }
    }

    void Found()
    {
        if (sensors.ActiveTargets == null || sensors.ActiveTargets.Count == 0 )
            return;

        foreach (var candidate in sensors.ActiveTargets.Values)
        {
            if (candidate.detectable != null)
            {
                if(candidate.Awarness >= detectedAwarness && agentState != AgentState.Attacking)
                {
                    //CancelCurrentCommand();
                    currentTarget = candidate.detectable;                   
                    agentState = AgentState.Chasing;

                }
            }
        }
    }
   

    public void HeardSomthing(Vector3 location)
    {
        if(agentState != AgentState.Attacking || agentState != AgentState.Chasing)
        {
            soundLocation = location;
            agentState = AgentState.Investegate;
            // 
           //MoveTo(location);
        }
        
    }
    public void SetAgentActive(bool active)
    {

        agent.enabled = active;
        if (active)
        {
            //characterController.height = 1.76f;
            agent.isStopped = false;
        }
        else if (!active)
        {
            //characterController.height = 0.05f;
            isCrawling = true;
            animator.SetBool("crawl", true);
            animator.Play("Base Layer.Crawl");
        }
    }
  
    private void KnockbackBehaviour()
    {
        if (currentTarget.transform)
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
            idleTime = Random.Range(10f, 25f); 
            StartCoroutine(IdleTimer(idleTime));       
    }
    private IEnumerator IdleTimer(float time)
    {
        yield return new WaitForSeconds(time); 
        agentState = AgentState.Wander;

    }
    private void WanderBehavior()
    {
       if(atDestination)
       {
            Vector3 location = PickLocationInRange(searchRange);
            MoveTo(location);
       }
    }
    private void InvestegateBehavior()
    {
        MoveTo(soundLocation);
       
    }
    public Vector3 PickLocationInRange(float range)
    {
        Vector3 searchLocation = transform.position;
        searchLocation += Random.Range(-range, range) * Vector3.forward;
        searchLocation += Random.Range(-range, range) * Vector3.right;
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(searchLocation, out hitResult, nearestPointSearchRange, NavMesh.AllAreas))
        {
            return hitResult.position;
        }
        return transform.position;
    }

    protected virtual void CancelCurrentCommand()
    {
        // clear the current path
        agent.ResetPath();

        reachedDestination = false;
        destinationSet = false;
        //OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
    }

    public virtual void MoveTo(Vector3 destination)
    {
      
        if (agent.isOnNavMesh)
        {
            CancelCurrentCommand();
            SetDestination(destination);
        }
    }

    public virtual void SetDestination(Vector3 destination)
    {
        // find nearest spot on navmesh and move there
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(destination, out hitResult, nearestPointSearchRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hitResult.position);
            destinationSet = true;
            reachedDestination = false;
        }
    }






    private void ChaseBehaviour()
    {
        if (currentTarget.transform)
        {
            if (agent.isOnNavMesh)
            {
                if (!agent.pathEndPosition.InRangeOf(currentTarget.transform.position, minAttackRange))
                {
                    agent.SetDestination(currentTarget.transform.position);
                }
                if (transform.position.InRangeOf(currentTarget.transform.position, attackRange))
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
      
        
    }
   
    private void HandleCrawling()
    {
        if (!agent.isOnNavMesh)
        {
            //Debug.LogWarning("Agent is not on NavMesh");
            return;
        }

        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;

        float manualRemainingDistance = Vector3.Distance(agent.transform.position, agent.destination);


        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        velocity = smoothDeltaPosition / Time.deltaTime;


        bool shouldMove = velocity.sqrMagnitude > 0.25f && manualRemainingDistance > agent.stoppingDistance;

        animator.SetBool("crawl", shouldMove);
        animator.SetBool("move", false);

        animator.SetFloat("vely", agent.velocity.magnitude);
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

        float t = Mathf.Max(dy / 3, 1);
        float speed = t;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            velocity = Vector2.Lerp(Vector2.zero, velocity, agent.remainingDistance - agent.stoppingDistance);

            speed = (agent.remainingDistance - agent.stoppingDistance) / agent.stoppingDistance;

        }

        bool shouldMove = velocity.sqrMagnitude > 0.25f && agent.remainingDistance > agent.stoppingDistance;
        animator.SetBool("crawl", false);
        if (run)
        {
            animator.SetBool("move", shouldMove);
        }
        else
        {
            animator.SetBool("jog", shouldMove);
        }



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
        if (!currentTarget.transform)
        {
            //swap state?
            agentState = AgentState.Idle;
            return;
        }
        if (currentTarget.transform.position.InRangeOf(transform.position, attackRange))
        {
            Attack();
            Vector3 dir = currentTarget.transform.position - transform.position;
            transform.forward = Vector3.MoveTowards(transform.forward, dir, agent.angularSpeed * Time.deltaTime).WithY();
            if (currentTarget.transform.position.InRangeOf(transform.position, minAttackRange))
            {
                //we are too close = pushback self!
                transform.position -= transform.forward * Time.deltaTime;
            }
        }
        else
        {
            //Debug.Log("Switcfhing startte");
            agent.SetDestination(currentTarget.transform.position);
            agent.isStopped = false;
            agentState = AgentState.Chasing;
        }

    }

    public void Attack()
    {
        if (agentStatehit == AgentHit.Crawl)
        {
            animator.Play("Base Layer.Crawl");
            StartCoroutine(CrawlAttackCooldown(.5f)); //wait for animation to end instead?
        }
        animator.SetInteger("Attack", Random.Range(1, 4));
        StartCoroutine(AttackCooldown(.5f)); //wait for animation to end instead?
    }
    private IEnumerator CrawlAttackCooldown(float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetBool("CrawlAttack", true);
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
        //knockback = false;
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
                    //Debug.Log("Warping!!!!!!!!!!!");
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
