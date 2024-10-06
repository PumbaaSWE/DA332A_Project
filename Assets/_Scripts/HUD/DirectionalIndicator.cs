using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalIndicator : MonoBehaviour
{
    TakeDamageHUD takeDamageHUD;
    Transform targetTransform;
    Vector3 directon;
    public float TimeCreated { get; private set; } 

    internal void Init(TakeDamageHUD takeDamageHUD, Transform targetTransform)
    {
        this.takeDamageHUD = takeDamageHUD;
        this.targetTransform = targetTransform;
    }


    public void Activate(Vector3 directon, float damage)
    {
        this.directon = directon;
        TimeCreated = Time.time;
        ComputeRotation();
        gameObject.SetActive(true);
    }

    public DirectionalIndicator Deactivate()
    {
        gameObject.SetActive(false);
        return this;
    }

    void Awake()
    {
        //Deactivate();
    }

    void Update()
    {
        ComputeRotation();
    }

    private void ComputeRotation()
    {
        float z = -Vector3.SignedAngle(targetTransform.forward, directon, targetTransform.up);
        transform.rotation = Quaternion.Euler(0, 0, z);
    }
}
