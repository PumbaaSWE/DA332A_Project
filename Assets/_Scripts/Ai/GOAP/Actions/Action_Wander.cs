using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Wander : Action_Base
{
    [SerializeField] float SearchRange = 20f;
    //[SerializeField] float WanderCooldown = 2f; 
    //private float cooldownTimer = 0f;
    float MaxTimeAtDestination = 5f;
    private float timeSpentAtDestination = 0f;
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
        timeSpentAtDestination += Time.deltaTime;
       
            if (Agent.AtDestination)
            {
                PickNewLocation();
            }
            else if (timeSpentAtDestination >= MaxTimeAtDestination) 
            {
                PickNewLocation(); 
            }
        
       
    }

    private void PickNewLocation()
    {

       
            Vector3 location = Agent.PickLocationInRange(SearchRange);
            Agent.MoveTo(location, false);
        

        timeSpentAtDestination = 0f;
        //cooldownTimer = WanderCooldown;
    }
}
