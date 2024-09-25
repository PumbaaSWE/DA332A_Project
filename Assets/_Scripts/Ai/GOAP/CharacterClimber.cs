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
    [SerializeField] float uprightThreshold = 0.9f;
    [SerializeField] float rayDistance = 3.0f;
    private float rotationThreshold = 1f;

    public Vector3 LookDirection;
    public Transform LookPoint;

    public bool IsMoving => controller.wallClimber.Speed > float.Epsilon;
    public bool AtDestination => ReachedDestination;
    EnemyAI linkedAI;

    internal Animator animator;
    [SerializeField] float NearestPointSearchRange = 7f;
    private NavMeshPath navMeshPath;
    private float pathDistance;

    public WallClimber wallClimber;
    public NavMeshAgent navAgent;
    public Transform newTarget;
    private Vector3 lastPosition;
    private float idleTime = 0f;
    private float idleThreshold = 0.8f;
    private void Awake()
    {
        controller = GetComponent<MoveTowardsController>();
        wallClimber = GetComponent<WallClimber>();
        navMeshPath = new NavMeshPath();
        navAgent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start();
      
        linkedAI = GetComponent<EnemyAI>();
    }
    private float CalculateNavMeshPath(Vector3 destination)
    {
        if (NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, navMeshPath))
        {
            pathDistance = GetPathLength(navMeshPath);
            return pathDistance;
        }
        return Mathf.Infinity;
    }

    private float GetPathLength(NavMeshPath path)
    {
        float totalLength = 0.0f;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            totalLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return totalLength;
    }

    private float CalculateClimbOverDistance(Vector3 wallPosition, Vector3 destination)
    {
        float distanceToWall = Vector3.Distance(transform.position, wallPosition);

        RaycastHit hitTop;
        if (Physics.Raycast(wallPosition, Vector3.forward, out hitTop, Mathf.Infinity, LayerMask.GetMask("Wall")))
        {

            GameObject wallObject = hitTop.collider.gameObject;


            float wallHeight = wallObject.GetComponent<Collider>().bounds.size.y;
            Debug.Log("Wall height: " + wallHeight);

            float distanceAfterClimb = Vector3.Distance(wallPosition + Vector3.up * wallHeight, destination);

            return distanceToWall + wallHeight + distanceAfterClimb;
        }
        else
        {
           
            return Mathf.Infinity;  
        }
    }


    protected override void Update()
    {
        base.Update();

        // CheckGroundUpright();
        if (DestinationSet && IsAtDestination())
        {
            DestinationSet = false;
            ReachedDestination = true;

            Debug.Log("Destination reached.");
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
   
    void WallMovment(Vector3 destination)
    {
        Vector3 wallPosition = Vector3.zero;

        int steps = 10;
        Vector3 direction = (destination - transform.position).normalized;
        float distanceToDestination = Vector3.Distance(transform.position, destination);
        float stepSize = distanceToDestination / steps;

        //for (int i = 1; i <= steps; i++)
        //{
        //    Vector3 stepPosition = transform.position + direction * stepSize * i;
        //    if (Physics.Raycast(stepPosition, direction, out RaycastHit hit, stepSize, LayerMask.GetMask("Wall")))
        //    {
        //        wallPosition = hit.point;
        //        break; 
        //    }
        //}
        //if (wallPosition == Vector3.zero)
        //{
         
        //    SetDestinationNav(destination);
        //    return;
        //}

        //float navMeshPathDistance = CalculateNavMeshPath(destination);
        //float climbOverDistance = CalculateClimbOverDistance(wallPosition, destination);

        //if (climbOverDistance < navMeshPathDistance)
        //{
        //    StartClimbing(wallPosition);
        //}
        //else
        {
            
            SetDestinationNav(destination);
        }
    }
   

    public virtual void SetDestinationNav(Vector3 destination)
    {
        NavMeshHit hitResult;
        if (NavMesh.SamplePosition(destination, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, hitResult.position, NavMesh.AllAreas, path))
            {
                StartCoroutine(FollowNavMeshPath(path));

                DestinationSet = true;
                ReachedDestination = false;
                Debug.Log("Destination set to: " + hitResult.position);
            }
        }
    }

    private IEnumerator FollowNavMeshPath(NavMeshPath path)
    {
        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 corner = path.corners[i];
            Debug.Log("Moving to corner " + i);
    
            NavMeshHit hit;
            if (NavMesh.SamplePosition(corner, out hit, 1f, NavMesh.AllAreas))
            {
                int areaMask = hit.mask;

                if (NavMesh.GetAreaFromName("Wall") == areaMask)
                {
                  
                }
                else if (NavMesh.GetAreaFromName("Walkable") == areaMask)
                {
                    
                }
            }

     
            controller.SetTarget(corner);

            while (Vector3.Distance(transform.position, corner) > controller.stoppingDistance + 0.1f)
            {
                yield return new WaitForSeconds(0.1f);  
            }
        }
    }

    public Vector3 GetClosestNavMeshPoint(Vector3 currentPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(currentPosition, out hit, 10, NavMesh.AllAreas))
        {

            return hit.position;
        }
        else
        {

            return currentPosition;  
        }
    }



    private void StartClimbing(Vector3 wallPosition)
    {
        StartCoroutine(ClimbOverWall(wallPosition));
    }

    private IEnumerator ClimbOverWall(Vector3 wallPosition)
    {
        while (Vector3.Distance(transform.position, wallPosition) > 0.1f)
        {
          
            Vector3 climbDirection = Vector3.up;
            controller.SetTarget(transform.position + climbDirection * Time.deltaTime);

            yield return null;  
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

    public virtual void MoveTo(Vector3 destination)
    {
        CancelCurrentCommand();
        SetDestination(destination);
    }

    public virtual void SetDestination(Vector3 destination)
    {
        WallMovment(destination);
        //controller.SetTarget(destination);
        //DestinationSet = true;
        //ReachedDestination = false;
        Debug.Log("Destination set to: " + destination);
        Debug.Log("Pos is: " + this.transform.position);
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
        if (NavMesh.SamplePosition(searchLocation, out hitResult, NearestPointSearchRange, NavMesh.AllAreas))
        {
            return hitResult.position;
        }
        return transform.position;
    }
    //public Vector3 PickLocationInRange(float range)
    //{
    //    Vector3 searchLocation = transform.position;

    //    searchLocation += Random.Range(-range, range) * transform.forward;
    //    searchLocation += Random.Range(-range, range) * transform.right;

    //    float raycastDistance = 10f; 
    //    LayerMask groundMask = LayerMask.GetMask("Default"); 

    //    RaycastHit hitResult;

    //    List<Vector3> directions = new List<Vector3>()
    //{
    //    Vector3.down,       
    //    transform.forward,  
    //    -transform.forward, 
    //    transform.right,    
    //    -transform.right    
    //};


    //    foreach (Vector3 direction in directions)
    //    {
    //        if (Physics.Raycast(searchLocation, direction, out hitResult, raycastDistance, groundMask))
    //        {
    //            return hitResult.point;
    //        }
    //    }

    //    return searchLocation;
    //}

    public void Attack()
    {
        //temp
        //sword.Orbit();
        //


        //animator.SetInteger("Attack", Random.Range(1, 4));
        //StartCoroutine(AttackCooldown(.5f)); //wait for animation to end instead?
    }
    public void AttackBehaviour(Vector3 currentTarget, float minAttackRange)
    {

        //if (!currentTarget.transform)
        //{
        //    //swap state?
        //    agentState = AgentState.Idle;
        //    return;
        //}
        //if (currentTarget.InRangeOf(transform.position, 2.5f))
        //{
        //    Attack();
        //    Vector3 dir = currentTarget - transform.position;
        //    transform.forward = Vector3.MoveTowards(transform.forward, dir, agent.angularSpeed * Time.deltaTime).WithY();
        //    if (currentTarget.InRangeOf(transform.position, minAttackRange))
        //    {
        //        //we are too close = pushback self!
        //        transform.position -= transform.forward * Time.deltaTime;
        //    }
        //}
        //else
        //{
        //    //Debug.Log("Switcfhing startte");
        //    agent.SetDestination(currentTarget);
        //    agent.isStopped = false;
        //    //agentState = AgentState.Chasing;
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

    void CheckGroundUpright()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayDistance))
        {
            Vector3 groundNormal = hit.normal;
            float dotProduct = Vector3.Dot(groundNormal, Vector3.up);
            isUpp = dotProduct >= uprightThreshold;
        }
        else
        {
            isUpp = false;
        }
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
