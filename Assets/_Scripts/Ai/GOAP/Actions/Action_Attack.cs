using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

public class Action_Attack : Action_Base
{
    //[SerializeField] float bullets = 10f;
    public float attackRange = 2.5f;
    public float minAttackRange = 1.0f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_Attack) });
    Goal_Attack attackGoal;
    float attckTimer;

    [SerializeField] Transform target;
    public PlayerDataSO player;
    private void Start()
    {
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
        //bullets

        return 0;
    }

    public override void OnActivated(Goal_Base linkedGoal)
    {
        base.OnActivated(linkedGoal);
        attackGoal = (Goal_Attack)LinkedGoal;
        if(climer)
        {
            animator.SetBool("Attack", true);
        }
        
    }
    public override void OnDeactivated()
    {
        base.OnDeactivated();
        if(climer)
        {
            animator.SetBool("Attack", false);
        }
    }
    //private IEnumerator AttackCooldown(float t)
    //{
    //    yield return new WaitForSeconds(t);
    //    animator.SetBool("Attack", true);
    //}
    //private IEnumerator AnimationCooldown(int idx, float t)
    //{
    //    yield return new WaitForSeconds(t);
       

    //}
    public override void OnTick()
    {
        if (!climer)
        {
            agent.AttackBehaviour(attackGoal.AttackTarget, minAttackRange);
            DoDmg(1.4f);
        }
        else
        {

            climberAgent.AttackBehaviour(attackGoal.AttackTarget, minAttackRange);
            DoDmg(1.35f);
            //StartCoroutine(AttackCooldown(.5f));
        }
       
    }

    void DoDmg(float attackTime)
    {
        Vector3 targetDelta = target.position - transform.position;
        attckTimer -= Time.deltaTime;
        if (targetDelta.sqrMagnitude < 3 && attckTimer < 0)
        {

            //Debug.Log("Attack!!");
            if (target.TryGetComponent(out IDamageble damageble))
            {
                damageble.TakeDamage(transform.position, targetDelta, 20);
                attckTimer = attackTime;
                //Debug.Log("Do damage!!");
            }
        }

    }
}
