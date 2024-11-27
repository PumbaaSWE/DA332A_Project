using System.Collections;
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


    void Start()
    {
        player.NotifyOnPlayerChanged(OnPlayer);
    }

 
    private void OnPlayer(Transform obj)
    {
        target = obj;
        if (target)
        {
            // Hantering om m�let s�tts
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
                if (!Physics.Linecast(transform.position, target.position, visionMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CanHearTarget()
    {
        if (target == null) return false;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < hearingRange)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        // Rita synf�ltet
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward * visionRange;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Rita h�rselomr�det
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
}
