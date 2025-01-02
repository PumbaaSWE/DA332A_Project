using UnityEngine;

[DisallowMultipleComponent]
//[RequireComponent(typeof(Health))]
public class EnemyDeathEvent : MonoBehaviour
{

    private void OnEnable()
    {
        if (TryGetComponent(out Health health)) health.OnDeath += SendEvent;
        if (TryGetComponent(out EnemyHealth enemyHealth)) enemyHealth.OnDeath += SendEvent2;
    }

    

    private void OnDisable()
    {
        if (TryGetComponent(out Health health)) health.OnDeath -= SendEvent;
        if (TryGetComponent(out EnemyHealth enemyHealth)) enemyHealth.OnDeath -= SendEvent2;
    }

    private void SendEvent2()
    {
        EventBus<DeathEvent>.Raise(new DeathEvent(transform.position));
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
