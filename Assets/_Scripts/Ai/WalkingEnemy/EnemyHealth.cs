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

        if (Value <= 0)
        {
            ragdoll.TriggerRagdoll(direction, point);
            regrow.TriggerRegrow(point);
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
}
