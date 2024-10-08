using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

//public enum EOffmeshLinkStatus
//{
//    NotStarted,
//    InProgress
//}

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterAgent : CharacterBase
{

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;
    private LookAt lookAt;
    //int knockbackHash = Animator.StringToHash("Knockback");
    //int knockbackTriggerHash = Animator.StringToHash("KnockbackTrigger");
    //bool knockback;
    bool wasGrounded;

    public bool isCrawling;
    public bool isBlind;
    public bool isArmless;
    public enum AgentState { Crawl, Blind, Armless, Normal }
    public AgentState agentState = AgentState.Normal;
    [SerializeField] float NearestPointSearchRange = 7f;

    NavMeshAgent agent;
    bool DestinationSet = false;
    bool ReachedDestination = false;
    public EOffmeshLinkStatus OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
    public bool isUpp;
    [SerializeField] float uprightThreshold = 0.9f;
    [SerializeField] float rayDistance = 3.0f;
    private float rotationThreshold = 1f;

    public Vector3 LookDirection; /*{ get; private set; }*/
    public Transform LookPoint;/*{ get; private set; }*/
    [SerializeField] Transform target;
    public PlayerDataSO player;
    CharacterController characterController;

    public bool run;
    public Vector3 debug;
    public enum NavMeshArea
    {
        Walkable = 0,
        NotWalkable = 1,
        Jump = 2,
        Cover = 3
    }
    int coverAreaMask = 1 << (int)NavMeshArea.Cover;

    public bool IsMoving => agent.velocity.magnitude > float.Epsilon;
    public bool AtDestination => ReachedDestination;
    EnemyAI linkedAI;

    internal Animator animator;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
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
    protected override void Start()
    {
        base.Start();

        linkedAI = GetComponent<EnemyAI>();

    }

    protected override void Update()
    {

        base.Update();

        if (isCrawling)
        {
            agentState = AgentState.Crawl;
        }
        else
        {
            agentState = AgentState.Normal;
        }

        //CheckGroundUpright();
        // have a path and near the end point?
        if (!agent.pathPending && !agent.isOnOffMeshLink && DestinationSet && (agent.remainingDistance <= agent.stoppingDistance))
        {
            DestinationSet = false;
            ReachedDestination = true;
            // make agent idle
        }

        UpdateState();


        // are we on an offmesh link?
        if (agent.isOnOffMeshLink)
        {
            // have we started moving along the link
            if (OffMeshLinkStatus == EOffmeshLinkStatus.NotStarted)
            {
                StartCoroutine(FollowOffmeshLink());
            }
        }
    }
    void UpdateState()
    {
        switch (agentState)
        {
            case AgentState.Normal:

            
                
                    SynchronizeAnimatorAndAgent();
                

                break;
            case AgentState.Armless:
                // HandleArmless();
                // should be more carfull 
                break;
            case AgentState.Blind:
                if (isBlind)
                {
                    linkedAI._VisionConeRange = 5f;
                    linkedAI._VisionConeAngle = 20f;
                }
                else
                {
                    linkedAI._VisionConeRange = 60f;
                    linkedAI._VisionConeAngle = 30f;
                }
                break;
            case AgentState.Crawl:

                HandleCrawling();

                break;
        }
    }
    IEnumerator FollowOffmeshLink()
    {
        // animation???

        // start the offmesh link - disable NavMesh agent control
        OffMeshLinkStatus = EOffmeshLinkStatus.InProgress;
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // move along the path
        Vector3 newPosition = transform.position;
        while (!Mathf.Approximately(Vector3.Distance(newPosition, agent.currentOffMeshLinkData.endPos), 0f))
        //  while (Vector3.Distance(newPosition, agent.currentOffMeshLinkData.endPos) > 0.1f)
        {
            newPosition = Vector3.MoveTowards(transform.position, agent.currentOffMeshLinkData.endPos, agent.speed * Time.deltaTime);
            transform.position = newPosition;


            // yield return null;
            yield return new WaitForEndOfFrame();
            // make sure that u are viewing from the game and not the scene, it will not update
            // this is for better animations
        }
        // flag the link as completed
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
        agent.CompleteOffMeshLink();

        // return control the agent
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.updateUpAxis = true;

    }
    public void SetAgentActive(bool active)
    {
       
        agent.enabled = active;
        if (active)
        {
            characterController.height = 1.76f;
            agent.isStopped = false;
        }
        else if(!active) 
        {
            characterController.height = 0.2f;
            isCrawling = true;
            animator.SetBool("crawl", true);
            animator.Play("Base Layer.Crawl");
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


        if (run)
        {
            animator.SetBool("move", shouldMove);
            animator.SetBool("crawl", false);
            animator.SetFloat("vely", agent.velocity.magnitude);

        }
        else
        {
            animator.SetBool("jog", shouldMove);
            animator.SetBool("crawl", false);
            animator.SetFloat("vely", agent.velocity.magnitude);
        }
     

        lookAt.lookAtTargetPosition = agent.steeringTarget + transform.forward;
    }

    //private void SynchronizeAnimatorAndAgent()
    //{
    //    if (!agent.isOnNavMesh)
    //    {
    //        //Debug.LogWarning("agent is not on NavMesh");
    //        return;
    //    }

    //    Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
    //    worldDeltaPosition.y = 0;
    //    // Map 'worldDeltaPosition' to local space
    //    float dx = Vector3.Dot(transform.right, worldDeltaPosition);
    //    float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
    //    Vector2 deltaPosition = new Vector2(dx, dy);

    //    // Low-pass filter the deltaMove
    //    float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
    //    smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);


    //    velocity = smoothDeltaPosition / Time.deltaTime;

    //    float t = Mathf.Max(dy / 3, 1);
    //    float speed = t;
    //    if (agent.remainingDistance <= agent.stoppingDistance)
    //    {
    //        velocity = Vector2.Lerp(Vector2.zero, velocity, agent.remainingDistance - agent.stoppingDistance);

    //        speed = (agent.remainingDistance - agent.stoppingDistance) / agent.stoppingDistance;

    //    }

    //    bool shouldMove = velocity.sqrMagnitude > 0.25f && agent.remainingDistance > agent.stoppingDistance;

    //    animator.SetBool("move", shouldMove);


    //    //this does nothing FIX!!!!
    //    //Vector3 d = agent.steeringTarget - transform.position;
    //    //float ddx = Vector3.Dot(transform.right, d);
    //    //animator.SetFloat("velx", ddx); 
    //    animator.SetFloat("vely", speed);




    //    lookAt.lookAtTargetPosition = agent.steeringTarget + transform.forward;

    //    //float deltaMagnitude = worldDeltaPosition.magnitude;
    //    //if (deltaMagnitude > agent.radius / 2)
    //    //{
    //    //    transform.position = Vector3.Lerp(animator.rootPosition, agent.nextPosition, smooth);
    //    //}
    //}

    public Vector3 PickLocationInRange(float range)
    {
        Vector3 searchLocation = transform.position;
        searchLocation += Random.Range(-range, range) * Vector3.forward;
        searchLocation += Random.Range(-range, range) * Vector3.right;
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(searchLocation, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
        {
            return hitResult.position;
        }
        return transform.position;
    }
    public Vector3 PickCoverInRange(float range)
    {
        Vector3 searchLocation = transform.position;
        searchLocation += Vector3.forward;
        searchLocation += Vector3.right;
        NavMeshHit hitResult;

        if (NavMesh.SamplePosition(searchLocation, out hitResult, NearestPointSearchRange, coverAreaMask))
        {
            //Debug.Log("Found cover at: " + hitResult.position);
            debug = hitResult.position;
            return hitResult.position;
        }
        else
        {
            Debug.LogWarning("No cover found within range.");
        }

        return transform.position;
    }


    protected virtual void CancelCurrentCommand()
    {
        // clear the current path
        agent.ResetPath();

        ReachedDestination = false;
        DestinationSet = false;
        OffMeshLinkStatus = EOffmeshLinkStatus.NotStarted;
    }

    public virtual void MoveTo(Vector3 destination)
    {
        CancelCurrentCommand();

        SetDestination(destination);

    }
    //public void HeardSomthing(Vector3 location)
    //{
    //    if (agentState != AgentState.Attacking || agentState != AgentState.Chasing)
    //    {
    //        agentState = AgentState.Investegate;
    //        //InvestegateBehavior(location);
    //        MoveTo(location);
    //    }

    //}

    public virtual void SetDestination(Vector3 destination)
    {
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(destination, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
        {
            agent.SetDestination(hitResult.position);
            DestinationSet = true;
            ReachedDestination = false;
          
        }
     
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

    public void AttackBehaviour(Vector3 currentTarget, float minAttackRange)
    {

        //if (!currentTarget.transform)
        //{
        //    //swap state?
        //    agentState = AgentState.Idle;
        //    return;
        //}
        if (currentTarget.InRangeOf(transform.position, 2.5f))
        {
            Attack();

            Vector3 dir = currentTarget - transform.position;
            transform.forward = Vector3.MoveTowards(transform.forward, dir, agent.angularSpeed * Time.deltaTime).WithY();
            if (currentTarget.InRangeOf(transform.position, minAttackRange))
            {
                //we are too close = pushback self!
                transform.position -= transform.forward * Time.deltaTime;
            }
        }
        else
        {
            //Debug.Log("Switcfhing startte");
            agent.SetDestination(currentTarget);
            agent.isStopped = false;
            //agentState = AgentState.Chasing;
        }

    }

    public void Attack()
    {
        if (agentState == AgentState.Crawl)
        {
            animator.Play("Base Layer.Crawl");
            StartCoroutine(CrawlAttackCooldown(.5f)); //wait for animation to end instead?
        }
        else
        {
          

            animator.SetInteger("Attack", Random.Range(1, 4));
            StartCoroutine(AttackCooldown(.5f)); //wait for animation to end instead?

          

        }

       
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
        Vector3 targetDelta = target.position - transform.position;
        if (target.TryGetComponent(out IDamageble damageble))
        {
            damageble.TakeDamage(transform.position, targetDelta, 15);
        }
    }
    private IEnumerator AnimationCooldown(int idx, float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetInteger(idx, 0);
        //knockback = false;
    }

    void CheckGroundUpright()
    {
        // climbing
        // this can conflict with ai dead or healing
        RaycastHit hit;


        if (Physics.Raycast(agent.transform.position, Vector3.down, out hit, rayDistance))
        {

            Vector3 groundNormal = hit.normal;


            float dotProduct = Vector3.Dot(groundNormal, Vector3.up);

            if (dotProduct >= uprightThreshold)
            {
                isUpp = true;
            }
            else
            {
                isUpp = false;
            }
        }
        else
        {
            isUpp = false;
        }
    }

    public void SetLookDirection(Vector3 direction)
    {



        Vector3 targetDirection = direction - transform.position;

        float angleToTarget = Vector3.Angle(transform.forward, targetDirection);
        if (angleToTarget > rotationThreshold)
        {

            LookDirection = targetDirection.normalized;

            Quaternion targetRotation = Quaternion.LookRotation(LookDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * agent.angularSpeed);


        }

    }






}

