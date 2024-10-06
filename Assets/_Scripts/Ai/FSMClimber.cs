using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;



public class FSMClimber : MonoBehaviour
{
    //public GameObject testObejct;
    public Transform targetPosition;  
    public float flySpeed = 5f;      
    public float rotationSpeed = 2f;  
    private bool hasReachedDestination = false;
    EnemyDeath enemyDeath;
    RagdollController ragController;


    AwarenessSystem awarenessSystem;
    [SerializeField] float detectedAwarness = 1.2f;
    DetectableTarget currentTarget;
    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;
    [SerializeField] float searchRange = 5f;
    public bool isCrawling;
    public bool isBlind;
    public bool isArmless;
    public enum AgentHit { Crawl, Blind, Armless, Normal }
    public AgentHit agenthitState = AgentHit.Normal;
    public enum AgentState { Idle, Wander, Patrol, Chasing, Attacking, Knockback, Investegate , Jump}
    public AgentState agentState = AgentState.Idle;
    MoveTowardsController controller;
    bool DestinationSet = false;
    bool ReachedDestination = true;
    public bool isUpp;
    //[SerializeField] float uprightThreshold = 0.9f;
    //[SerializeField] float rayDistance = 3.0f;
    private float rotationThreshold = 1f;
    Vector3 soundLocation;
    public Vector3 LookDirection;
    public Transform LookPoint;
    Vector3 avoidThis;
    bool avoidPlayer;

    public bool IsMoving => controller.wallClimber.Speed > float.Epsilon;
    public bool atDestination => ReachedDestination;
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
    public float attackRange = 2.0f;
    public float minAttackRange = 1.0f;

    [SerializeField] float patrolPointReachedThreshold = 0.5f;
    [SerializeField] List<Transform> patrolPoints;
    int currentPatrolPoint;
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
        enemyDeath = GetComponent<EnemyDeath>();
        awarenessSystem = GetComponent<AwarenessSystem>();
        controller = GetComponent<MoveTowardsController>();
        ragController = GetComponentInChildren<RagdollController>();
        wallClimber = GetComponent<WallClimber>();
        navMeshPath = new NavMeshPath();
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }


    public  void Start()
    {
        
        player.NotifyOnPlayerChanged(OnPlayer);
        linkedAI = GetComponent<EnemyAI>();
    }
    public  void Update()
    {
        Found();
        CheckIfAIsAboveAndFar();
        //if(hasReachedDestination)
        //{
        //    agentState = AgentState.Idle;
        //}
        // CheckGroundUpright();
        if (DestinationSet && IsAtDestination())
        {
            DestinationSet = false;
            ReachedDestination = true;

            //Debug.Log("Destination reached.");
        }

        if (CheckIfAIsAboveAndFar())
        {
            agentState = AgentState.Jump;
        }

        //if (!navAgent.pathPending && !navAgent.isOnOffMeshLink && DestinationSet && (navAgent.remainingDistance <= navAgent.stoppingDistance))
        //{
        //    DestinationSet = false;
        //    ReachedDestination = true;
        //    // make agent idle
        //}

        //if (DestinationSet)
        //{
        //    if (transform.position == lastPosition)
        //    {
        //        idleTime += Time.deltaTime;
        //    }
        //    else
        //    {
        //        idleTime = 0f;
        //    }
        //}

        //if (idleTime > idleThreshold)
        //{

        //    Vector3 randomDirection = Random.insideUnitSphere * 2;
        //    randomDirection += transform.position;
        //    SetDestinationNav(randomDirection);
        //}
        lastPosition = transform.position;
        UpdateState();
        if (awarenessSystem.GetSoundPos() != Vector3.zero)
        {
            soundLocation = awarenessSystem.GetSoundPos();
        }
        else
        {
            soundLocation = Vector3.zero;
        }
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
        switch (agenthitState)
        {
            case AgentHit.Normal:

                break;
            case AgentHit.Armless:

                break;
            case AgentHit.Blind:
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
            case AgentHit.Crawl:
                if (isCrawling)
                {
                    
                }
                break;
        }

        switch (agentState)
        {
            case AgentState.Idle:
                IdleBehaviour();
                break;
            case AgentState.Wander:
                WanderBehavior();
                break;
            case AgentState.Patrol:
              UpdateStatePatrolling();
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
            case AgentState.Jump:
                JumpBehavior();
                break;

        }
    }
    private void IdleBehaviour()
    {
        idleTime = Random.Range(5f, 15f);
        StartCoroutine(IdleTimer(idleTime));
    }
    private IEnumerator IdleTimer(float time)
    {
        yield return new WaitForSeconds(time);
        agentState = AgentState.Wander;

    }
    private void WanderBehavior()
    {
        if (atDestination)
        {
            Vector3 location = PickLocationInRange(searchRange);
            MoveTo(location,false);
        }
    }

    void UpdateStatePatrolling()
    {
        Vector3 vectorToPatrolPoint = patrolPoints[currentPatrolPoint].position - transform.position;

        // reached patrol ponit
        if (vectorToPatrolPoint.magnitude <= patrolPointReachedThreshold)
        {
            //next point 
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Count;
        }
        // move towards the point
        if (atDestination)
        {

            MoveTo(patrolPoints[currentPatrolPoint].position, false);
        }

        //test
        //if(currentPatrolPoint == 5)
        //{
        //    agentState = AgentState.Jump;
        //}
   
    }
    public void HeardSomthing(Vector3 location)
    {
        if (agentState != AgentState.Attacking || agentState != AgentState.Chasing)
        {
            soundLocation = location;
            agentState = AgentState.Investegate;
            // 
            //MoveTo(location);
        }

    }
    private void InvestegateBehavior()
    {
        MoveTo(soundLocation, false);
    
    }

    private void ChaseBehaviour()
    {
        if (currentTarget.transform)
        {


            if (!transform.position.InRangeOf(currentTarget.transform.position, minAttackRange))
            {
                MoveTo(currentTarget.transform.position, false);
                //agent.SetDestination(currentTarget.transform.position);
            }
            if (transform.position.InRangeOf(currentTarget.transform.position, attackRange))
            {
                agentState = AgentState.Attacking;
                //agent.isStopped = true;
            }

        }


    }
    bool CheckIfAIsAboveAndFar()
    {
       
        Vector3 horizontalDistance = new Vector3(transform.position.x, 0, transform.position.z) -
                                     new Vector3(player.PlayerTransform.position.x, 0, player.PlayerTransform.position.z);
        float distance = horizontalDistance.magnitude;

    
        float heightDifference = transform.position.y - player.PlayerTransform.position.y;

        //if(currentTarget != null)
        {
            if (distance <= 10f && heightDifference > 0)
            {
                return true;
            }
        }
        return false;
       
     
    }
    public void JumpBehavior()
    {
        MoveTo(targetPosition.position, false);
        animator.SetBool("Jump", true);
        //animator.Play("Base Layer.Jump", 0, 1f);
        //animator.speed = -1;

        Vector3 adjustedTargetPosition = new Vector3(
       player.PlayerTransform.position.x,               
       player.PlayerTransform.position.y ,          
       player.PlayerTransform.position.z              
   );


        transform.position = Vector3.MoveTowards(transform.position, adjustedTargetPosition, flySpeed * Time.deltaTime);
        Vector3 direction = (player.PlayerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, player.PlayerTransform.position) < 0.1f)
        {
            animator.SetBool("Jump", false);
            ragController.EnableRagdoll();
            hasReachedDestination = true;
            ragController.DisableRagdoll();
            agentState = AgentState.Wander;
        }
    }
    void Found()
    {
        if (awarenessSystem.ActiveTargets == null || awarenessSystem.ActiveTargets.Count == 0)
            return;

        foreach (var candidate in awarenessSystem.ActiveTargets.Values)
        {
            if (candidate.detectable != null)
            {
                if (candidate.Awarness >= detectedAwarness && agentState != AgentState.Attacking)
                {
                    //CancelCurrentCommand();
                    currentTarget = candidate.detectable;
                    agentState = AgentState.Chasing;

                }
            }
        }
    }
    private void AttackBehaviour()
    {
        animator.Play("Base Layer.Crawl");
        if (!currentTarget.transform)
        {
            //swap state?
            animator.SetBool("Biting", false);
            agentState = AgentState.Idle;
            return;
        }
        if (currentTarget.transform.position.InRangeOf(transform.position, attackRange))
        {
            Attack();
            //Vector3 dir = currentTarget.transform.position - transform.position;
            
            //if (currentTarget.transform.position.InRangeOf(transform.position, minAttackRange))
            //{
               

            //}
        }
        else
        {
            animator.SetBool("Biting", false);

            agentState = AgentState.Chasing;
        }

    }

    public void Attack()
    {
        animator.SetBool("Biting", true);
    }


    public virtual void MoveTo(Vector3 destination, bool avoidPlayer)
    {
        CancelCurrentCommand();
        SetDestination(destination);

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

}
