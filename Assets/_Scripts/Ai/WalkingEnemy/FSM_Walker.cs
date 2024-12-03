using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;
using static Limb;
using static UnityEditor.FilePathAttribute;

public class FSM_Walker : MonoBehaviour
{
    [Header("Sensors")]
    Sensing sensing;
    [SerializeField] float detectedAwarness = 1.2f;
    private float nearestPointSearchRange = 7f;
    [SerializeField] float gunHearingRange = 45;


    [Header("Nav")]
    [SerializeField] float wanderRange = 25f;
    private float idleTime;
    bool destinationSet = true;
    bool reachedDestination = false;
    public bool atDestination => reachedDestination;
    private NavMeshAgent agent;


    //public Transform target;
    [SerializeField] float dmg = 15f;
    // Attack
    public float attackRange = 2.5f;
    public float minAttackRange = 1.0f;

    // Crawl Attack
    public float attackCrawlRange = 1.0f;
    public float minAttackCrawlRange = .3f;

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;
    private LookAt lookAt;
    bool attack;

    //chase
    private Vector3 lastTargetPosition;

    // States
    public enum AgentState { Idle, Wander, Chasing, Attacking, Investegate, Sleep }
    public AgentState agentState = AgentState.Idle;
    private AgentState previousState = AgentState.Idle;
    private Vector3 previousTargetPosition;



    // Animation
    string moveBool;
    string crawlBool;
    private Animator animator;

    public bool canAttakRun;
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
    bool ragdoll;

    Limbstate limbState;

    public int nrOfAttacks;

    public bool sleep;

    private Vector3 startPosition;
    // sound
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private AudioSource attackAudio;
    [SerializeField] private List<AudioClip> footstepClips;
    [SerializeField] private List<AudioClip> soundClips;
    [SerializeField] private List<AudioClip> attackClips;
    //[SerializeField] private float soundRadius = 10f;

  
    private void OnEnable()
    {
        Firearm.OnShoot += ReactToShoot;
    }

    private void OnDisable()
    {
        Firearm.OnShoot -= ReactToShoot;
    }


    private void Awake()
    {
        limbState = GetComponent<Limbstate>();
        footstepAudio = GetComponent<AudioSource>();
        footstepAudio.volume = 1f;
        characterController = GetComponent<CharacterController>();

    
        sensing = GetComponent<Sensing>();
        //sensors = GetComponent<AwarenessSystem>();
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
        startPosition = transform.position;
        player.NotifyOnPlayerChanged(OnPlayer);
    }


    void Update()
    {
 

        if(sleep)
        {
            agentState = AgentState.Sleep;
        }

        if (agent.isOnNavMesh)
        {
            if (!agent.pathPending && !agent.isOnOffMeshLink && destinationSet && (agent.remainingDistance <= agent.stoppingDistance))
            {
                destinationSet = false;
                reachedDestination = true;
                agentState = AgentState.Idle;
            }
        }

        if (agentState != AgentState.Sleep)
        {

            UpdateState();
            Found();
        }

        if (previousState != agentState)
        {
            //MoveTo(transform.position);
            OnStateEnter(agentState);
            OnStateExit(previousState);
            destinationSet = false;
            reachedDestination = true;
            previousState = agentState;
        }

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
                footstepAudio.Stop();
                break;
            case AgentState.Wander:
                if (sensing.isTrackingPlayer)
                {
                    agentState = AgentState.Chasing;
                    return;
                }
                WanderBehavior();
                break;
            case AgentState.Investegate:
                InvestegateBehavior();
                break;
            case AgentState.Chasing:
                if (!sensing.isTrackingPlayer && !sensing.CanHearTarget())
                {
                    agentState = AgentState.Wander; 
                    return;
                }
                ChaseBehaviour();
                break;
            case AgentState.Attacking:
                AttackBehaviour();
                break;
        }
    }

    void OnStateEnter(AgentState newState)
    {
       
        if (newState == AgentState.Sleep)
        {
            animator.SetBool("move", false);
            animator.SetBool("crawl", false);
            agent.isStopped = true;
            agent.enabled = false;
        }
        if (newState == AgentState.Chasing)
        {
            attackAudio.clip = soundClips[1];

            if (!attackAudio.isPlaying)
            {
                attackAudio.Play();
            }
           

        }
    }
    void OnStateExit(AgentState oldState)
    {
        if (oldState == AgentState.Sleep)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }
        if(oldState == AgentState.Wander || oldState == AgentState.Idle)
        {
            if(limbState.standing && limbState.limbStatehit == Limbstate.AgentHit.Normal)
            {
                PlayDetectPlayerAnimation();

            }
        }
    }


  
    void Found()
    {
        if(agentState != AgentState.Attacking && (sensing.isTrackingPlayer || sensing.CanHearTarget()))
        {
            agentState = AgentState.Chasing;
        }       

       
    }

    public void ReactToShoot(Vector3 playerPosition)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        if (distanceToPlayer <= gunHearingRange)
        {
            soundLocation = playerPosition;
            agentState = AgentState.Investegate;

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
        if (!destinationSet)
        {
            Vector3 location = PickLocationInRange(wanderRange);
            MoveTo(location);
            destinationSet = true;
            reachedDestination = false;
        }

        if (atDestination)
        {
            destinationSet = false;
            idleTime = Random.Range(5f, 10f); 
            StartCoroutine(IdleTimer(idleTime));
        }
    }

    private void InvestegateBehavior()
    {
      
        if (atDestination)
        {
           MoveTo(soundLocation);
        }

    }
    public Vector3 PickLocationInRange(float range)
    {
        //Vector3 searchLocation = transform.position;
        Vector3 searchLocation = startPosition;
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

    //private void ChaseBehaviour()
    //{


    //    if (target.transform == null)
    //    {
    //        Debug.Log("null");
    //        agentState = AgentState.Idle;
    //        return;
    //    }

    //    if (agent.isOnNavMesh)
    //    {
    //        float distanceToTarget = Vector3.Distance(agent.transform.position, target.transform.position);

    //        if (distanceToTarget > minAttackRange)
    //        {
    //            MoveTo(target.transform.position);


    //        }


    //        //if (distanceToTarget <= 2.9f && canAttakRun)
    //        //{
    //        //    //StartCoroutine(PlayAnimation("Zombie Attack", 6));
    ////animator.SetLayerWeight(6, 1);
    ////    animator.SetBool("Charge", true);

    //        //    //StartCoroutine(AttackCooldown(.4f));

    //        //}
    //        //else if (transform.position.InRangeOf(target.transform.position, attackRange))
    //        //{
    //        //    animator.SetLayerWeight(6, 0);
    //        //    animator.SetBool("Charge", false);
    //        //}
    //        //else
    //        //{
    //        //    animator.SetLayerWeight(6, 0);
    //        //    animator.SetBool("Charge", false);
    //        //}


    //        if (transform.position.InRangeOf(target.transform.position, attackRange))
    //        {
    //            agentState = AgentState.Attacking;
    //            agent.isStopped = true;
    //        }
    //    }


    //}

    private void ChaseBehaviour()
    {
        if (target == null)
        {
            agentState = AgentState.Wander;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange)
        {
            if (!limbState.standing) 
            {
                animator.SetLayerWeight(6, 0);
                Attack();
            }
            else if (agent.velocity.magnitude < 0.1f)
            {
                Attack();
            }
            else if (canAttakRun) 
            {
                PerformChargeAttack();
            }
            else
            {
                animator.SetLayerWeight(6, 0);
                animator.SetBool("Charge", false);
            }
        }
        else
        {
            animator.SetLayerWeight(6, 0);
            animator.SetBool("Charge", false);
        }

        if (agent.isOnNavMesh)
        {
            MoveTo(target.position);
        }
    }



    private void PerformChargeAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(6);

        if (stateInfo.IsName("Charge") && stateInfo.normalizedTime < 1f)
        {
            return;
        }

        animator.SetLayerWeight(6, 1);
        animator.SetBool("Charge", true);

        agent.isStopped = false;
    }

    //private void PerformCrawlAttack()
    //{
    //    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

    //    if (stateInfo.IsName("CrawlA") && stateInfo.normalizedTime < 1f)
    //    {
    //        return;
    //    }

    //    if(attack)
    //    {
    //        ApplyDamage();
    //        attack = false;
       
    //    }


    //    animator.SetTrigger("CrawlA");
    //    agent.isStopped = true; 
    //}
  
   

    //private void ChaseBehaviour()
    //{
    //    if (currentTarget.transform)
    //    {
    //        if (agent.isOnNavMesh)
    //        {

    //            if (Vector3.Distance(agent.transform.position, currentTarget.transform.position) > minAttackRange)
    //            {
    //                MoveTo(currentTarget.transform.position);
    //            }
    //            if (transform.position.InRangeOf(currentTarget.transform.position, attackRange))
    //            {
    //                agentState = AgentState.Attacking;
    //                agent.isStopped = true;
    //            }
    //        }


    //    }

    //}

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

        footstepAudio.clip = footstepClips[1];
        if (shouldMove  && !footstepAudio.isPlaying)
        {
            footstepAudio.Play();
        }
        else if (!shouldMove && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
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

        animator.SetBool("move", shouldMove);
        animator.SetBool("crawl", false);
        animator.SetFloat("vely", agent.velocity.magnitude);

        lookAt.lookAtTargetPosition = agent.steeringTarget + transform.forward;

        footstepAudio.clip = footstepClips[0];
        if (shouldMove  && !footstepAudio.isPlaying)
        {
            footstepAudio.Play();
        }
        else if (!shouldMove  && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
    }

    private void PlayDetectPlayerAnimation()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("DetectPlayer") && stateInfo.normalizedTime < 1f)
        {
            return;
        }

        animator.SetTrigger("DetectPlayer");
    }
    private void AttackBehaviour()
    {
        if (target == null)
        {
            agentState = AgentState.Wander;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            agentState = AgentState.Chasing;
            agent.isStopped = false;
            return;
        }

        if (!limbState.standing)
        {
            agentState = AgentState.Idle;
            return;
        }

        if (CanAttack())
        {
            animator.SetLayerWeight(6, 0); 
            animator.SetTrigger("Attack");
            Attack();
        }
    }

    private bool CanAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return !stateInfo.IsName("Attack") || stateInfo.normalizedTime >= 1f;
    }



    //private void AttackBehaviour()
    //{
    //    if (!target.transform)
    //    {
    //        //swap state?
    //        agentState = AgentState.Idle;
    //        return;
    //    }
    //    if(!limbState.standing)
    //    {

    //    }
    //    else if (target.transform.position.InRangeOf(transform.position, attackRange))
    //    {
    //        Attack();
    //        Vector3 dir = target.transform.position - transform.position;
    //        transform.forward = Vector3.MoveTowards(transform.forward, dir, agent.angularSpeed * Time.deltaTime).WithY();
    //        if (target.transform.position.InRangeOf(transform.position, minAttackRange))
    //        {
    //            //we are too close = pushback self!
    //            transform.position -= transform.forward * Time.deltaTime;
    //        }
    //    }       
    //    {
    //        //animator.SetBool("CrawlAttack", false);

    //        //agent.SetDestination(currentTarget.transform.position);
    //        //agent.isStopped = false;
    //        //agentState = AgentState.Chasing;
    //    }
    //}

    public void Attack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

       
        if (stateInfo.IsName("Run") || stateInfo.IsName("Move"))
        {
            return; 
        }

        animator.SetInteger("Attack", Random.Range(nrOfAttacks, 4));
        StartCoroutine(AttackCooldown(.5f));

    }
    private IEnumerator AttackCooldown(float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetInteger("Attack", 0);
    }


    private IEnumerator CrawlAttackCooldown(float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetBool("CrawlAttack", true);
    }
   

    private IEnumerator AnimationCooldown(int idx, float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetInteger(idx, 0);
    }

    public void ApplyDamage()
    {
        if (target == null || target.transform == null)
            return;


        Vector3 targetDelta = target.transform.position - transform.position;

        if (targetDelta.sqrMagnitude < 2 * 2)
        {
            attackAudio.clip = attackClips[0];

            if (!attackAudio.isPlaying)
            {
                attackAudio.Play();
            }

            if (target.TryGetComponent(out IDamageble damageable))
            {
                damageable.TakeDamage(transform.position, targetDelta, dmg); 
            }
        }
    }
    public void PauseAgent()
    {
        agent.isStopped = true;
    }

    public void ResumeAgent()
    {
        agent.isStopped = false;
     
    }

    private void OnAnimatorMove()
    {
        //if (Time.timeScale == 0) return;

        Vector3 rootPosition = animator.rootPosition;

        rootPosition.y = agent.nextPosition.y;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc)
        {
            bool grouned = cc.SimpleMove((rootPosition - transform.position) / Time.unscaledDeltaTime);

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


