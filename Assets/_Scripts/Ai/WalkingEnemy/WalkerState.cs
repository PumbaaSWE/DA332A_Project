
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
        
        //else
        //{
        //    fSM.agentStatehit = FSM.AgentHit.Normal;
        //}
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
        //if (ragdoll.IsLegDetached())
        //{
        //    ragdoll.EnableRagdoll();

        //    fSM.SetAgentActive(false);
        //    fSM.isCrawling = true;
        //}

        if (ragdoll.IsHeadDetached())
        {
            fSM.agentStatehit = FSM.AgentHit.Blind;
            ragdoll.Regrow();
        }
        if (ragdoll.IsArmDetached())
        {
            fSM.agentStatehit = FSM.AgentHit.Armless;
            ragdoll.Regrow();
        }
    }
}
