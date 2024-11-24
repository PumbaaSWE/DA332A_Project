using UnityEngine;

public class DeathEventListener : MonoBehaviour
{
    EventBinding<DeathEvent> eventBinding;
    public void OnEnable()
    {
        eventBinding ??= new(OnDeathEvent);
        EventBus<DeathEvent>.Register(eventBinding);
    }

    private void OnDisable()
    {
        EventBus<DeathEvent>.Deregister(eventBinding);
    }

    private void OnDeathEvent(DeathEvent deathEvent)
    {
        //Vector3 deathPos = deathEvent.position;
        //Debug.Log("Enemy died at: " + deathPos);
    }
}
