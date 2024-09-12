using UnityEngine;
using UnityEngine.Events;

public abstract class EventChannelListener<T> : MonoBehaviour
{
    [SerializeField] protected EventChannel<T> channel;
    [SerializeField] protected UnityEvent<T> unityEvent;

    protected virtual void OnEnable()
    {
        channel.OnChannelEvent += OnChannelEvent;
    }

    protected virtual void OnDisable()
    {
        channel.OnChannelEvent -= OnChannelEvent;
    }

    protected virtual void OnChannelEvent(T ctx)
    {
        unityEvent?.Invoke(ctx);
    }
}
