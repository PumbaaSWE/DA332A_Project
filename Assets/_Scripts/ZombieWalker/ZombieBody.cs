using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    [SerializeField] private BodyPart[] legs; 
    [SerializeField] private BodyPart[] arms; 
    [SerializeField] private BodyPart head; 
    //[SerializeField] private BodyPart tail; 
    [SerializeField] private float legHealth = 100;
    [SerializeField] private float armHealth = 100;
    [SerializeField] private float headHealth = 100;
    [SerializeField] private float totalHealth = 400;

    

    void Awake()
    {
        Debug.Assert(legs != null, "ZombieBoby - BodyPart legs missing please assign in inspector");
        Debug.Assert(arms != null, "ZombieBoby - BodyPart arms missing please assign in inspector");
        Debug.Assert(head != null, "ZombieBoby - BodyPart head missing please assign in inspector");
        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].OnDetach += OnLegDetach;
        }
        for (int i = 0; i < arms.Length; i++)
        {
            arms[i].OnDetach += OnArmDetach;
        }
        head.OnDetach += OnHeadDetach;
        //if (tail) tail.OnDetach += OnTailDetach;

        Hitbox[] hbs = GetComponentsInChildren<Hitbox>();
        foreach (var hb in hbs)
        {
            hb.OnHitRb += Hb_OnHitRb;
        }
    }

    private void Hb_OnHitRb(Vector3 point, Vector3 dir, float damage, Rigidbody rb)
    {
        //rb.AddForceAtPosition(dir*100, point, ForceMode.Impulse);
    }

    //private void OnTailDetach(bool obj)
    //{

    //}

    private void OnLegDetach(bool doRegrow)
    {
        GetComponent<RagdollController>().EnableRagdoll();
    }

    private void OnArmDetach(bool doRegrow)
    {

    }

    private void OnHeadDetach(bool doRegrow)
    {

    }

    public bool HasLegs()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            if (legs[i].IsDetached) return false;
        }
        return true;
    }

    public bool HasArms()
    {
        for (int i = 0; i < arms.Length; i++)
        {
            if (arms[i].IsDetached) return false;
        }
        return true;
    }

    public bool HasHead()
    {
        return head.IsDetached;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        if(legs != null)
            for (int i = 0; i < legs.Length; i++)
                if(legs[i]) legs[i].MaxHealth = legHealth;
        if (arms != null)
            for (int i = 0; i < arms.Length; i++)
                if (arms[i]) arms[i].MaxHealth = armHealth;
        if(head) head.MaxHealth = headHealth;

        if (TryGetComponent(out Health health))
        {
            health.MaxHealth = totalHealth;
            health.SetHealth(totalHealth);
        }
    }
}
