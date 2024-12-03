using System;
using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private BodyPart[] legs; 
    [SerializeField] private BodyPart[] arms; 
    [SerializeField] private BodyPart head;
    //[SerializeField] private BodyPart tail; 
    [Header("Health Settings")]
    [SerializeField][Tooltip("Total health before death (overrides Health-script)")] private float totalHealth = 400;
    [Header("Leg Settings")]
    [SerializeField][Tooltip("Max health per leg")] private float legHealth = 100;
    [SerializeField][Tooltip("Time in seconds before regrow starts")] private float legRegrowDelay = 5;
    [SerializeField][Tooltip("Time in seconds of how long it takes to regrow")] private float legRegrowTime = 1;
    [Header("Arm Settings")]
    [SerializeField][Tooltip("Max health per arm")] private float armHealth = 100;
    [SerializeField][Tooltip("Time in seconds before regrow starts")] private float armRegrowDelay = 5;
    [SerializeField][Tooltip("Time in seconds of how long it takes to regrow")] private float armRegrowTime = 1;
    [Header("Head Settings")]
    [SerializeField] private float headHealth = 100;
    [SerializeField][Tooltip("Time in seconds before regrow starts")] private float headRegrowDelay = 5;
    [SerializeField][Tooltip("Time in seconds of how long it takes to regrow")] private float headRegrowTime = 1;

    

    void Awake()
    {
        Debug.Assert(legs != null, "ZombieBoby - BodyPart legs missing please assign in inspector");
        Debug.Assert(arms != null, "ZombieBoby - BodyPart arms missing please assign in inspector");
        Debug.Assert(head != null, "ZombieBoby - BodyPart head missing please assign in inspector");
        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].OnDetach += OnLegDetach;
            legs[i].OnRegrown += OnLegRegrown;
        }
        for (int i = 0; i < arms.Length; i++)
        {
            arms[i].OnDetach += OnArmDetach;
        }
        head.OnDetach += OnHeadDetach;
        //if (tail) tail.OnDetach += OnTailDetach;

        //Hitbox[] hbs = GetComponentsInChildren<Hitbox>();
        //foreach (var hb in hbs)
        //{
        //    hb.OnHitRb += Hb_OnHitRb;
        //}
    }

    private void OnLegRegrown()
    {
        if (HasLegs() && HasArms())
        {
            //we have arms and legs
            GetComponent<RagdollToAnimation>().BeginAnimating(() => { Debug.Log("Resseting completed"); });
            GetComponent<RagdollController>().DisableRagdoll();
        }
    }

    //private void Hb_OnHitRb(Vector3 point, Vector3 dir, float damage, Rigidbody rb)
    //{
    //    rb.AddForceAtPosition(dir*100, point, ForceMode.Impulse);
    //}

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
        if (legs != null)
        {
            for (int i = 0; i < legs.Length; i++)
            {

                if (legs[i])
                {
                    legs[i].MaxHealth = legHealth;
                    legs[i].RegrowTime = legRegrowTime;
                    legs[i].TimeBeforeRegrow = legRegrowDelay;
                }
            }
        }
        if (arms != null)
        {

            for (int i = 0; i < arms.Length; i++)
            {
                if (arms[i])
                {
                    arms[i].MaxHealth = armHealth;
                    arms[i].RegrowTime = armRegrowTime;
                    arms[i].TimeBeforeRegrow = armRegrowDelay;
                }
            }
        }
        if (head)
        {
            head.MaxHealth = headHealth;
            head.RegrowTime = headRegrowTime;
            head.TimeBeforeRegrow = headRegrowDelay;
        }

        if (TryGetComponent(out Health health))
        {
            health.MaxHealth = totalHealth;
            health.SetHealth(totalHealth);
        }
    }
}
