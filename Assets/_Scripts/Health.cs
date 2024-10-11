using System;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;

    public Action<Health, float> OnHealthChanged;
    public Action<Health> OnDeath;

    public bool dead;

    public float Value => health;
    public float MaxHealth => maxHealth;

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

    [ContextMenu("Add Hitboxes")]
    public void AddHitboxes()
    {
        var colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            //if (colliders[0].isTrigger) continue;
            Hitbox h = colliders[i].GetOrAddComponent<Hitbox>();
            h.Health = this;
        }
    }
}