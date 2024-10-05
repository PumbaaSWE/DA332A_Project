using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Chase_W : Action_Base
{
    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Chase_W) });

    Goal_Chase_W ChaseGoal;

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

        // cache the chase goal
        ChaseGoal = (Goal_Chase_W)LinkedGoal;

        agent.MoveTo(ChaseGoal.MoveTarget);
    }

    public override void OnDeactivated()
    {
        // Keep this?
        //Agent.MoveTo(Agent.transform.position);
        //
        base.OnDeactivated();

        ChaseGoal = null;
    }

    public override void OnTick()
    {
        agent.MoveTo(ChaseGoal.MoveTarget);
    }
}
