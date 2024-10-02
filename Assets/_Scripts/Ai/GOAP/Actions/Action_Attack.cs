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
    public ParticleSystem attackParticles;
    [SerializeField] Transform target;
    public PlayerDataSO player;
    private void Start()
    {
        player.NotifyOnPlayerChanged(OnPlayer); ;
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
        animator.SetBool("Attack", true);
        
    }
    public override void OnDeactivated()
    {
        base.OnDeactivated();

        animator.SetBool("Attack", false);
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
        //if (!noNav)
        //{
        //    agent.AttackBehaviour(attackGoal.AttackTarget, minAttackRange);
        //}
        //else
        {

            Agent.AttackBehaviour(attackGoal.AttackTarget, minAttackRange);

            //StartCoroutine(AttackCooldown(.5f));

            Vector3 targetDelta = target.position - transform.position;
            attckTimer -= Time.deltaTime;
            if (targetDelta.sqrMagnitude < 3 && attckTimer < 0)
            {
                if (attackParticles != null)
                {
                    var main = attackParticles.main;
                    main.startSize = 1.5f;  
                    main.startColor = Color.red;
                    attackParticles.Play();
                }

                //Debug.Log("Attack!!");
                if (target.TryGetComponent(out IDamageble damageble))
                {
                    damageble.TakeDamage(transform.position, targetDelta, 15);
                    attckTimer = 1.4f;
                    //Debug.Log("Do damage!!");
                }
            }
        }
    }
}
