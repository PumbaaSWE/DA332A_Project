using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Action_Stalk : Action_Base
{
    [SerializeField] float SearchRange = 10f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Stalk) });
    Goal_Stalk stalkGoal;
    private Vector3 lastMoveTarget;
    [SerializeField] private float positionUpdateTolerance = 0.5f;
    public override List<System.Type> GetSupportedGoals()
    {
        return SupportedGoals;
    }

    public override float GetCost()
    {
        return 1f;
    }

    public override void OnActivated(Goal_Base linkedGoal)
    {
        base.OnActivated(linkedGoal);

        stalkGoal = (Goal_Stalk)LinkedGoal;

        if (Agent.AtDestination || Vector3.Distance(Agent.transform.position, stalkGoal.MoveTarget) > SearchRange)
        {
            Vector3 directionToPlayer = (stalkGoal.MoveTarget - Agent.transform.position).normalized;
            Vector3 targetPosition = stalkGoal.MoveTarget - directionToPlayer * SearchRange;

            Agent.MoveTo(targetPosition);
        }
    }

    public override void OnTick()
    {

        if (Agent.AtDestination)
        {
               FacePlayer();
            if (ShouldUpdateMoveTarget(stalkGoal.MoveTarget))
            {
                MoveAgentToTarget();
            }
        }
    }
    private bool ShouldUpdateMoveTarget(Vector3 newMoveTarget)
    {
        return Vector3.Distance(lastMoveTarget, newMoveTarget) > positionUpdateTolerance;
    }

    private void MoveAgentToTarget()
    {
        lastMoveTarget = stalkGoal.MoveTarget;
        Agent.MoveTo(lastMoveTarget);
    }
    private void FacePlayer()
    {
        Vector3 directionToPlayer = (stalkGoal.MoveTarget - Agent.transform.position).normalized;

        directionToPlayer.y = 0;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            Agent.transform.rotation = Quaternion.Slerp(Agent.transform.rotation, lookRotation, Time.deltaTime * 5f); 
        }
    }
}
