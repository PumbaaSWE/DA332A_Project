using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Goal_RangedAttack : Goal_Base
{


    [SerializeField] int attackPriority;


    [SerializeField] float MinAwarenessToAttack = 2f;
    [SerializeField] float AwarenessToStopAttack = 1f;

    DetectableTarget CurrentTarget;
    float CurrentPriority = 0f;
    [SerializeField] float shootingRange = 40f;
    [SerializeField] float meleeRange = 6f;

    public float distance;
 
    public Vector3 AttackTarget => CurrentTarget != null ? CurrentTarget.transform.position : transform.position;

 
    public override void OnTickGoal()
    {



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
                    distance = Vector3.Distance(Agent.transform.position, candidate.rawPosition);
                    CurrentPriority = candidate.Awarness < AwarenessToStopAttack ? 0 : attackPriority /*- distance*/;
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
            if (candidate.Awarness >= MinAwarenessToAttack)
            {
                CurrentTarget = candidate.detectable;
                CurrentPriority = attackPriority;
                return;
            }
        }


    }

    public override void OnGoalActivated(Action_Base linkedAction)
    {
        base.OnGoalActivated(linkedAction);

        CurrentPriority = attackPriority;
    }

    public override int CalculatePriority()
    {
        // lägg till om fienden är långt borta
        return Mathf.FloorToInt(CurrentPriority);
    }

    public override bool CanRun()
    {
        // add if somthing is in the way

        // no targets
        if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0 || CurrentTarget == null)
            return false;

      
        // check if we have anything we are aware of
        foreach (var candidate in Sensors.ActiveTargets.Values)
        {
            if (candidate.Awarness >= MinAwarenessToAttack)
            {
               
                    float distance = Vector3.Distance(Agent.transform.position, candidate.rawPosition);

                    if (distance < shootingRange && distance > meleeRange)
                    {
                        return true;
                        //if (rangeWeapon.ammoCount < 1)
                        //{
                        //    return false;
                        //}
                        //else
                        //{
                        //    return true;
                        //}
                    }           

            }
        }

        return false;
    }

    


}
