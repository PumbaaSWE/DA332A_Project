using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Throw : Action_Base
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform throwPoint;
    [SerializeField] float throwForce = 10f;
    [SerializeField] float throwInterval = 3f;   
    private float lastThrowTime = 0f;              

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Stalk) });
    Goal_Stalk stalkGoal;
    Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override List<System.Type> GetSupportedGoals()
    {
        return SupportedGoals;
    }

    public override float GetCost()
    {
        if (!IsPlayerLookingAtAI())
        {
            return 0f;
        }
        else
        {
            return 1f;
        }
    }

    public override void OnActivated(Goal_Base linkedGoal)
    {
        base.OnActivated(linkedGoal);
        stalkGoal = (Goal_Stalk)LinkedGoal;
    }

    public override void OnTick()
    {
        if (player != null && !IsPlayerLookingAtAI())
        {
            
            if (Time.time - lastThrowTime >= throwInterval)
            {
                ThrowObjectAtPlayer();
                lastThrowTime = Time.time; 
            }
        }
    }

    private bool IsPlayerLookingAtAI()
    {
        Vector3 directionToAI = (transform.position - player.position).normalized;
        float angle = Vector3.Angle(player.forward, directionToAI);
        return angle < 90f;
    }

    private void ThrowObjectAtPlayer()
    {
        GameObject projectile = Instantiate(projectilePrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (player.position - throwPoint.position).normalized;
            rb.AddForce(direction * throwForce, ForceMode.VelocityChange);
        }
        Destroy(projectile, 5f);  
    }

}