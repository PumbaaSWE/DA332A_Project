using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Limbstate : MonoBehaviour
{
    public enum AgentHit { Crawl, LegAndArmLess, Armless, StandUp, Wait ,Normal }
    public AgentHit limbStatehit = AgentHit.Normal;
    private AgentHit previousState;

    FSM_Walker fsm;
    Regrow regrow;
    Ragdoll rag;

    Animator animator;
    int layerIndex = 3;

    public bool standing = true;

    private void Awake()
    {
        rag = GetComponent<Ragdoll>();
        animator = GetComponent<Animator>();
        regrow = GetComponent<Regrow>();    
        fsm = GetComponent<FSM_Walker>();
    }

    private void Update()
    {
        if (limbStatehit != previousState)
        {            
            previousState = limbStatehit;
            OnStateEnter(limbStatehit);
        }

        CheckLimbs();    
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
            case AgentHit.Wait:
                CheckStanding();
                break;
            case AgentHit.Normal:
                Normal();
                break;
        }
    }
    void SetLayerActive(bool isActive)
    {
        float weight = isActive ? 1f : 0f;
        animator.SetLayerWeight(layerIndex, weight);
    }
    public void CheckLimbs()
    {
        if (limbStatehit == AgentHit.StandUp || limbStatehit == AgentHit.Wait)
        {
            return;
        }

        if (regrow.IsArmDetached())
        {
            if (regrow.IsLegDetached())
            {
               
                limbStatehit = AgentHit.LegAndArmLess;
            }
            else
            {
                limbStatehit = AgentHit.Armless;
            }
        }
        else if (regrow.IsLegDetached() && !regrow.IsArmDetached())
        {
            limbStatehit = AgentHit.Crawl;
        }
        else
        {
            if (limbStatehit != AgentHit.Normal && !standing)
            {                
                limbStatehit = AgentHit.StandUp;
            }
        }
    }

    void OnStateEnter(AgentHit newState)
    {
        if (newState == AgentHit.LegAndArmLess)
        {
            rag.TriggerRagdoll(Vector3.zero, Vector3.zero);
        }

        if (newState == AgentHit.Armless && !standing)
        {
            rag.TriggerRagdoll(Vector3.zero, Vector3.zero);
        }
    }

    void CrawlBehavior()
    {
        standing = false;
        SetLayerActive(true);
        fsm.HandleCrawling();
    }

    void ArmAndLegBehavior()
    {
        standing = false;
        fsm.agentState = FSM_Walker.AgentState.Sleep;
    }
    void ArmBehavior()
    {
        if (!standing)
        {
            limbStatehit = AgentHit.StandUp;
        }
        fsm.SynchronizeAnimatorAndAgent();
    }
    void StandUp()
    {
        fsm.agentState = FSM_Walker.AgentState.Sleep;
        animator.Play("Standing Up", 0, 0);
        limbStatehit = AgentHit.Wait;
    }

    void CheckStanding()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Getting Up"))
        {
            standing = true;
            SetLayerActive(false);
            limbStatehit = AgentHit.Normal;
            fsm.agentState = FSM_Walker.AgentState.Idle;
        }
    }
    void Normal()
    {
        if(!standing)
        {
            limbStatehit = AgentHit.StandUp;
        }
        SetLayerActive(false);
        fsm.SynchronizeAnimatorAndAgent();
    }

}


