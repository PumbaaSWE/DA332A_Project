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
        MoveOrRetreat();
    }

    public override void OnTick()
    {
        if (Agent.AtDestination)
        {
            FacePlayer();
            MoveOrRetreat();
        }
    }

    private void MoveOrRetreat()
    {
        float distanceToPlayer = Vector3.Distance(Agent.transform.position, stalkGoal.MoveTarget);
        if (distanceToPlayer < SearchRange)
        {
            Vector3 directionAwayFromPlayer = (Agent.transform.position - stalkGoal.MoveTarget).normalized;
            Vector3 retreatPosition = Agent.transform.position + directionAwayFromPlayer * RetreatDistance;

            Agent.MoveTo(retreatPosition, true);
        }

        else
        {
            Vector3 directionToPlayer = (stalkGoal.MoveTarget - Agent.transform.position).normalized;
            Vector3 targetPosition = stalkGoal.MoveTarget - directionToPlayer * SearchRange;

            Agent.MoveTo(targetPosition, true);
        }
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
