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
                if(!noNav)
                {
                    agent.MoveTo(closestFood.transform.position);
                }
                else
                {
                    Agent.MoveTo(closestFood.transform.position);
                }
            }
            else
            {
                eatGoal.prio -= 30;
            }
             
        }
        else
        {
            if (!noNav)
            {
                Vector3 location = agent.PickLocationInRange(SearchRange);
                agent.MoveTo(location);
            }
            else
            {
                Vector3 location = Agent.PickLocationInRange(SearchRange);
                Agent.MoveTo(location);
            }

           
        }
    }

    public override void OnTick()
    {
        timeSpentAtDestination += Time.deltaTime;
        if (!noNav)
        {
            if (agent.AtDestination)
            {
                OnActivated(LinkedGoal);
            }
        }
        else
        {
            if (Agent.AtDestination)
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

       
    }
    private void PickNewLocation()
    {

        if (!noNav)
        {
            Vector3 location = agent.PickLocationInRange(SearchRange);
            agent.MoveTo(location);
        }
        else
        {
            Vector3 location = Agent.PickLocationInRange(SearchRange);
            Agent.MoveTo(location);
        }

        timeSpentAtDestination = 0f;
        //cooldownTimer = WanderCooldown;
    }
}
