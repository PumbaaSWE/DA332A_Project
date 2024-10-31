using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limbstate : MonoBehaviour
{
    public enum AgentHit { Crawl, LegAndArmLess, Armless, StandUp, Normal }
    public AgentHit limbStatehit = AgentHit.Normal;

    FSM_Walker fsm;

    private void Awake()
    {
        fsm = GetComponent<FSM_Walker>();
    }

    private void Update()
    {
        UpdateState();
    }
    void UpdateState()
    {

        switch (limbStatehit)
        {
            case AgentHit.Crawl:
                CrawlBehavior();
                break;
            case AgentHit.LegAndArmLess:
                ArmAndLegBehavior();
                break;

            case AgentHit.Armless:
                ArmBehavior();
                break;
            case AgentHit.StandUp:
                StandUp();
                break;
            case AgentHit.Normal:
                Normal();
                break;
        }
    }


    void CrawlBehavior()
    {
        fsm.HandleCrawling();
    }

    void ArmAndLegBehavior()
    {
        fsm.agentState = FSM_Walker.AgentState.Sleep;
    }
    void ArmBehavior()
    {
        fsm.SynchronizeAnimatorAndAgent();
    }
    void StandUp()
    {
        fsm.agentState = FSM_Walker.AgentState.Sleep;
    }

    void Normal()
    {
        fsm.SynchronizeAnimatorAndAgent();
    }

}


