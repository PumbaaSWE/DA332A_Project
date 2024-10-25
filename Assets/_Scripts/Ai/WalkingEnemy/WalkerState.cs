
using UnityEngine;

public class WalkerState : MonoBehaviour, IDamageble
{
    Health health;
    FSM fSM;
    RagdollLims ragdoll;
    public float deahtHealth = 600;
    private void Awake()
    {
        ragdoll = GetComponent<RagdollLims>();
        fSM = GetComponent<FSM>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        //CheckLimbs();
        //else
        //{
        //    fSM.agentStatehit = FSM.AgentHit.Normal;
        //}
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
            ragdoll.TriggerRagdoll(direction, point);
            CheckLimbs();
        }

    }
    public void CheckLimbs()
    {
      
        if (ragdoll.IsHeadDetached())
        {
            fSM.agentStatehit = FSM.AgentHit.Blind;
            ragdoll.Regrow();
        }
        else if (ragdoll.IsArmDetached())
        {
            fSM.agentStatehit = FSM.AgentHit.Armless;
            ragdoll.Regrow();
        }
        else if (ragdoll.IsLegDetached())
        {

        }
        else
        {
            fSM.agentStatehit = FSM.AgentHit.Normal;
        }
    }

    public void NormalState()
    {
        if (!ragdoll.IsHeadDetached() && !ragdoll.IsArmDetached() && !ragdoll.IsLegDetached())
        {
            fSM.agentStatehit = FSM.AgentHit.Normal;
            Debug.Log("normalstate");
        }      
    }
}
