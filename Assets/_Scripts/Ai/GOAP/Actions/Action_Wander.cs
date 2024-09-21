using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Wander : Action_Base
{
    [SerializeField] float SearchRange = 10f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Wander) });

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

        Vector3 location = Agent.PickLocationInRange(SearchRange);

        Agent.MoveTo(location);
    }

    public override void OnTick()
    {
        // arrived at destination?
        if (Agent.AtDestination)
            OnActivated(LinkedGoal);
    }
}
