
using UnityEngine;

public class WalkerState : MonoBehaviour, IDamageble
{
    Health health;
    FSM fSM;
    RagdollLims ragdoll;
    public float deahtHealth = 1400;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        ragdoll = GetComponent<RagdollLims>();
        fSM = GetComponent<FSM>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (deahtHealth <= 0)
        {
            ragdoll.Death();
        }


        NormalState();
    }
    public void TurnOfFSM()
    {

    }

    public void TurnOnFSM()
    {

    }
    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        health.Damage(damage);
        deahtHealth -= damage;

        if (health.Value <= 0)
        {
            // ragdoll.TriggerRagdoll(direction, point);
            ragdoll.TakeDMG(direction, point);
            CheckLimbs();
        }
       
        
       // PlayHitAnimation(direction);
        Impact(direction, point);
        

    }
    public void CheckLimbs()
    {
      
        if (ragdoll.IsHeadDetached())
        {
            fSM.agentStatehit = FSM.AgentHit.Blind;
            ragdoll.Regrow(false);
        }
        else if (ragdoll.IsArmDetached())
        {
            fSM.agentStatehit = FSM.AgentHit.Armless;
            ragdoll.Regrow(true);
        }
        else if (ragdoll.IsLegDetached() && !fSM.isCrawling)
        {
            //ragdoll.TriggerRagdoll();
        }
        //else
        //{
        //    fSM.limbStatehit = FSM.AgentHit.Normal;
        //    Debug.Log("normaldr");
        //}
    }

    public void NormalState()
    {
        if (!ragdoll.IsHeadDetached() && !ragdoll.IsArmDetached() && !ragdoll.IsLegDetached() && ragdoll.state == RagdollLims.RagdollState.Default )
        {
            fSM.agentStatehit = FSM.AgentHit.Normal;
        }      
    }

    private void PlayHitAnimation(Vector3 direction)
    { 
        //if(!fSM.isCrawling)
        {
            if (IsFrontHit(direction))
            {
                animator.Play("Standing React Large Gut");
                animator.Play("Standing React Small From Headshot");
            }
            if (IsBackHit(direction))
            {
                animator.Play("Standing React Small From Back");
            }
        }
        
    }

    private bool IsFrontHit(Vector3 direction)
    {
        return Vector3.Dot(direction, transform.forward) < 0;
    }
    private bool IsBackHit(Vector3 direction)
    {     
        return Vector3.Dot(direction, transform.forward) > 0;
    }
    public void Impact(Vector3 from, Vector3 at)
    {
        Vector3 dir = transform.position - from;
        float height = at.y - transform.position.y;

        float fwd = -Vector3.Dot(dir, transform.forward);
        float right = -Vector3.Dot(dir, transform.right);
        float deg = 0.707f;
        if (fwd > deg)
        {

            if (height > 1.5f)
            {
                animator.Play("Standing React Small From Headshot", 1);
            }
            else
            {
                animator.Play("Standing React Large Gut", 1);

            }
        }
        if (fwd < -deg)
        {
            animator.Play("Standing React Small From Back", 1);
        }
        if (right > deg)
        {
            animator.Play("Standing React Large From Right", 1);
        }
        if (right < -deg)
        {
            animator.Play("Standing React Large From Left", 1);
        }

    }
}
