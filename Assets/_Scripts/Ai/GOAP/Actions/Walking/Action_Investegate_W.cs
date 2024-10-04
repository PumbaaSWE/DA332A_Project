using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Action_Investegate_W : Action_Base
{
    [SerializeField] float SearchRange = 10f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Investagate_W) });
    int cost = 0;
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



        agent.MoveTo(Sensors.soundLocation);
    }

    public override void OnTick()
    {
        // arrived at destination?
        if (agent.AtDestination)
            OnActivated(LinkedGoal);
    }
}
