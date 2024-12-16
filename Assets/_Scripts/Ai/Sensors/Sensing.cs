
using System.Collections.Generic;
using UnityEngine;

public class Sensing : MonoBehaviour
{
    [Header("Vision Settings")]
    public float visionRange = 10f;
    public float visionAngle = 45f;
    public LayerMask visionMask;

    [Header("Hearing Settings")]
    public float hearingRange = 5f;
    public float gunHearingRange = 45f;
    public LayerMask hearingMask;

    [Header("Target")]
    [SerializeField] Transform target;
    public PlayerDataSO player;

    [Header("Memory Settings")]
    public float memoryDuration = 2f;
    private float memoryTimer = 0f;
    public bool isTrackingPlayer = false;
    EnemyHealth enemyHealth;

  
    Animator animator;
    [SerializeField] private AudioSource attackAudio;
    [SerializeField] private AudioClip screem;
    void Start()
    {
        player.NotifyOnPlayerChanged(OnPlayer);
        animator = GetComponent<Animator>();
    }

    private void OnPlayer(Transform obj)
    {
        target = obj;
    }

    private void Update()
    {
        if (target == null) return;

        if (CanSeeTarget())
        {
            memoryTimer = memoryDuration;

            if (!isTrackingPlayer)
            {
                PlayDetectionAnimation();
            }

            isTrackingPlayer = true;
        }
        else if (isTrackingPlayer)
        {
            memoryTimer -= Time.deltaTime;
            if (memoryTimer <= 0f)
            {
                isTrackingPlayer = false;
            }
        }
    }
    private void PlayDetectionAnimation()
    {
        attackAudio.clip = screem;
        attackAudio.Play();

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("DetectPlayer") && stateInfo.normalizedTime < 1f)
            {
                return;
            }

            animator.SetTrigger("DetectPlayer");
          
        }
    }
    public bool CanSeeTarget()
    {
        if (target == null) return false;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < visionRange)
        {
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            if (angleToTarget < visionAngle / 2f)
            {
                Ray ray = new Ray(transform.position, directionToTarget);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, visionRange, visionMask))
                {
                
                    if (hit.transform == target)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }


    public bool CanHearTarget()
    {
        if (target == null) return false;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < hearingRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget, out hit, hearingRange, hearingMask))
            {
                if (hit.transform == target)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward * visionRange;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
}
