using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(EnemyAI))]
public class VisionSensor : MonoBehaviour
{
    [SerializeField] LayerMask detectionMask = ~0;

    EnemyAI linkedAI;

    void Start()
    {
        linkedAI = GetComponent<EnemyAI>();
    }

    void Update()
    {

        // Check all 
        for (int i = 0; i < DetectableTargetManager.Instance.AllTargets.Count; i++)
        {
            var candiadateTarget = DetectableTargetManager.Instance.AllTargets[i];
            //skip if ourselves
            if (candiadateTarget.gameObject == gameObject)
            {
                continue;
            }
            var vectorToTarget = candiadateTarget.transform.position + Vector3.up - linkedAI.EyeLocation;

            //if out of range
            if (vectorToTarget.sqrMagnitude > linkedAI.VisionConeRange * linkedAI.VisionConeRange)
            {
                continue;
            }
            vectorToTarget.Normalize();

            //if out of vision of cone
            if (Vector3.Dot(vectorToTarget.normalized, linkedAI.EyeDirection) < linkedAI.CosVisionConeAngle)
            {
                continue;
            }

            // rayCast to target 
            RaycastHit hitResult;
            if (Physics.Raycast(linkedAI.EyeLocation, vectorToTarget.normalized, out hitResult,
                linkedAI.VisionConeRange, detectionMask, QueryTriggerInteraction.Collide))
            {
                if (hitResult.collider.GetComponentInParent<DetectableTarget>() == candiadateTarget)// { }

                {
                    linkedAI.ReportCanSee(candiadateTarget);
                }
            }
        }
    }



}
