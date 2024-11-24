using System;
using UnityEngine;

public class Hitbox : MonoBehaviour, IDamageble
{
    public Health Health { get => health; set => health = value; }
    [SerializeField]private Health health;
    [SerializeField]private Rigidbody rb;

    public event Action<Vector3, Vector3, float> OnHit;
    public event Action<Vector3, Vector3, float, Rigidbody> OnHitRb;

    private void Awake()
    {
        if(!health) health = GetComponentInParent<Health>();
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        health.Damage(damage);
        OnHit?.Invoke(point, direction, damage);
        OnHitRb?.Invoke(point, direction, damage, rb);
    }
}
