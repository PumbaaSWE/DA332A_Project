using System;
using UnityEngine;

public class DeathEventListener : MonoBehaviour
{
    EventBinding<DeathEvent> eventBinding;
    [SerializeField] private bool listening;
    public Action<DeathEvent> OnDeathEvent;
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
    }

    private void OnDisable()
    {
        //EventBus<DeathEvent>.Deregister(eventBinding);
        DoListen = false;
    }

    private void InternalOnDeathEvent(DeathEvent deathEvent)
    {
        //Vector3 deathPos = deathEvent.position;
        //Debug.Log("Enemy died at: " + deathPos);
        OnDeathEvent?.Invoke(deathEvent);
    }
}
