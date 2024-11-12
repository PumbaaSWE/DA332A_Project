using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour
{
    EnemyAI linkedAI;


    void Start()
    {
        linkedAI = GetComponent<EnemyAI>();
    }
    void Update()
    {
        for (int i = 0; i < DetectableTargetManager.Instance.AllTargets.Count; i++)
        {
            var candidateTarget = DetectableTargetManager.Instance.AllTargets[i];

            if (candidateTarget.gameObject == gameObject)
            {
                continue;
            }

            if (Vector3.Distance(linkedAI.EyeLocation + Vector3.up, candidateTarget.gameObject.transform.position)
                <= linkedAI.ProximityDetectionRange)
            {
                linkedAI.ReportInProximity(candidateTarget);
            }
        }
    }
}
