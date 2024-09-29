using System;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour, IDamageble
{
    
    public Health health;
    public event Action<Vector3, Vector3, float> OnHit;
    
    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        health.Damage(damage);
        OnHit?.Invoke(point, direction, damage);
        //Debug.Log("PlayerHitbox takes damage!! " + damage);
    }
}
