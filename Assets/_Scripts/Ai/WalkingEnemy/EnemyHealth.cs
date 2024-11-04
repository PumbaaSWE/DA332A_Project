using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageble
{
    public float deahtHealth = 1400;
    Animator animator;
    Ragdoll ragdoll;
    Regrow regrow;
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;

    public Action<EnemyHealth, float> OnHealthChanged;
    public Action<EnemyHealth> OnDeath;

    public bool dead;

    public float Value => health;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        regrow = GetComponent<Regrow>();
        ragdoll = GetComponent<Ragdoll>();
    }
    private void Update()
    {
        if (deahtHealth <= 0)
        {
            //ragdoll.Death();
        }

    }

    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        Damage(damage);
        deahtHealth -= damage;
        Impact(direction, point);
        if (Value <= 0)
        {
            //regrow.TriggerRegrow(point);
            Detachable d = regrow.Hit(point);

            if (d != null)
            {
                if (d.leg)
                {
                    ragdoll.TriggerRagdoll(direction, point);
                }
            }
            
            
        }
        // PlayHitAnimation(direction);
        //Impact(direction, point);

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



