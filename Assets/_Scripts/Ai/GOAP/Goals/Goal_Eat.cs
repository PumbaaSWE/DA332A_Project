using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Eat : Goal_Base
{
    public int prio = 30;

    [SerializeField] float PriorityBuildRate = 1f;
    [SerializeField] float PriorityDecayRate = 0.1f;
    float CurrentPriority = 0f;

    public override void OnTickGoal()
    {
         CurrentPriority += PriorityBuildRate * Time.deltaTime;

        // if eat food prio = 0;
    }

    public override void OnGoalActivated(Action_Base linkedAction)
    {
        base.OnGoalActivated(linkedAction);

        CurrentPriority = prio;
    }

    public override int CalculatePriority()
    {
        return Mathf.FloorToInt(CurrentPriority);
    }

    public override bool CanRun()
    {
        return true;
    }
}
