using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ZombieSensor : MonoBehaviour, IDamageble
{

    [SerializeField] PlayerDataSO playerData;
    [SerializeField] float visionRange = 10;
    //[SerializeField] float looseSightTime = 1;
    //[SerializeField] float visionRangeFar = 20;
    [SerializeField] LayerMask visionMask;
    Transform player;
    float timer;
    Hitbox[] hitboxes;
    public event Action<Transform, Vector3, Vector3, float> OnHit;

    public Vector3 TargetPos { get; private set; }
    public Vector3 TargetDir { get; private set; }
    public bool SeeTarget { get; private set; }
    public float TargetDist { get; private set; }
    public float TargetLastSeenTime => timer;

    public Health Health { get; private set; }

    private void Awake()
    {
        hitboxes = GetComponentsInChildren<Hitbox>();
    }

    void OnEnable()
    {
        TargetPos = transform.position;
        TargetDir = transform.forward;
        SeeTarget = false;
        TargetDist = visionRange + 1;
        playerData.NotifyOnPlayerChanged(OnPlayerChanged);

        foreach (var hitbox in hitboxes)
        {
            hitbox.OnHit += TakeDamage;
        }
    }


    private void OnDisable()
    {
        playerData.UnsubscribeOnPlayerChanged(OnPlayerChanged);
        foreach (var hitbox in hitboxes)
        {
            hitbox.OnHit -= TakeDamage;
        }
    }

    void OnPlayerChanged(Transform player)
    {
        this.player = player;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player) return;
        float dt = Time.deltaTime;

        Vector3 eyePos = transform.position + new Vector3(0, 1.6f, 0);
        Vector3 playerHeadPos = player.GetChild(0).position + Vector3.up * 0.1f;
        Vector3 delta = playerHeadPos - eyePos;

        //Vector3 delta = player.position - transform.position;
        SeeTarget = false;

        //always see if close 360 sens
        if (delta.sqrMagnitude < 4)
        {
            SeeTarget = true;
            TargetDir = delta.normalized;
            TargetPos = player.position;
            TargetDist = delta.magnitude;
            timer = 0;
            return;
        }

        //semiclose 180 degree fov
        if (delta.sqrMagnitude < visionRange * visionRange && Vector3.Dot(delta, transform.forward) > 0)
        {
            if(HasDirectLineOfSight(eyePos, delta, visionRange))
            {
                return;
            }
            else
            {
                timer += dt;
            }
            Debug.DrawLine(eyePos, playerHeadPos, Color.magenta);
        }
        else
        {

            Debug.DrawLine(eyePos, playerHeadPos, Color.cyan);
        }
        //if(timer > looseSightTime)
        //{
        //    SeeTarget = false;
        //}
        //else
        //{
        //    SeeTarget = true;
        //    TargetDir = delta.normalized;
        //    TargetPos = player.position;
        //    TargetDist = delta.magnitude;
        //}

        //float angle = Vector3.Angle(delta, transform.forward);

        ////Vector3 direction = delta.normalized;
        ////float cosAngle = Vector3.Dot(transform.forward, direction);
        //SeeTarget = false;
        //if (angle < 45)
        //{
        //    DirectLineOfSight(delta, visionRangeFar);
        //}

    }


    bool HasDirectLineOfSight(Vector3 origin, Vector3 delta, float range)
    {
        if (Physics.Raycast(origin, delta, out RaycastHit hit, range, visionMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform == player)
            {
                //Debug.Log("We see player");
                TargetPos = player.position;
                TargetDir = delta.normalized;
                TargetDist = delta.magnitude;
                SeeTarget = true;
                timer = 0;
                return true;
            }
        }
        return false;
    }

    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        OnHit?.Invoke(player, point, direction, damage);
        if(SeeTarget) return;
        Vector3 delta = player.position - transform.position;
        TargetPos = player.position;
        TargetDir = delta.normalized;
        TargetDist = delta.magnitude;
    }
}
