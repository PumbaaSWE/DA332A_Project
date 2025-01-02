using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static HearingManager;

public class PlayerSound : MonoBehaviour
{
 
 
    public float walkingThreshold = 0.1f;
    float sound = 0.4f;
    private Rigidbody rb;
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
        float magnitude = velocity.magnitude;
        lastPosition = transform.position;

        if (magnitude > walkingThreshold)
        {
            HearingManager.Instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EFootstep, sound);
        }
    }
}
