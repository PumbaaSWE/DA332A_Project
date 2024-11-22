using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class EnemyHealth : MonoBehaviour, IDamageble
{
    public GameObject enemy;

    public float deathHealth = 1400;
    Animator animator;
    Ragdoll ragdoll;
    Regrow regrow;
    [SerializeField] private float health;
    [SerializeField] private float legHealth;
    [SerializeField] private float maxHealth;

    public Action<EnemyHealth, float> OnHealthChanged;
    public Action<EnemyHealth> OnDeath;

    public bool dead;

    public float Value => health;
    public float MaxHealth => maxHealth;

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
        if (deathHealth <= 0)
        {
            Death();
        }

    }
    public void Death()
    {
        ragdoll.TriggerRagdoll(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        Destroy(enemy, 1.5f);
    }

    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        deathHealth -= damage;
        Impact(direction, point);
        //Damage(damage);
        Detachable d = regrow.GetDetachable(point);
        if (d != null)
        {
            if (d.leg)
            {
                legHealth -= damage;
            }
            else
            {
                health -= damage;
            }
        }

       

        if (legHealth <= 0)
        {
            LoseLimbSound();
            regrow.Hit(point);
            //regrow.TriggerRegrow(point);
            if (d != null)
            {
                if (d.leg)
                {
                    ragdoll.TriggerRagdoll(direction, point);
                }
            }
            legHealth = maxHealth;
        }
        else if (health <= 0)
        {
            LoseLimbSound();
            regrow.Hit(point);
            health = maxHealth;
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

    public void Reset()
    {
        health = maxHealth;
        dead = false;
    }

    public void Damage(float amount)
    {
        if (amount < 0)
            return;

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            if (!dead)
            {
                dead = true;
                OnDeath?.Invoke(this);
            }

        }
        OnHealthChanged?.Invoke(this, -amount);
    }

    public void Heal(float amount)
    {
        if (amount < 0)
            return;

        health += amount;

        if (health > maxHealth)
        {
            health = maxHealth;
        }
        OnHealthChanged?.Invoke(this, amount);
    }

    public void SetHealth(float health)
    {
        this.health = health;
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



