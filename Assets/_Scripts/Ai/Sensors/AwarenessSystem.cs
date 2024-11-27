using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HearingManager;
using static Unity.VisualScripting.Member;


public class Trackedtarget
{
    public bool bosted = false;

    [SerializeField] bool GOAP;

    public DetectableTarget detectable;
    public Vector3 rawPosition;

  

    public float lastSenedTime = -1f;
    public float Awarness; // 0 = not aware ;
                           // 0-1 = rough idea (no set location)
                           // ; 1-2 likely target (location)
                           // 2 == fully detcted

    public bool UpdatingAwarness(DetectableTarget target, Vector3 position,
        float awarness, float minAwareness)
    {
        var oldAwaeness = Awarness;
        if (target != null)
        {
            detectable = target;
        }

        rawPosition = position;
        lastSenedTime = Time.time;
        Awarness = Mathf.Clamp(Mathf.Max(Awarness, minAwareness) + awarness, 0f, 2f);

        if (oldAwaeness < 2f && Awarness >= 2f)
        {
            return true;
        }
        if (oldAwaeness < 1f && Awarness >= 1f)
        {
            return true;
        }

        return false;
    }
    public bool DecayAwareness(float decayTime, float amount)
    {

        // dectected too recently - no change
        if ((Time.time - lastSenedTime) < decayTime)
        {
            return false;
        }
        var oldAwaeness = Awarness;

        Awarness -= amount;

        if (oldAwaeness >= 2f && Awarness < 2f)
        {
            return true;
        }
        if (oldAwaeness >= 1f && Awarness < 1f)
        {
            return true;
        }
        if (oldAwaeness <= 0f && Awarness >= 0f)
        {
            return true;
        }

        return Awarness <= 0f;
    }
}



//[RequireComponent(typeof(EnemyAI))]
public class AwarenessSystem : MonoBehaviour
{
    [SerializeField] AnimationCurve visonSensitivity;
    public float visionMinimumAwareness = 1f;
    public float visionAwarenessBuildRate = 10f;

    [SerializeField] float hearingMinimumAwareness = 0f;
    [SerializeField] float hearingAwarnessBuildRate = 0.5f;

    [SerializeField] float proximityMinimumAwareness = 0f;
    [SerializeField] float proximityAwarenessBuildRate = 1f;

    [SerializeField] float awarenessDecayDelay = 0.1f;
    [SerializeField] float awarenessDecaRate = 0.1f;

    public Vector3 soundLocation;

    Dictionary<GameObject, Trackedtarget> targets = new Dictionary<GameObject, Trackedtarget>();
    EnemyAI linkedAI;
    Vector3 targ;
    public Dictionary<GameObject, Trackedtarget> ActiveTargets => targets;

    FSM fsm;
    FSM_Walker fsmWalker;
    FSMClimber fsmClimber;
    //CharacterAgent characterAgent;
    void Start()
    {
        //characterAgent = GetComponent<CharacterAgent>();
        fsmWalker = GetComponent<FSM_Walker>();
        fsmClimber = GetComponent<FSMClimber>();
        fsm = GetComponent<FSM>();
        linkedAI = GetComponent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        List<GameObject> toCleanup = new List<GameObject>();
        foreach (var targetGO in targets.Keys)
        {
            if (targets[targetGO].DecayAwareness(awarenessDecayDelay, awarenessDecaRate * Time.deltaTime))
            {
                if (targets[targetGO].Awarness <= 0f)
                {
                    linkedAI.OnFullyLost();
                    toCleanup.Add(targetGO);
                }
                else
                {
                    if (targets[targetGO].Awarness >= 1f)
                    {
                        linkedAI.OnLostDetect(targetGO);
                    }
                    else
                    {
                        linkedAI.OnLostSuspicion();
                    }
                    //Debug.Log("Threshold change for" + targetGO.name + " " + targets[targetGO].Awarness);
                }

            }
        }

        // cleanup target that are no longer detected

        foreach (var target in toCleanup)
        {
            targets.Remove(target);
        }
    }
    private void UpdateAwarness(GameObject targetGO, DetectableTarget target, Vector3 position, float awareness, float minAwareness)
    {
        if (!targets.ContainsKey(targetGO))
        {
            targets[targetGO] = new Trackedtarget();
        }

        if (targets[targetGO].UpdatingAwarness(target, position, awareness, minAwareness))
        {
            if (targets[targetGO].Awarness >= 2f)
            {
                linkedAI.OnFullyDetected(targetGO);
                targ = position;
            }
            else if (targets[targetGO].Awarness >= 1f)
            {
                linkedAI.OnDetected(targetGO);
                targ = position;
            }
            else
            {
                targ = Vector3.zero;
                linkedAI.OnSuspicious();
            }
            // Debug.Log("Threshold change for" + targetGO.name + " " + targets[targetGO].Awarness);
        }
    }
    public Vector3 GetSoundPos()
    {
        return targ;
    }
    public void ReportCanSee(DetectableTarget seen)
    {

        // determine where the target is in the field of view
        var vectorToTarget = (seen.transform.position - linkedAI.EyeLocation).normalized;
        var dotProduct = Vector3.Dot(vectorToTarget, linkedAI.EyeDirection);

        // determine the awareness contribution
        var awareness = visonSensitivity.Evaluate(dotProduct) * visionAwarenessBuildRate * Time.deltaTime;

        UpdateAwarness(seen.gameObject, seen, seen.transform.position, awareness, visionMinimumAwareness);
    }


    public void ReportCanHear(GameObject source, Vector3 location, EHeardSoundCategory category, float intensity)
    {
        var awareness = intensity * hearingAwarnessBuildRate * Time.deltaTime;
        UpdateAwarness(source, null, location, awareness, hearingMinimumAwareness);
        if(category == EHeardSoundCategory.EGunshot)
        {
            soundLocation = location;
            if(fsm)
            {
                fsm.HeardSomthing(location);
               
            }
            else if (fsmClimber)
            {
                fsmClimber.HeardSomthing(location);
            }
            else if (fsmWalker)
            {
          
            }

            UpdateAwarness(source, null, location, awareness, 1);
        }
        if(intensity > 0.7f || awareness > 0.4f)
        {
            soundLocation = location;
            if (fsm)
            {
                fsm.HeardSomthing(location);
               
            }
            else if(fsmClimber)
            {
                fsmClimber.HeardSomthing(location);
            }
            else if (fsmWalker)
            {
      
            }
        }
       
       

    }

    public void ReportInProximity(DetectableTarget target)
    {
        var awareness = proximityAwarenessBuildRate * Time.deltaTime;
        UpdateAwarness(target.gameObject, target, target.transform.position, awareness, proximityMinimumAwareness);
    }
}
