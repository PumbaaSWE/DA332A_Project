using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Action_Investegate : Action_Base
{
    //[SerializeField] float SearchRange = 10f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Investagate) });
    int cost = 0;
    Goal_Investagate goal;
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
        goal = (Goal_Investagate)LinkedGoal;

        if (!climer)
        {
            agent.MoveTo(Sensors.soundLocation);
        }
        else
        {
            climberAgent.MoveTo(Sensors.soundLocation, false);
        }
        

    }
    public override void OnDeactivated()
    {
        base.OnDeactivated();
        Sensors.soundLocation = Vector3.zero;
    }

    public override void OnTick()
    {

        if (Sensors.soundLocation == Vector3.zero)
        {
            goal.priority = 0;
        }
        // arrived at destination?
        if (!climer)
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
            if (agent.AtDestination)
                OnActivated(LinkedGoal);
        }
        else
        {
            Vector3 directionToPlayer = (Sensors.soundLocation - climberAgent.transform.position).normalized;
            float distanceToSound = Vector3.Distance(Sensors.soundLocation, climberAgent.transform.position);

            if (distanceToSound < 5f)
            {
                goal.priority = 0;
            }
            else
            {
                goal.priority = 40;
            }

            if (climberAgent.AtDestination)
                OnActivated(LinkedGoal);
        }        
    }
}
