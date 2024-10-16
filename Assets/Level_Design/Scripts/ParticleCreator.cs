using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCreator : MonoBehaviour, IDamageble
{
    // Start is called before the first frame update

    public GameObject particle;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
