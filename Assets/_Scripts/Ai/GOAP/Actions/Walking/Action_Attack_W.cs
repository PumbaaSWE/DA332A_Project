using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Attack_W : Action_Base
{
    //[SerializeField] float bullets = 10f;
    public float attackRange = 2.5f;
    public float minAttackRange = 1.0f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Attack_W) });
    Goal_Attack_W attackGoal;
    public override List<System.Type> GetSupportedGoals()
    {
        return SupportedGoals;
    }

    public override float GetCost()
    {
        //bullets

        return 0;
    }

    public override void OnActivated(Goal_Base linkedGoal)
    {
        base.OnActivated(linkedGoal);
        attackGoal = (Goal_Attack_W)LinkedGoal;
    }

    public override void OnTick()
    {
        agent.AttackBehaviour(attackGoal.AttackTarget, minAttackRange);
    }
}
