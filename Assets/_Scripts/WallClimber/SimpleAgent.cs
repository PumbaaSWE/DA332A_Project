using UnityEngine;

public class SimpleAgent : MonoBehaviour
{

    MoveTowardsController controller;
    LookAt lookAt;
    float timer;
    public Transform eye;
    public PlayerDataSO player;
    [SerializeField] Transform target;
    // SphereCollider sphereCollider;
    enum AgentState { Chasing, Looking, Wander, Avoiding };
    [SerializeField]AgentState mindlessState = AgentState.Wander;

    Vector3 avoid = Vector3.zero;
    // List<Transform> avoids = new();
    Vector3 targetDelta;
    bool canSeeTarget;
    Vector3 lastSeenPos;
    float lastSeenTime;
    float attckTimer;

    [SerializeField] LayerMask seeLayer = -1;
    [SerializeField] float attackDamage = 10;
    [SerializeField] float attackRange = 1.4f;
    [SerializeField] float attackSpeed = 2;
    [SerializeField] float maxDetectionRange = 10;
    [SerializeField] float forgetTime = 10;
    [SerializeField] float avoidRadius = 5;
    [SerializeField] LayerMask avoidLayer = -1;

    Health health;
    RagdollController ragdollController;

    void Start()
    {
        controller = GetComponent<MoveTowardsController>();
        lookAt = GetComponentInChildren<LookAt>();
        player.NotifyOnPlayerChanged(OnPlayer);
        health = GetComponent<Health>();
        ragdollController = GetComponent<RagdollController>();
        health.OnDeath += (x) => { ragdollController.EnableRagdoll(); Destroy(gameObject, 10); };
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
            //target = target.GetChild(0);
        }
    }

    private bool CanSeeTarget(Vector3 targetDelta)
    {
        if (targetDelta.sqrMagnitude > maxDetectionRange * maxDetectionRange) return false;

        Vector3 lookDir = (target.position+Vector3.up) - eye.position;
        if (lookDir.sqrMagnitude < attackRange * attackRange) return true;

        Debug.DrawLine(eye.position, eye.position+lookDir, Color.cyan);

        if (Physics.Raycast(eye.position, lookDir, out RaycastHit hit, maxDetectionRange, seeLayer, QueryTriggerInteraction.Ignore))
        {

            if (hit.collider.transform == target)
            {
                Debug.DrawLine(eye.position, hit.point, Color.magenta);
                return true;
            }
            Debug.DrawLine(eye.position, hit.point, Color.red);
        }
        return false;
    }


    static readonly Collider[] others = new Collider[20];
    private void CheckAvoid()
    {
        avoid = Vector3.zero;
        int n = Physics.OverlapSphereNonAlloc(transform.position, avoidRadius, others, avoidLayer, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < n; i++)
        {
            Transform t = others[i].transform;
            avoid -= t.position - transform.position;
        }
        mindlessState = n > 0 ? AgentState.Avoiding : AgentState.Wander;
    }

    private void HandleWander(float dt)
    {
        timer -= dt;
        if (timer < 0)
        {
            timer = 10;
            Vector3 pos = transform.position + Random.onUnitSphere * 10;
            controller.SetTarget(pos);
            lookAt.lookAtTargetPosition = pos;
        }
    }

    private void HandleChase(float dt)
    {
        if (!canSeeTarget)
        {
            controller.SetTarget(lastSeenPos);
            mindlessState = AgentState.Looking;
            return;
        }


        controller.SetTarget(target.position - targetDelta.normalized * (attackRange *.9f));

        attckTimer -= dt;
        if (targetDelta.sqrMagnitude < attackRange * attackRange && attckTimer < 0)
        {
            //Debug.Log("Attack!!");
            if (target.TryGetComponent(out IDamageble damageble))
            {
                damageble.TakeDamage(transform.position, targetDelta, attackDamage);
                attckTimer = attackSpeed;
                //Debug.Log("Do damage!!");
            }
        }
    }

    private void HandleLooking()
    {
        Debug.Log("HandleLooking");
        Debug.Log(Time.time - lastSeenTime + " " + Time.time + " " + lastSeenTime);
        
        if (Time.time - lastSeenTime > forgetTime)
        {
            mindlessState = AgentState.Wander;
            timer = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        CheckAvoid(); //dont do this every frame?

        if (target)
        {
            targetDelta = target.position - transform.position;
            canSeeTarget = CanSeeTarget(targetDelta);
            if (canSeeTarget)
            {
                lastSeenPos = target.position;
                lastSeenTime = Time.time;
                lookAt.lookAtTargetPosition = lastSeenPos;
                if (mindlessState != AgentState.Avoiding) mindlessState = AgentState.Chasing;
                //Debug.Log("See target");
            }
            //CheckAvoid();

            switch (mindlessState)
            {
                case AgentState.Chasing:
                    HandleChase(dt);
                    break;
                case AgentState.Looking:
                    HandleLooking();
                    break;
                case AgentState.Wander:
                    HandleWander(dt);
                    break;
                case AgentState.Avoiding:
                    HandleAvoid();
                    break;
                default:
                    break;
            }

        }
        else
        {
            HandleWander(dt);
        }


    }

    private void HandleAvoid()
    {
        controller.SetTarget(avoid);
    }
}
