
using UnityEngine;


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
    int layerIndexNoArms = 4;
    int layerIndexNoHead = 5;

    public bool standing;

    public bool noHead;


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
            OnStateExit(previousState);
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

    // redo later
    void SetLayerActive(bool isActive)
    {
        float weight = isActive ? 1f : 0f;
        animator.SetLayerWeight(layerIndex, weight);
    }
    void SetNoArmsLayerActive(bool isActive)
    {
        float weight = isActive ? 1f : 0f;
        animator.SetLayerWeight(layerIndexNoArms, weight);
    }
    void SetNoHeadLayerActive(bool isActive)
    {
        float weight = isActive ? 1f : 0f;
        animator.SetLayerWeight(layerIndexNoHead, weight);
    }
    public void CheckLimbs()
    {
        if (limbStatehit == AgentHit.StandUp || limbStatehit == AgentHit.Wait)
        {
            return;
        }

        if (regrow.IsHeadDetached())
        {
            SetNoHeadLayerActive(true);
            animator.speed = 0.5f;
            //animator.SetFloat("LayerSpeed", 0.5f);
            //animator.SetFloat("velx", 0.5f);
        }
        else
        {
            animator.speed = 1f;
            SetNoHeadLayerActive(false);
            //animator.SetFloat("LayerSpeed", 1f);
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
            else if(standing)
            {
                limbStatehit = AgentHit.Normal;
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
    void OnStateExit(AgentHit oldState)
    {
        if (oldState == AgentHit.Armless)
        {
            SetNoArmsLayerActive(false);
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
        //fsm.agentState = FSM_Walker.AgentState.Sleep;


        //////////////////////////
        //fsm.sleep = true;
    }
    void ArmBehavior()
    {
        //if (!standing)
        //{
        //    limbStatehit = AgentHit.StandUp;
        //}
        fsm.SynchronizeAnimatorAndAgent();

        if(standing)
        {
            SetNoArmsLayerActive(true);
        }
        else
        {
            SetNoArmsLayerActive(false);
        }
       
    }
    void StandUp()
    {

        fsm.sleep = true;
        //fsm.agentState = FSM_Walker.AgentState.Sleep;
        SetLayerActive(false);
        animator.Play("Standing Up", 0, 0);
        limbStatehit = AgentHit.Wait;
    }

    void CheckStanding()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Standing Up")  )
        {
            standing = true;
            fsm.sleep = false;
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
        SetNoArmsLayerActive(false);
        SetLayerActive(false);
        fsm.SynchronizeAnimatorAndAgent();
    }

}


