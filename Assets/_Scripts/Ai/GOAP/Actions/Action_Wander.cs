using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Wander : Action_Base
{
    [SerializeField] float SearchRange = 20f;
    [SerializeField] float WanderCooldown = 2f; // Tid mellan nya m�lpunkter
    private float cooldownTimer = 0f;

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

        PickNewLocation();
    }

    public override void OnTick()
    {

        //if (cooldownTimer > 0)
        //{
        //    cooldownTimer -= Time.deltaTime;
        //    return;
        //}


        if (Agent.AtDestination)
        {
            PickNewLocation();
        }
    }

    private void PickNewLocation()
    {

        Vector3 location = Agent.PickLocationInRange(SearchRange);
        Agent.MoveTo(location);
        //cooldownTimer = WanderCooldown;
    }
}