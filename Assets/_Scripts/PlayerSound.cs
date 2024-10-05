using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static HearingManager;

public class PlayerSound : MonoBehaviour
{
 
 
    public float walkingThreshold = 0.1f;
    float sound = 0.1f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();  
    }

    void Update()
    {
      
        if (rb.velocity.magnitude > walkingThreshold)
        {
            HearingManager.Instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EFootstep, sound);
        }
       
    }
}
