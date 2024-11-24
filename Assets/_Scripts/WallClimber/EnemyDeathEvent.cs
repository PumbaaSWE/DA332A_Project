using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyDeathEvent : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        GetComponent<Health>().OnDeath += SendEvent;
    }

    private void OnDisable()
    {
        GetComponent<Health>().OnDeath -= SendEvent;
    }

    private void SendEvent(Health obj)
    {
        EventBus<DeathEvent>.Raise(new DeathEvent(transform.position));
    }
}

public struct DeathEvent : IEvent
{
    public Vector3 position;
    public DeathEvent(Vector3 position)
    {
        this.position = position;
    }
}
