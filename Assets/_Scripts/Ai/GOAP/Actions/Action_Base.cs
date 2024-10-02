using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Base : MonoBehaviour
{
    protected CharacterAgent agent;
    protected CharacterClimber climberAgent;
    protected AwarenessSystem Sensors;
    protected Goal_Base LinkedGoal;
    public bool climer;
    protected Animator animator;

    void Awake()
    {

        animator = GetComponentInChildren<Animator>();
        if (climer)
        {
            climberAgent = GetComponent<CharacterClimber>();
        }
        else
        {
            agent = GetComponentInChildren<CharacterAgent>();
        }

        Sensors = GetComponent<AwarenessSystem>();
    }

    public virtual List<System.Type> GetSupportedGoals()
    {
        return null;
    }

    public virtual float GetCost()
    {
        return 0f;
    }

    public virtual void OnActivated(Goal_Base linkedGoal)
    {
        LinkedGoal = linkedGoal;
    }

    public virtual void OnDeactivated()
    {
        LinkedGoal = null;
    }

    public virtual void OnTick()
    {

    }
}
