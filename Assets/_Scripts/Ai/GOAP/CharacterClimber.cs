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

    private void Awake()
    {
        controller = GetComponent<MoveTowardsController>();
    }

    protected override void Start()
    {
        base.Start();
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
         
            Debug.Log("Destination reached.");
        }

        UpdateState();
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
                // Hantera normal rörelse
                break;
            case AgentState.Armless:
                // Hantera armless tillstånd
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
        
        controller.SetTarget(destination);
        DestinationSet = true;
        ReachedDestination = false;
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

        searchLocation += Random.Range(-range, range) * transform.forward;
        searchLocation += Random.Range(-range, range) * transform.right;

        float raycastDistance = 10f; 
        LayerMask groundMask = LayerMask.GetMask("Ground"); 

        RaycastHit hitResult;

        List<Vector3> directions = new List<Vector3>()
    {
        Vector3.down,       
        transform.forward,  
        -transform.forward, 
        transform.right,    
        -transform.right    
    };


        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(searchLocation, direction, out hitResult, raycastDistance, groundMask))
            {
                return hitResult.point;
            }
        }

        return searchLocation;
    }

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
