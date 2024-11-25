using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class EnemyHealth : MonoBehaviour, IDamageble
{
    public GameObject enemy;

    public float health = 1000;
    Animator animator;
    Ragdoll ragdoll;
    Regrow regrow;
    FSM_Walker fsm;
   



    private float leftLegHealth = 100f;
    private float rightLegHealth = 100f;
    private float leftArmHealth = 100f;
    private float rightArmHealth = 100f;
    private float headHealth = 100f;
    private float limbHealth = 100f;

    public Action<EnemyHealth, float> OnHealthChanged;
    public Action<EnemyHealth> OnDeath;

    public bool dead;

    [SerializeField] private AudioSource dmgAudio;
    [SerializeField] private List<AudioClip> dmgClips;
    [SerializeField] private AudioClip deathClip;

    [SerializeField] GameObject headblodParticle;
    [SerializeField] GameObject rightArmblodParticle;
    [SerializeField] GameObject leftArmblodParticle;
    [SerializeField] GameObject rightLegblodParticle;
    [SerializeField] GameObject leftLegblodParticle;

    private void Awake()
    {
        fsm = GetComponent<FSM_Walker>();
         animator = GetComponent<Animator>();
        regrow = GetComponent<Regrow>();
        ragdoll = GetComponent<Ragdoll>();
       
    }

    private void Update()
    {
        if (health <= 0)
        {          
            Death();
         
        }

    }
    public void Death()
    {
        //ragdoll.TriggerRagdoll(new Vector3(0, 0.5f, 0), new Vector3(0, 0, 0));
        Destroy(enemy, 25f);
        ragdoll.state = Ragdoll.RagdollState.Ragdoll;
        regrow.canRegrow = false;
        fsm.agentState = FSM_Walker.AgentState.Sleep;

        dmgAudio.clip = deathClip;
        if (!dmgAudio.isPlaying)
        {
            dmgAudio.Play();
        }

    }

    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {

        leftLegblodParticle.SetActive(false);
        rightLegblodParticle.SetActive(false);
        rightArmblodParticle.SetActive(false);
        leftArmblodParticle.SetActive(false);
        headblodParticle.SetActive(false);


        health -= damage;
        Impact(direction, point);
        //Damage(damage);
        Detachable d = regrow.GetDetachable(point);
        if (d != null)
        {
            if (d.leftLeg)
            {
                leftLegHealth -= damage;
            }
            else if(d.rightLeg)
            {
                rightLegHealth -= damage;
            }
            else if(d.rightArm) 
            {
                rightArmHealth -= damage;
            }
            else if(d.leftArm)
            {
                leftArmHealth -= damage;
            }
            else if(d.head)
            {
                headHealth -= damage;
            }
        }

        

        if (leftLegHealth <= 0)
        {
            leftLegblodParticle.SetActive(true);
            LoseLimbSound();
            regrow.Hit(point);
            if (d != null)
            {
                if (d.leftLeg)
                {
                    ragdoll.TriggerRagdoll(direction, point);
                }
            }
            leftLegHealth = limbHealth;

        }
        else if(rightLegHealth <= 0)
        {
            rightLegblodParticle.SetActive(true);
            LoseLimbSound();
            regrow.Hit(point);
            if (d != null)
            {
                if (d.leftLeg)
                {
                    ragdoll.TriggerRagdoll(direction, point);
                }
            }
            leftLegHealth = limbHealth;
          
        }
        else if (rightArmHealth <= 0)
        {
            rightArmblodParticle.SetActive(true);
            LoseLimbSound();
            regrow.Hit(point);
            rightArmHealth = limbHealth;
            
        }
        else if (leftArmHealth <= 0)
        {
            leftArmblodParticle.SetActive(true);
            LoseLimbSound();
            regrow.Hit(point);
            leftArmHealth = limbHealth;
          
        }
        else if (headHealth <= 0)
        {
            headblodParticle.SetActive(true);
            LoseLimbSound();
            regrow.Hit(point);
            headHealth = limbHealth;
        }

        if (health <= 0)
        {
            ragdoll.TriggerRagdoll(direction, point);
            regrow.Hit(point);
        }
        //dmgAudio.clip = dmgClips[1];
        //if (!dmgAudio.isPlaying)
        //{
        //    dmgAudio.Play();
        //}

        // PlayHitAnimation(direction);
        //Impact(direction, point);


    }

    void LoseLimbSound()
    {
        dmgAudio.clip = dmgClips[0];
        if (!dmgAudio.isPlaying)
        {
            dmgAudio.Play();
        }
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



