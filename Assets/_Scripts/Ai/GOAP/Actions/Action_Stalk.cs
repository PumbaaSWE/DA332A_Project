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

        if(!noNav)
        {
            if (agent.AtDestination || Vector3.Distance(agent.transform.position, stalkGoal.MoveTarget) > SearchRange)
            {
                Vector3 directionToPlayer = (stalkGoal.MoveTarget - agent.transform.position).normalized;
                Vector3 targetPosition = stalkGoal.MoveTarget - directionToPlayer * SearchRange;

                agent.MoveTo(targetPosition);
            }
        }
        else
        {
            if (Agent.AtDestination || Vector3.Distance(Agent.transform.position, stalkGoal.MoveTarget) > SearchRange)
            {
                Vector3 directionToPlayer = (stalkGoal.MoveTarget - Agent.transform.position).normalized;
                Vector3 targetPosition = stalkGoal.MoveTarget - directionToPlayer * SearchRange;

                Agent.MoveTo(targetPosition);
            }
        }

       
    }

    public override void OnTick()
    {
        if(!noNav)
        {
            if (agent.AtDestination)
            {
                FacePlayer();
                OnActivated(LinkedGoal); // Continue following the player
            }
        }
        else
        {
            if (Agent.AtDestination)
            {
                FacePlayer();
                OnActivated(LinkedGoal); // Continue following the player
            }
        }
      
    }
  
    private void FacePlayer()
    {
        if (!noNav)
        {
            Vector3 directionToPlayer = (stalkGoal.MoveTarget - agent.transform.position).normalized;

            directionToPlayer.y = 0;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
        else
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
}
