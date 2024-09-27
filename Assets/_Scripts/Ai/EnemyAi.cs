using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static HearingManager;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[RequireComponent(typeof(AwarenessSystem))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI FeedbackDisplay;

    public float _VisionConeAngle = 60f;
    public float _VisionConeRange = 30f;
    [SerializeField] Color _VisionConeColour = new Color(1f, 0f, 0f, 0.25f);

    [SerializeField] float _HearingRange = 20f;
    [SerializeField] Color _HearingRangeColour = new Color(1f, 1f, 0f, 0.25f);

    [SerializeField] float _ProximityDetectionRange = 3f;
    [SerializeField] Color _ProximityRangeColour = new Color(1f, 1f, 1f, 0.25f);

    [SerializeField] Transform head;
    public Vector3 EyeLocation => head.position;
    public Vector3 EyeDirection => transform.forward;

    public float VisionConeAngle => _VisionConeAngle;
    public float VisionConeRange => _VisionConeRange;
    public Color VisionConeColour => _VisionConeColour;

    public float HearingRange => _HearingRange;
    public Color HearingRangeColour => _HearingRangeColour;

    public float ProximityDetectionRange => _ProximityDetectionRange;
    public Color ProximityDetectionColour => _ProximityRangeColour;

    public float CosVisionConeAngle { get; private set; } = 0f;

    AwarenessSystem Awareness;

    void Awake()
    {
        CosVisionConeAngle = Mathf.Cos(VisionConeAngle * Mathf.Deg2Rad);
        Awareness = GetComponent<AwarenessSystem>();
    }



    public void ReportCanSee(DetectableTarget seen)
    {
        Awareness.ReportCanSee(seen);

        Debug.Log("can see");
    }

    public void ReportCanHear(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        Awareness.ReportCanHear(source, location, category, intensity);
        //Debug.Log("can hear");

    }

    public void ReportInProximity(DetectableTarget target)
    {
        Awareness.ReportInProximity(target);
        //Debug.Log("can sense");
    }

    public void OnSuspicious()
    {
        Debug.Log("I hear you");
        //FeedbackDisplay.text = "I hear you";
    }

    public void OnDetected(GameObject target)
    {
        Debug.Log("I see you ");
        //FeedbackDisplay.text = "I see you " + target.gameObject.name;
    }

    public void OnFullyDetected(GameObject target)
    {
        Debug.Log("Charge! ");
        //FeedbackDisplay.text = "Charge! " + target.gameObject.name;
    }

    public void OnLostDetect(GameObject target)
    {
        Debug.Log("Where are you ");
        //FeedbackDisplay.text = "Where are you " + target.gameObject.name;
    }

    public void OnLostSuspicion()
    {
        Debug.Log("Where did you go");
        //FeedbackDisplay.text = "Where did you go";
    }

    public void OnFullyLost()
    {
        Debug.Log("Must be nothing");
        //FeedbackDisplay.text = "Must be nothing";
    }

   
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyAI))]
public class EnemyAIEditor : Editor
{
    public void OnSceneGUI()
    {
        var ai = target as EnemyAI;

        // draw the detectopm range
        Handles.color = ai.ProximityDetectionColour;
        Handles.DrawSolidDisc(ai.transform.position, Vector3.up, ai.ProximityDetectionRange);

        // draw the hearing range
        Handles.color = ai.HearingRangeColour;
        Handles.DrawSolidDisc(ai.transform.position, Vector3.up, ai.HearingRange);

        //// work out the start point of the vision cone
        //Vector3 startPoint = Mathf.Cos(-ai.VisionConeAngle * Mathf.Deg2Rad) * ai.transform.forward +
        //                     Mathf.Sin(-ai.VisionConeAngle * Mathf.Deg2Rad) * ai.transform.right;

        //// draw the vision cone
        //Handles.color = ai.VisionConeColour;
        //Handles.DrawSolidArc(ai.transform.position, Vector3.up, startPoint, ai.VisionConeAngle * 2f, ai.VisionConeRange);


 // 3d
        Vector3 startPoint = Quaternion.Euler(0, -ai.VisionConeAngle, 0) * ai.transform.forward;

  
        Handles.color = ai.VisionConeColour;


        Handles.DrawSolidArc(ai.transform.position, Vector3.up, startPoint, ai.VisionConeAngle * 2f, ai.VisionConeRange);
    }
}
#endif // UNITY_EDITOR