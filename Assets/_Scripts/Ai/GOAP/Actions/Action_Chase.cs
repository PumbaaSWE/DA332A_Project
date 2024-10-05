using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Chase : Action_Base
{
    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Chase) });

    Goal_Chase ChaseGoal;
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

        // cache the chase goal
        ChaseGoal = (Goal_Chase)LinkedGoal;


        if(climer)
        {
            animator.SetBool("Attack", false);
            climberAgent.MoveTo(ChaseGoal.MoveTarget, false);
        }
        else
        {
            agent.MoveTo(ChaseGoal.MoveTarget);
        }
       
        
    }

    public override void OnDeactivated()
    {
        // Keep this?
        //climberAgent.MoveTo(climberAgent.transform.position);
        //
        base.OnDeactivated();

        ChaseGoal = null;
    }

    public override void OnTick()
    {
        // SoundManager.Instance.salamanderChannel.PlayOneShot(SoundManager.Instance.salamanderChase);

        if (climer)
        {
            if (ShouldUpdateMoveTarget(ChaseGoal.MoveTarget))
            {
                MoveAgentToTarget();
            }
        }
        else
        {
            agent.MoveTo(ChaseGoal.MoveTarget);
        }
       
    }

    private bool ShouldUpdateMoveTarget(Vector3 newMoveTarget)
    {
        return Vector3.Distance(lastMoveTarget, newMoveTarget) > positionUpdateTolerance;
    }

    private void MoveAgentToTarget()
    {
        lastMoveTarget = ChaseGoal.MoveTarget;
        climberAgent.MoveTo(lastMoveTarget, false); 
    }
}
