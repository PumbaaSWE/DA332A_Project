using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGoal
{
    int CalculatePriority();
    bool CanRun();
    void OnTickGoal();
    void OnGoalActivated(Action_Base linkedAction);
    void OnGoalDeactivated();
}
public class Goal_Base : MonoBehaviour, IGoal
{
    public bool noNav;
    protected MoveTowardsController controller; 

    protected CharacterAgent Agent;
    protected AwarenessSystem Sensors;
    protected GOAPUI DebugUI;
    protected Action_Base LinkedAction;
    protected RangeWeapon rangeWeapon;
    void Awake()
    {
        if (!noNav)
        {
            Agent = GetComponent<CharacterAgent>();
        }
        else
        {
            controller = GetComponent<MoveTowardsController>();
        }

        Sensors = GetComponent<AwarenessSystem>();
    }

    void Start()
    {
        DebugUI = FindObjectOfType<GOAPUI>();

        rangeWeapon = GetComponentInChildren<RangeWeapon>();
        //DebugUI = FindObjectOfType<GOAPUI>();
        // DebugUI = FindAnyObjectByType<GOAPUI>();
    }
    void Update()
    {
        OnTickGoal();
      
        //DebugUI.UpdateGoal(this, GetType().Name, LinkedAction ? "Running" : "Paused", CalculatePriority());


    }

    public virtual int CalculatePriority()
    {
        return -1;
    }
    public virtual bool CanRun()
    {
        return false;
    }


    public virtual void OnTickGoal()
    {

    }

    public virtual void OnGoalActivated(Action_Base linkedAction)
    {
        LinkedAction = linkedAction;

    }

    public virtual void OnGoalDeactivated()
    {
        LinkedAction = null;
    }
}
