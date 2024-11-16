using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class ZombieBrain : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;

    [SerializeField] float meleeRange = 2;
    [SerializeField] float meleeDamage = 10;
    [SerializeField] LayerMask meleeMask;
    [SerializeField] ZombieSensor sensor;
    bool attacking;

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
          //  if(!attacking)StopCoroutine(DoDamageCoroutine());
            attacking = true;
            float angleDelta = 360 * dt;
            float angle = Vector3.SignedAngle(transform.forward, sensor.TargetDir, Vector3.up);
            float r = Mathf.Clamp(angle, -angleDelta, angleDelta);
            transform.Rotate(new(0, r, 0));

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(delta, Vector3.up), 360 * dt);
        }
        else
        {
            animator.SetBool("AttackBool", false);
           // StopCoroutine(DoDamageCoroutine());
            agent.isStopped = false;
            attacking = false;
            agent.SetDestination(sensor.TargetPos);
        }
    }

    private IEnumerator DoDamageCoroutine()
    {
        while (attacking)
        {
            yield return new WaitForSeconds(.4f);
            DoDamage();
            yield return new WaitForSeconds(.4f);
        }
    }
    private void DoDamage()
    {
        Debug.Log("DoDammage called?");
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + sensor.TargetDir, Color.red, .5f);
        if(Physics.Raycast(transform.position + Vector3.up, sensor.TargetDir, out RaycastHit hit, meleeRange, meleeMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("hit" + hit.transform.gameObject.name);
            if (hit.transform.TryGetComponent(out IDamageble damageble))
            {
                damageble.TakeDamage(hit.point, sensor.TargetDir, meleeDamage);
            }
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
