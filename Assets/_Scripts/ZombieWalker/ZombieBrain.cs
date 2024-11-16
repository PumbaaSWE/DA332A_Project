using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class ZombieBrain : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;

    [SerializeField] float meleeRange = 2;
    [SerializeField] ZombieSensor sensor;

    // Start is called before the first frame update
    void Start()
    {
        sensor.OnHit += Sensor_OnHit;
        GetComponent<Health>().OnDeath += OnDeath;  
    }

    private void OnDeath(Health health)
    {
        GetComponent<RagdollController>().EnableRagdoll();
        //Invoke(nameof(Sink), 5);
    }

    //private void Sink()
    //{
    //    GetComponent<RagdollController>().FreezeRagdoll();
    //    StartCoroutine(SinkCoroutine());
    //}

    //private IEnumerator SinkCoroutine()
    //{

    //    float t = 2;
    //    while (t > 0)
    //    {
    //        float dt = Time.deltaTime;
    //        t -= dt;
    //        transform.position += Vector3.down * 0.5f * dt;
    //        yield return null;
    //    }
    //    Destroy(gameObject);
    //}

    private void Sensor_OnHit(Transform player, Vector3 point, Vector3 dir, float damage)
    {
        animator.SetTrigger("HitForward");
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
        UpdateBrain(dt);
    }

    private void UpdateBrain(float dt)
    {

        agent.stoppingDistance = sensor.SeeTarget ? meleeRange : 0;


        if (sensor.TargetDist < meleeRange && sensor.SeeTarget)
        {
            animator.SetBool("AttackBool", true);
            agent.isStopped = true;

            float angleDelta = 360 * dt;
            float angle = Vector3.SignedAngle(transform.forward, sensor.TargetDir, Vector3.up);
            float r = Mathf.Clamp(angle, -angleDelta, angleDelta);
            transform.Rotate(new(0, r, 0));

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(delta, Vector3.up), 360 * dt);
        }
        else
        {
            animator.SetBool("AttackBool", false);
            agent.isStopped = false;
            agent.SetDestination(sensor.TargetPos);
        }
    }

    //private void OldUpdate(float dt)
    //{
    //    if (target)
    //    {
    //        Vector3 targetPos = target.position.WithY();
    //        Vector3 delta = targetPos - transform.position;

    //        if (delta.sqrMagnitude < meleeRange * meleeRange)
    //        {
    //            animator.SetBool("AttackBool", true);
    //            agent.isStopped = true;
    //            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(delta, Vector3.up), 360 * dt);
    //        }
    //        else
    //        {
    //            animator.SetBool("AttackBool", false);
    //            agent.isStopped = false;
    //            agent.SetDestination(targetPos);
    //        }
    //    }
    //}
}
