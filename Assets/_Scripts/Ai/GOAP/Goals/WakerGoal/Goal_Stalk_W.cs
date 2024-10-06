using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Stalk_W : Goal_Base
{
    public int prio = 50;
    [SerializeField] float MinAwarenessTostalk = 1.5f;
    [SerializeField] float AwarenessToStopChase = 1f;

    DetectableTarget CurrentTarget;
    float CurrentPriority = 0;



    public Vector3 MoveTarget => CurrentTarget != null ? CurrentTarget.transform.position : transform.position;
    //public Vector3 FleeFromTarget => CurrentTarget != null ? CurrentTarget.transform.position : transform.position;


    public override void OnTickGoal()
    {
        CurrentPriority = 0;

        // no targets
        if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0)
            return;

        if (CurrentTarget != null)
        {
            // check if the current is still sensed
            foreach (var candidate in Sensors.ActiveTargets.Values)
            {
                if (candidate.detectable == CurrentTarget)
                {
                    CurrentPriority = candidate.Awarness < AwarenessToStopChase ? 0 : prio;
                    return;
                }
            }

            // clear our current target
            CurrentTarget = null;
        }

        // acquire a new target if possible
        foreach (var candidate in Sensors.ActiveTargets.Values)
        {
            // found a target to acquire
            if (candidate.Awarness >= MinAwarenessTostalk)
            {
                CurrentTarget = candidate.detectable;
                CurrentPriority = prio;
                return;
            }
        }
    }

    public override void OnGoalDeactivated()
    {
        base.OnGoalDeactivated();

        CurrentTarget = null;
    }

    public override int CalculatePriority()
    {
        return Mathf.FloorToInt(CurrentPriority);
    }

    public override bool CanRun()
    {
        // no targets
        if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0 || CurrentTarget == null)
            return false;



        return true;
    }
}
