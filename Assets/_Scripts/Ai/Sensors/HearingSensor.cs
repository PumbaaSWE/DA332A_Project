using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HearingManager;

[RequireComponent(typeof(EnemyAI))]
public class HearingSensor : MonoBehaviour
{
    EnemyAI linkedAI;
    void Start()
    {
        linkedAI = GetComponent<EnemyAI>();
        HearingManager.Instance.Register(this);
    }



    private void OnDestroy()
    {
        if (HearingManager.Instance != null)
        {
            HearingManager.Instance.Deregister(this);
        }
    }

    public void OnHeardSound(GameObject sourse, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        if (Vector3.Distance(location, linkedAI.EyeLocation) > linkedAI.HearingRange)
        {
            return;
        }
        linkedAI.ReportCanHear(sourse, location, category, intensity);
    }
}
