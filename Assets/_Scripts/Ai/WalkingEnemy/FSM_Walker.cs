using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FSM_Walker : MonoBehaviour
{
    [Header("Sensors")]
    Sensing sensing;
    private float nearestPointSearchRange = 5f;
    [SerializeField] float gunHearingRange = 55;


    [Header("Nav")]
    [SerializeField] float wanderRange = 25f;
    private float idleTime;
    bool destinationSet = true;
    bool reachedDestination = false;
    public bool atDestination => reachedDestination;
    private NavMeshAgent agent;
    private Vector3 startPosition;
    //public Transform target;   


    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;
    private LookAt lookAt;
    bool attack;

    //chase
    [SerializeField] Transform target;
    public PlayerDataSO player;
    private Vector3 lastTargetPosition;
    Vector3 soundLocation;

    // States
    public enum AgentState { Idle, Wander, Chasing, Attacking, Investegate, Sleep }
    public AgentState agentState = AgentState.Idle;
    private AgentState previousState = AgentState.Idle;
    private Vector3 previousTargetPosition;
    public bool sleep;
    public bool isCrawling;
    public bool isArmles;


    // Animation
    string moveBool;
    string crawlBool;
    private Animator animator;
    bool wasGrounded;

    //Attacks
    public bool canAttakRun;
    int nrAttack;
    float attckTimer;
    public int nrOfAttacks;
    // Attack
    public float attackRange = 2.5f;
    public float minAttackRange = 1.0f;
    // Crawl Attack
    public float attackCrawlRange = 1.0f;
    public float minAttackCrawlRange = .3f;
    [SerializeField] float dmg = 15f;

    CharacterController characterController;
    Limbstate limbState;
    EnemyHealth enemyHealth;



    // sound
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private AudioSource attackAudio;
    [SerializeField] private List<AudioClip> footstepClips;
    [SerializeField] private List<AudioClip> soundClips;
    [SerializeField] private List<AudioClip> attackClips;

    private float nextPlayTime = 5f;
    private void OnEnable()
    {
        enemyHealth.OnHealthChanged += ReactToShoot;
    }

    private void OnDisable()
    {
        enemyHealth.OnHealthChanged += ReactToShoot;
    } 
    public void ReactToShoot(float health)
    {
        MoveTo(player.PlayerTransform.position);
    }
    private void Awake()
    {
        limbState = GetComponent<Limbstate>();
        footstepAudio = GetComponent<AudioSource>();
        footstepAudio.volume = 1f;
        characterController = GetComponent<CharacterController>();

        enemyHealth= GetComponent<EnemyHealth>();   
        sensing = GetComponent<Sensing>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        lookAt = GetComponent<LookAt>();
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
      
    }
    void OnStateExit(AgentState oldState)
    {
        if (oldState == AgentState.Sleep)
        {
            agent.enabled = true;
            agent.isStopped = false;
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

    private void TryPlaySound()
    {
        if (soundClips.Count > 0)
        {
            int randomIndex = Random.Range(2, soundClips.Count);
            attackAudio.clip = soundClips[randomIndex];

            attackAudio.Play();
        }

        nextPlayTime = Time.time + Random.Range(5, 10);
    }
    private void ChaseBehaviour()
    {
        if (target == null)
        {
            agentState = AgentState.Wander;
            return;
        }
        if (Time.time >= nextPlayTime)
        {
            TryPlaySound();
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


