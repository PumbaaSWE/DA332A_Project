using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCreator : MonoBehaviour, IDamageble
{
    // Start is called before the first frame update

    public GameObject particle;
    
    void Start()
    {

        var hbs = GetComponentsInChildren<Hitbox>();
        foreach (var hb in hbs)
        {
            hb.OnHit += TakeDamage;
        }
        if (TryGetComponent(out Health health))
        {
            health.OnDeath += (_) => UnsubHitboxes();
        }
        
    }

    private void UnsubHitboxes()
    {
        var hbs = GetComponentsInChildren<Hitbox>();
        foreach (var hb in hbs)
        {
            hb.OnHit -= TakeDamage;
        }
    }

    public virtual void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        CreateParticle(point);
    }

    public void CreateParticle(Vector3 point)
    {
        Instantiate(particle, point, Quaternion.identity);
    }
}
