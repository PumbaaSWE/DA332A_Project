using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class CharacterClimber : CharacterBase
{
    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;

    public bool isCrawling;
    public bool isBlind;
    public bool isArmless;
    public enum AgentState { Crawl, Blind, Armless, Normal }
    public AgentState agentState = AgentState.Normal;

    MoveTowardsController controller;
    bool DestinationSet = false;
    bool ReachedDestination = false;
    public bool isUpp;
    //[SerializeField] float uprightThreshold = 0.9f;
    //[SerializeField] float rayDistance = 3.0f;
    private float rotationThreshold = 1f;

    public Vector3 LookDirection;
    public Transform LookPoint;
    Vector3 avoidThis;
    bool avoidPlayer;

    public bool IsMoving => controller.wallClimber.Speed > float.Epsilon;
    public bool AtDestination => ReachedDestination;
    EnemyAI linkedAI;

    internal Animator animator;
    [SerializeField] float NearestPointSearchRange = 3f;
    private NavMeshPath navMeshPath;
    private float pathDistance;

    public WallClimber wallClimber;
    public NavMeshAgent navAgent;
    public Transform newTarget;
    private Vector3 lastPosition;
    private float idleTime = 0f;
    private float idleThreshold = 0.8f;
    [SerializeField] Transform target;
    public PlayerDataSO player;
   
    private void OnDestroy()
    {
        player.UnsubscribeOnPlayerChanged(OnPlayer);
    }
    private void OnPlayer(Transform obj)
    {
        target = obj;
        if (target)
        {

            //do if not null
        }
    }
    private void Awake()
    {
        controller = GetComponent<MoveTowardsController>();
        wallClimber = GetComponent<WallClimber>();
        navMeshPath = new NavMeshPath();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }


    protected override void Start()
    {
        base.Start();
        player.NotifyOnPlayerChanged(OnPlayer);
        linkedAI = GetComponent<EnemyAI>();
    }
 
    protected override void Update()
    {
        base.Update();

        // CheckGroundUpright();
        if (DestinationSet && IsAtDestination())
        {
            DestinationSet = false;
            ReachedDestination = true;

            //Debug.Log("Destination reached.");
        }

        //if (!navAgent.pathPending && !navAgent.isOnOffMeshLink && DestinationSet && (navAgent.remainingDistance <= navAgent.stoppingDistance))
        //{
        //    DestinationSet = false;
        //    ReachedDestination = true;
        //    // make agent idle
        //}
        if(DestinationSet)
        {
            if (transform.position == lastPosition)
            {
                idleTime += Time.deltaTime;
            }
            else
            {
                idleTime = 0f;
            }
        }
       
        if ( idleTime > idleThreshold)
        {

            Vector3 randomDirection = Random.insideUnitSphere * 2;
            randomDirection += transform.position;
            SetDestinationNav(randomDirection);
        }
        lastPosition = transform.position;
        UpdateState();
    }
   
 
    public virtual void SetDestinationNav(Vector3 destination)
    {
        NavMeshHit hitResult;
        int avoidLinkMask = ~NavMesh.GetAreaFromName("Link"); 
        if (NavMesh.SamplePosition(destination, out hitResult, NearestPointSearchRange, avoidLinkMask))
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, hitResult.position, avoidLinkMask, path))
            {
                StartCoroutine(FollowNavMeshPath(path));

                DestinationSet = true;
                ReachedDestination = false;
               // Debug.Log("Destination set to: " + hitResult.position);
            }
        }
    }

    private IEnumerator FollowNavMeshPath(NavMeshPath path)
    {
        

        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 corner = path.corners[i];

            
            float currentDistanceToThreat = Vector3.Distance(transform.position, player.PlayerTransform.position);
            float cornerDistanceToThreat = Vector3.Distance(corner, player.PlayerTransform.position);

            // Kontrollera om detta hörn skulle leda till att karaktären kommer närmare spelaren
            if (cornerDistanceToThreat < currentDistanceToThreat && avoidPlayer == true)
            {
                Debug.Log("This corner brings the character closer to the player. Stopping movement.");
                yield break; 
            }

            NavMeshHit hit;
            if (NavMesh.SamplePosition(corner, out hit, 1f, NavMesh.AllAreas))
            {
                int areaMask = hit.mask;
            }

            controller.SetTarget(corner);

            // Vänta tills karaktären når hörnet innan vi går vidare till nästa hörn
            while (Vector3.Distance(transform.position, corner) > controller.stoppingDistance + 0.1f)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }



    private bool IsAtDestination()
    {
        float sqrDistanceToDestination = (transform.position - controller.point).sqrMagnitude;
        float sqrStoppingDistance = controller.stoppingDistance * controller.stoppingDistance;

        if (sqrDistanceToDestination <= sqrStoppingDistance)
        {
            return true;
        }

        return false;
    }

    void UpdateState()
    {
        switch (agentState)
        {
            case AgentState.Normal:
      
                break;
            case AgentState.Armless:
           
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
                if (isCrawling)
                {
                    // Hantera crawling
                }
                break;
        }
    }

    public virtual void MoveTo(Vector3 destination, bool avoidPlayer)
    {
        CancelCurrentCommand();
        SetDestination(destination);
       
    }

    public virtual void SetDestination(Vector3 destination)
    {
        Vector3 wallPosition = Vector3.zero;

        int steps = 10;
        Vector3 direction = (destination - transform.position).normalized;
        float distanceToDestination = Vector3.Distance(transform.position, destination);
        float stepSize = distanceToDestination / steps;

        SetDestinationNav(destination);
        //controller.SetTarget(destination);
        //DestinationSet = true;
        //ReachedDestination = false;
        //Debug.Log("Destination set to: " + destination);
        //Debug.Log("Pos is: " + this.transform.position);
    }

    protected virtual void CancelCurrentCommand()
    {
        controller.ResetTarget();
        ReachedDestination = false;
        DestinationSet = false;
    }
    public Vector3 PickLocationInRange(float range)
    {
        Vector3 searchLocation = transform.position;
        searchLocation += Random.Range(-range, range) * Vector3.forward;
        searchLocation += Random.Range(-range, range) * Vector3.right;

        NavMeshHit hitResult;

   
        int avoidLinkMask = ~NavMesh.GetAreaFromName("Link"); 

        if (NavMesh.SamplePosition(searchLocation, out hitResult, NearestPointSearchRange, avoidLinkMask))
        {
            return hitResult.position;
        }

        return transform.position;
    }

   

    public void Attack()
    {
        //temp
        //sword.Orbit();
        //
        // animator.SetBool("Attack", true);

        //animator.SetInteger("Attack", Random.Range(1, 4));
        /*StartCoroutine(AttackCooldown(.5f));*/ //wait for animation to end instead?
    }
    public void AttackBehaviour(Vector3 currentTarget, float minAttackRange)
    {

        //if (!currentTarget.transform)
        //{
        //    swap state?
        //    agentState = AgentState.Idle;
        //    return;
        //}
        //if (currentTarget.InRangeOf(transform.position, 1.5f))
        //{
        //    Attack();
        //    Vector3 dir = currentTarget - transform.position;
        //    //transform.forward = Vector3.MoveTowards(transform.forward, dir, agent.angularSpeed * Time.deltaTime).WithY();
        //    if (currentTarget.InRangeOf(transform.position, minAttackRange))
        //    {
        //        //  we are too close = pushback self!
        //        transform.position -= transform.forward * Time.deltaTime;
        //    }
        //}
        //else
        //{
        //    animator.SetBool("Attack", false);
        //}
        //else
        //{
        //    Debug.Log("Switcfhing startte");
        //    agent.SetDestination(currentTarget);
        //    agent.isStopped = false;
        //    agentState = AgentState.Chasing;
        //}

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

    public void SetLookDirection(Vector3 direction)
    {
        Vector3 targetDirection = (direction - transform.position).normalized;
        targetDirection.y = 0;

        float angleToTarget = Vector3.Angle(transform.forward, targetDirection);

        if (angleToTarget > rotationThreshold)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * controller.wallClimber.Speed);
        }
    }
   
}
