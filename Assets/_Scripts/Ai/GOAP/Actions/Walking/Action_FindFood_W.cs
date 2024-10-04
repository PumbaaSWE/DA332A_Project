using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Action_FindFood_W : Action_Base
{
    [SerializeField] float SearchRange = 10f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Eat_W) });

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

        Vector3 location = agent.PickLocationInRange(SearchRange);

        agent.MoveTo(location);
    }

    public override void OnTick()
    {
        // arrived at destination?
        if (agent.AtDestination)
            OnActivated(LinkedGoal);
    }
}
