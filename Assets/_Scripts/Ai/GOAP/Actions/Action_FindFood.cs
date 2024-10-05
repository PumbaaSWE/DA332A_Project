using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_FindFood : Action_Base
{
    [SerializeField] float SearchRange = 10f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Eat) });
    Goal_Eat eatGoal;

    float MaxTimeAtDestination = 20f;
    private float timeSpentAtDestination = 0f;
    WallClimber controller;
    public override List<System.Type> GetSupportedGoals()
    {
        return SupportedGoals;
    }
    private void Start()
    {
        controller = GetComponent<WallClimber>();
    }
    public override float GetCost()
    {
        return 1f;
    }

    public override void OnActivated(Goal_Base linkedGoal)
    {
        base.OnActivated(linkedGoal);

        eatGoal = (Goal_Eat)LinkedGoal;
        Food[] foods = FindObjectsOfType<Food>();

        if (foods.Length > 0)
        {
       
            Food closestFood = null;
            float closestDistance = float.MaxValue;

            foreach (var food in foods)
            {
                float distance = Vector3.Distance(transform.position, food.transform.position);
                if (distance < closestDistance && distance <= SearchRange)
                {
                    closestDistance = distance;
                    closestFood = food;
                }
            }
         
            if (closestFood != null)
            {
              
                
                    climberAgent.MoveTo(closestFood.transform.position, false);
                
            }
            else
            {
                eatGoal.prio -= 30;
            }
             
        }
        else
        {
           
                Vector3 location = climberAgent.PickLocationInRange(SearchRange);
                climberAgent.MoveTo(location, false);
            

           
        }
    }

    public override void OnTick()
    {
        timeSpentAtDestination += Time.deltaTime;
       
            if (climberAgent.AtDestination)
            {
                OnActivated(LinkedGoal);
            }
            
            if (controller.Speed <= 0)
            {
                PickNewLocation();
            }
            if (timeSpentAtDestination >= MaxTimeAtDestination)
            {
               PickNewLocation();
            }
        

       
    }
    private void PickNewLocation()
    {

        
        
            Vector3 location = climberAgent.PickLocationInRange(SearchRange);
            climberAgent.MoveTo(location, false);
        

        timeSpentAtDestination = 0f;
        //cooldownTimer = WanderCooldown;
    }
}
