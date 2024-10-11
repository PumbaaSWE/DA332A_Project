using UnityEngine;

public class Hitbox : MonoBehaviour, IDamageble
{


    public Health Health { get => health; set => health = value; }
    [SerializeField]private Health health;

    private void Awake()
    {
        if(!health) health = GetComponentInParent<Health>();
    }

    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        health.Damage(damage);
    }
}
