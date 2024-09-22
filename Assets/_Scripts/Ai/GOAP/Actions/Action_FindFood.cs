using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_FindFood : Action_Base
{
    [SerializeField] float SearchRange = 10f; // R�ckvidd f�r att s�ka efter mat

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Eat) });

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
                Agent.MoveTo(closestFood.transform.position);
            }
        }
        else
        {
      
            Vector3 location = Agent.PickLocationInRange(SearchRange);
            Agent.MoveTo(location);
        }
    }

    public override void OnTick()
    {

        if (Agent.AtDestination)
        {
            OnActivated(LinkedGoal);
        }
    }
}
