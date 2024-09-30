using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Attack : Goal_Base
{

    [SerializeField] int attackPriority = 95;


    [SerializeField] float MinAwarenessToAttack = 2f;
    [SerializeField] float AwarenessToStopAttack = 1f;

    DetectableTarget CurrentTarget;
    float CurrentPriority = 0f;

    public float attackRange = 2.3f;
    public float minAttackRange = 1.0f;
    //public float distance;

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

                    //distance = Vector3.Distance(agent.transform.position, candidate.rawPosition);
                    CurrentPriority = candidate.Awarness < AwarenessToStopAttack ? 0 : attackPriority;
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
        return Mathf.FloorToInt(CurrentPriority);
    }

    public override bool CanRun()
    {

        // no targets
        if (Sensors.ActiveTargets == null || Sensors.ActiveTargets.Count == 0)
            return false;
        //if (!Agent.isUpp)
        //{
        //    return false;
        //}
        // check if we have anything we are aware of
        foreach (var candidate in Sensors.ActiveTargets.Values)
        {
           if (candidate.Awarness >= MinAwarenessToAttack)
            {
                //if (IsPlayerLookingAtEnemy(candidate.rawPosition))
                {
                    if(noNav)
                    {
                        if (Vector3.Distance(controller.transform.position, candidate.rawPosition) < attackRange)
                        {

                            return true;
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(Agent.transform.position, candidate.rawPosition) < attackRange)
                        {

                            return true;
                        }
                    }

                  
                }

            }
        }

        return false;
    }

    bool IsPlayerLookingAtEnemy(Vector3 enemyPos)
    {
        float lookThreshold = 0.7f;
        Vector3 directionToEnemy = (enemyPos - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToEnemy);
        return dotProduct >= lookThreshold;
    }
}


