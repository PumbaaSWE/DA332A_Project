using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FSM_Walker : MonoBehaviour
{
    [Header("Sensors")]
    protected AwarenessSystem sensors;
    DetectableTarget currentTarget;
    [SerializeField] float detectedAwarness = 1.2f;
    private float nearestPointSearchRange = 7f;


    [Header("Nav")]
    [SerializeField] float searchRange = 5f;
    private float idleTime;
    bool destinationSet = true;
    bool reachedDestination = false;
    public bool atDestination => reachedDestination;
    //private NavMeshTriangulation triangulation;
    private NavMeshAgent agent;


    //public Transform target;

    // Attack
    public float attackRange = 2.5f;
    public float minAttackRange = 1.0f;

    // Crawl Attack
    public float attackCrawlRange = 1.0f;
    public float minAttackCrawlRange = .3f;

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;
    private LookAt lookAt;

    // States
    public enum AgentState { Idle, Wander, Chasing, Attacking, Investegate, Sleep }
    public AgentState agentState = AgentState.Idle;
    private AgentState previousState = AgentState.Idle;
    private Vector3 previousTargetPosition;



    // Animation
    string moveBool;
    string crawlBool;
    private Animator animator;

    int nrAttack;

    bool wasGrounded;
    public bool isCrawling;
    public bool isArmles;
    Vector3 soundLocation;
    CharacterController characterController;
    public bool run;
    [SerializeField] Transform target;
    public PlayerDataSO player;
    float attckTimer;
    Eye eye;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        eye = GetComponent<Eye>();
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
        player.NotifyOnPlayerChanged(OnPlayer);
    }


    void Update()
    {
        if (agent.isOnNavMesh)
        {
            if (!agent.pathPending && !agent.isOnOffMeshLink && destinationSet && (agent.remainingDistance <= agent.stoppingDistance))
            {
                destinationSet = false;
                reachedDestination = true;
                agentState = AgentState.Idle;
            }
        }


        if (previousState != agentState)
        {
            destinationSet = false;
            reachedDestination = true;
            previousState = agentState;
        }


        Found();
        UpdateState();


    }


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
    void UpdateState()
    {

        switch (agentState)
        {
            case AgentState.Idle:
                IdleBehaviour();
                break;
            case AgentState.Sleep:
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

        }

        if (isCrawling)
        {
            HandleCrawling();
        }


    }
    void Blind()
    {

        if (isCrawling)
        {
            HandleCrawling();
        }
        else
        {
            moveBool = "noHead";
            animator.SetBool("move", false);

            SynchronizeAnimatorAndAgent();
        }
    }
    void Normal()
    {
        nrAttack = 1;
        moveBool = "move";

        animator.SetBool("noHead", false);
        SynchronizeAnimatorAndAgent();
    }
    void Found()
    {
        if (sensors.ActiveTargets == null || sensors.ActiveTargets.Count == 0)
            return;

        foreach (var candidate in sensors.ActiveTargets.Values)
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
    public void SetAgentActive(bool active)
    {

        agent.enabled = active;
        if (active)
        {
            agent.isStopped = false;
        }
        else if (!active)
        {
            agentState = AgentState.Idle;
            isCrawling = true;
            animator.SetBool("crawl", true);
            animator.Play("Base Layer.Crawl");
            animator.SetBool("CrawlAttack", false);
            animator.SetInteger("Attack", 0);
        }
    }



    private void IdleBehaviour()
    {
        eye.NormalEye();
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
        eye.NormalEye();
        if (atDestination)
        {
            Vector3 location = PickLocationInRange(searchRange);
            MoveTo(location);
        }
    }
    private void InvestegateBehavior()
    {
        eye.AngryEye();
        if (atDestination)
        {
            MoveTo(soundLocation);
        }

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
        eye.AngryEye();
        if (currentTarget.transform)
        {
            if (agent.isOnNavMesh)
            {

                if (Vector3.Distance(agent.transform.position, currentTarget.transform.position) > minAttackRange)
                {
                    MoveTo(currentTarget.transform.position);
                }
                if (transform.position.InRangeOf(currentTarget.transform.position, attackRange))
                {
                    agentState = AgentState.Attacking;
                    agent.isStopped = true;
                }
            }

        }
    }

    public void HandleCrawling()
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

        if (isArmles)
        {
            animator.SetBool("crawlNoArm", shouldMove);
            animator.SetBool("crawl", false);
        }
        else
        {
            animator.SetBool("crawl", shouldMove);
            animator.SetBool("crawlNoArm", false);
        }

        animator.SetBool("move", false);
        animator.SetBool("noHead", false);

        animator.SetFloat("vely", agent.velocity.magnitude);
    }
    public void SynchronizeAnimatorAndAgent()
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

        animator.SetBool(moveBool, shouldMove);
        animator.SetBool("crawl", false);
        animator.SetFloat("vely", agent.velocity.magnitude);

        lookAt.lookAtTargetPosition = agent.steeringTarget + transform.forward;
    }

    private void AttackBehaviour()
    {

        eye.AngryEye();

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
            animator.SetBool("CrawlAttack", false);

            agent.SetDestination(currentTarget.transform.position);
            agent.isStopped = false;
            agentState = AgentState.Chasing;
        }

    }

    public void Attack()
    {

        //if (agentStatehit == AgentHit.Armless)
        //{
        //    animator.SetInteger("Attack", 3);
        //    StartCoroutine(AttackCooldown(.5f));
        //}
        //else if (agentStatehit == AgentHit.Crawl)
        //{
        //    //animator.Play("Base Layer.Crawl");
        //    animator.SetBool("CrawlAttack", true);
        //    animator.SetInteger("Attack", 5);
        //    StartCoroutine(AttackCooldown(.4f));
        //}
        //else
        //{
        //    animator.SetInteger("Attack", Random.Range(nrAttack, 4));
        //    StartCoroutine(AttackCooldown(.5f));
        //}
    }
    private IEnumerator CrawlAttackCooldown(float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetBool("CrawlAttack", true);
    }
    private IEnumerator AttackCooldown(float t)
    {
        yield return new WaitForSeconds(t);
        DoDmg(1f);
        animator.SetInteger("Attack", 0);
    }
    private IEnumerator AnimationCooldown(int idx, float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetInteger(idx, 0);
        //knockback = false;
    }
    void DoDmg(float attackTime)
    {
        Vector3 targetDelta = target.position - transform.position;
        attckTimer -= Time.deltaTime;
        if (targetDelta.sqrMagnitude < 3 && attckTimer < 0)
        {
            if (target.TryGetComponent(out IDamageble damageble))
            {
                damageble.TakeDamage(transform.position, targetDelta, 5);
                attckTimer = attackTime;
            }
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
                agent.nextPosition = transform.position;
            }
        }
        else
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.MovePosition(rootPosition);
            agent.nextPosition = rootPosition;
        }
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


