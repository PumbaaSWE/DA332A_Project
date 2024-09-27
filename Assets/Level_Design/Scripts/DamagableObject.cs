using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamagableObject : MonoBehaviour, IDamageble
{
    
    public UnityEvent onDamage;

    public virtual void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        onDamage?.Invoke();
    }

}
