using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Investagate : Goal_Base
{
    [SerializeField] int priority = 45;
    [SerializeField] float MinAwarenessToChase = 1.4f;
    //[SerializeField] float AwarenessToStopChase = 1f;

    DetectableTarget CurrentTarget;
    int CurrentPriority = 0;

    public override void OnTickGoal()
    {
        // add agent at target 
       
        CurrentPriority = priority;
    }

    public override void OnGoalDeactivated()
    {
        base.OnGoalDeactivated();

        CurrentTarget = null;
    }

    public override int CalculatePriority()
    {
        return CurrentPriority;
    }

    public override bool CanRun()
    {
        // no targets
        if (Sensors.soundLocation == null || Sensors.ActiveTargets.Count == 0)
            return false;

        // check if we have anything we are aware of
        foreach (var candidate in Sensors.ActiveTargets.Values)
        {
            if (candidate.Awarness >= MinAwarenessToChase)
                return true;
        }

        return false;
    }
}
