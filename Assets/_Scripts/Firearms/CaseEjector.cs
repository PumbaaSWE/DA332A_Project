using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;
using System;

public class CaseEjector : MonoBehaviour
{
    [SerializeField] GameObject Casing;
    [SerializeField] float EjectionForce = 1;
    [SerializeField] float MinHorizontalTorque = 1, MaxHorizontalTorque = 1, MinVerticalTorque = 0, MaxVerticalTorque = 0;
    [SerializeField] int CasingLayer = 16;
    [SerializeField] LayerMask InteractWith;
    [SerializeField] float LifeTime = 5;
    [SerializeField] float Mass = 1;
    [SerializeField] float LinearDrag = 0.05f;
    [SerializeField] float AngularDrag = 0.05f;
    [SerializeField] PhysicMaterial PhysicMat;
    [SerializeField] bool Debug;
    [SerializeField] int MaxCasings = 20;
    [SerializeField] float PitchVariance;
    Queue<GameObject> Casings = new();
    [SerializeField] AudioSource CasingSounds;
    [SerializeField] float MinEmitVelocity = 5;

    public void Eject()
    {
        if (Casings.Count >= MaxCasings)
            Destroy(Casings.Dequeue());

        GameObject newCasing = Instantiate(Casing,transform.position,transform.rotation);
        newCasing.name = "Casing Particle";
        newCasing.layer = CasingLayer;
        newCasing.transform.localScale = transform.lossyScale;

        Casing casing = newCasing.AddComponent<Casing>();
        casing.PlaySound = () => PlaySound();
        casing.MinEmitVelocity = MinEmitVelocity;
        casing.SetDestroy(LifeTime);

        // Creates and sets the values for the collider
        MeshCollider collider = newCasing.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.includeLayers = InteractWith;
        collider.material = PhysicMat;

        // Creates and sets the values for the rigidbody
        Rigidbody rb = newCasing.AddComponent<Rigidbody>();
        rb.mass = Mass;
        rb.drag = LinearDrag;
        rb.angularDrag = AngularDrag;
        rb.includeLayers = InteractWith;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        if (Debug)
            rb.useGravity = false;
        
        rb.AddForce(transform.right * EjectionForce, ForceMode.Impulse);
        rb.AddTorque(transform.up * Random.Range(MinHorizontalTorque,MaxHorizontalTorque), ForceMode.Impulse);
        rb.AddTorque(transform.forward * Random.Range(MinVerticalTorque, MaxVerticalTorque), ForceMode.Impulse);

        Casings.Enqueue(newCasing);

        //Destroy(newCasing, LifeTime);
    }

    public void PlaySound()
    {
        CasingSounds.pitch = 1 + Random.Range(-PitchVariance, PitchVariance);
        CasingSounds.PlayOneShot(CasingSounds.clip);
    }
}