using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ThrowDmg : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 targetDelta = other.transform.position - transform.position;
            if (other.TryGetComponent(out IDamageble damageble)) { 
               
                damageble.TakeDamage(transform.position, targetDelta, 15);
            
            }
        
        }
    }
   
}
