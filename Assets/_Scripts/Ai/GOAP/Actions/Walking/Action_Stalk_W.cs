using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Action_Stalk_W : Action_Base
{
    [SerializeField] float SearchRange = 10f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Stalk_W) });
    Goal_Stalk_W stalkGoal;
    public PlayerDataSO player;
    [SerializeField] Transform target;
    private void Start()
    {
        // player = GameObject.FindGameObjectWithTag("Player").transform;
        player.NotifyOnPlayerChanged(OnPlayer);
    }
    private void OnDestroy()
    {
        player.UnsubscribeOnPlayerChanged(OnPlayer);
    }
    private void OnPlayer(Transform obj)
    {
        target = obj;
        if (target)
        {

            //do if not null
        }
    }
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

        stalkGoal = (Goal_Stalk_W)LinkedGoal;

        // Check if the agent is at the destination or if it's too far from the target
        if (agent.AtDestination || Vector3.Distance(agent.transform.position, stalkGoal.MoveTarget) > SearchRange)
        {
            Vector3 directionToPlayer = (stalkGoal.MoveTarget - agent.transform.position).normalized;
            Vector3 targetPosition = stalkGoal.MoveTarget - directionToPlayer * SearchRange;

            // Move agent to a position within SearchRange distance from the player
            agent.MoveTo(targetPosition);
        }
    }

    public override void OnTick()
    {
        // When the agent arrives at the destination, face the player
        if (agent.AtDestination)
        {
            FacePlayer();
            OnActivated(LinkedGoal); // Continue following the player
        }
        if(IsPlayerLookingAtAI())
        {
            stalkGoal.prio -= 1;
        }
    }

    private void FacePlayer()
    {
        // Calculate direction to the player
        Vector3 directionToPlayer = (stalkGoal.MoveTarget - agent.transform.position).normalized;

        // Only rotate on the Y-axis to prevent tilting
        directionToPlayer.y = 0;

        // Check if there is a direction to face
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * 5f); // Smooth rotation
        }
    }

    private bool IsPlayerLookingAtAI()
    {
        Vector3 directionToAI = (transform.position - player.PlayerTransform.position).normalized;
        float angle = Vector3.Angle(player.PlayerTransform.forward, directionToAI);
        return angle < 3f;
    }
}
