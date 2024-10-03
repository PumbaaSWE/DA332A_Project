using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Action_Stalk : Action_Base
{
    [SerializeField] float SearchRange = 10f;
    [SerializeField] float RetreatDistance = 5f; 
    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Stalk) });
    Goal_Stalk stalkGoal;
    private Vector3 lastMoveTarget;
    //[SerializeField] private float positionUpdateTolerance = 0.5f;

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
        if (climer)
        {

            MoveOrRetreat();
        }
    }

    public override void OnTick()
    {
        if (climer)
        {
            if (climberAgent.AtDestination)
            {
                FacePlayer();
                MoveOrRetreat();
            }
        }
        
    }

    private void MoveOrRetreat()
    {
        float distanceToPlayer = Vector3.Distance(climberAgent.transform.position, stalkGoal.MoveTarget);
        if (distanceToPlayer < SearchRange)
        {
            Vector3 directionAwayFromPlayer = (climberAgent.transform.position - stalkGoal.MoveTarget).normalized;
            Vector3 retreatPosition = climberAgent.transform.position + directionAwayFromPlayer * RetreatDistance;

            climberAgent.MoveTo(retreatPosition, true);
        }

        else
        {
            Vector3 directionToPlayer = (stalkGoal.MoveTarget - climberAgent.transform.position).normalized;
            Vector3 targetPosition = stalkGoal.MoveTarget - directionToPlayer * SearchRange;

            climberAgent.MoveTo(targetPosition, true);
        }
    }

    private void FacePlayer()
    {
        Vector3 directionToPlayer = (stalkGoal.MoveTarget - climberAgent.transform.position).normalized;
        directionToPlayer.y = 0;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            climberAgent.transform.rotation = Quaternion.Slerp(climberAgent.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    



}
