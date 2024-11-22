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
    private void Awake()
    {
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
        ragdoll.TriggerRagdoll(new Vector3(0, 1f, 0), new Vector3(0, 0, 0));
        Destroy(enemy, 3.5f);
    }

    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
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
            LoseLimbSound();
            regrow.Hit(point);
            rightArmHealth = limbHealth;
        }
        else if (leftArmHealth <= 0)
        {
            LoseLimbSound();
            regrow.Hit(point);
            leftArmHealth = limbHealth;
        }
        else if (headHealth <= 0)
        {
            LoseLimbSound();
            regrow.Hit(point);
            headHealth = limbHealth;
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



