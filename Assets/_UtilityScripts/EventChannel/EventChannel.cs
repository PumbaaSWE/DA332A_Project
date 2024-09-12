using UnityEngine;

public abstract class EventChannel<T> : ScriptableObject
{
    public delegate void OnEvent(T ctx);
    public event OnEvent OnChannelEvent;

    public void Raise(T ctx)
    {
        OnChannelEvent?.Invoke(ctx);
    }
}
