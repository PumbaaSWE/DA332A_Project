using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Action_Investegate_W : Action_Base
{
    [SerializeField] float SearchRange = 10f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Investagate_W) });
    int cost = 0;
    Goal_Investagate_W goal;
    public override List<System.Type> GetSupportedGoals()
    {
        return SupportedGoals;
    }

    public override float GetCost()
    {
        //if (Sensors.soundLocation != null)
        //{
        //    cost = 0;
        //}
        //else
        //{
        //    cost = 1;
        //}


        return cost;
    }

    public override void OnActivated(Goal_Base linkedGoal)
    {
        base.OnActivated(linkedGoal);

        goal = (Goal_Investagate_W)LinkedGoal;

        agent.MoveTo(Sensors.soundLocation);
    }

    public override void OnTick()
    {
        Vector3 directionToPlayer = (Sensors.soundLocation - agent.transform.position).normalized;
        float distanceToSound = Vector3.Distance(Sensors.soundLocation, agent.transform.position);

        if (distanceToSound < 5f)
        {
            goal.priority = 0;
        }
        else
        {
            goal.priority = 40;
        }
        // arrived at destination?
        if (agent.AtDestination)
            OnActivated(LinkedGoal);
    }
}
