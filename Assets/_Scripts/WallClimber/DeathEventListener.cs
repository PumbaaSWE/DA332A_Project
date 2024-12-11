using System;
using UnityEngine;
using UnityEngine.Events;


/**
 * Fires of unity events when an enemy dies
 * You can also subscribe directly to OnDeathEvent for every enemy location regardless of inside trigger
 * If no trigger exists it will always be DeathOutside that triggers
 * 
 * If this component is disabled or DoListen = false no events will fire upon enemydeaths at all, 
 * usefull if you dont care about deaths at this moment or stop caring at some point
 * 
 * Multiple DeathEventListener can exist in a scene and will all know when enemies dies and can be enable/disabled separatly 
 * They can use different zones and be for separate purposes, whatever you need.
 * 
 *  -Jack
 * 
 */ 
public class DeathEventListener : MonoBehaviour
{
    EventBinding<DeathEvent> eventBinding;
    private bool listening;
    public Action<DeathEvent> OnDeathEvent;

    [Tooltip("Event will fire when an enemy dies within a box collider and return the name of the object with that collider attached")]
    [SerializeField] BoxCollider[] inside;
    //[SerializeField] BoxCollider[] outside;

    public UnityEvent<string> DeathInside;
    public UnityEvent DeathOutside;

    public bool DoListen
    {
        get { return listening; }
        set { 
            if(listening != value)
            {
                listening = value;
                if (listening)
                {
                    EventBus<DeathEvent>.Register(eventBinding);
                }
                else
                {
                    EventBus<DeathEvent>.Deregister(eventBinding);
                }
            }
            
        }
    }
    public void OnEnable()
    {
        eventBinding ??= new(InternalOnDeathEvent);
        //EventBus<DeathEvent>.Register(eventBinding);
        DoListen = true;

        DeathInside.AddListener((s) => Debug.Log("Death inside: " + s));
    }

    private void OnDisable()
    {
        //EventBus<DeathEvent>.Deregister(eventBinding);
        DoListen = false;
    }

    private void InternalOnDeathEvent(DeathEvent deathEvent)
    {
        Vector3 deathPos = deathEvent.position;
        //Debug.Log("Enemy died at: " + deathPos);
        OnDeathEvent?.Invoke(deathEvent);

        if (CheckInside(deathPos, inside, out string name))
        {
            DeathInside?.Invoke(name);
        }
        else
        {
            DeathOutside?.Invoke();
        } 

    }

    private bool CheckInside(Vector3 point, BoxCollider[] inside, out string name)
    {
        name = string.Empty;
        if(inside == null) return false;
        for (int i = 0; i < inside.Length; i++)
        {
            if (point == inside[i].ClosestPoint(point))
            {
                name = inside[i].name;
                return true;
            }
        }
        return false;
    }
}
