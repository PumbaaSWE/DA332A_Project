using System.Collections.Generic;
using System.Diagnostics;

public static class EventBus<T> where T : IEvent
{
    static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();

    public static void Register(EventBinding<T> binding)
    {
        Debug.Assert(binding != null, "Do not register null object as a binding!");
        bindings.Add(binding);
    }
    public static void Deregister(EventBinding<T> binding) => bindings.Remove(binding);

    public static void Raise(T @event) //event is a keyword but you can use it if you put @ infront...
    {
        foreach (IEventBinding<T> binding in bindings)
        {
            binding.OnEvent.Invoke(@event);
            binding.OnEventNoArgs.Invoke();
        }
    }

    static void Clear()
    {
        //Debug.Log($"Clearing {typeof(T).Name} bindings");
        bindings.Clear();
    }
}