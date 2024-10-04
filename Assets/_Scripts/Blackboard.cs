using System;
using System.Collections.Generic;


public class Blackboard : PersistentSingleton<Blackboard>
{
    Dictionary<string, object> dictionary;
    private readonly Dictionary<string, Action<object>> observers = new();

    protected override void Awake()
    {
        base.Awake();

        dictionary = new Dictionary<string, object>();
    }

    public void Set(string input, object obj)
    {
        if (dictionary.ContainsKey(input))
        {
            object current = dictionary[input];
            dictionary[input] = obj;
            if (!current.Equals(obj)) //its a new value...
            {
                NotifyObservers(input, obj);
            }
        }
        else
        {
            dictionary.Add(input, obj);
            NotifyObservers(input, obj);
        }
    }

    public T Get<T>(string input)
    {
        if (!dictionary.ContainsKey(input))
            return default;

        return (T)dictionary[input];
    }

    public static bool GetBool(string key)
    {
        return Instance.Get<bool>(key);
    }

    public static void SetBool(string key, bool value)
    {
        Instance.Set(key, value);
    }


    public static void ObserveKey(string key, Action<object> callback)
    {
        Instance.ObserveKeyInternal(key, callback);
    }

    public static void StopObservingKey(string key, Action<object> callback)
    {
        Instance.StopObservingKeyInternal(key, callback);
    }

    private void ObserveKeyInternal(string key, Action<object> callback)
    {

        if (observers.ContainsKey(key))
            observers[key] += callback;
        else
            observers.Add(key, callback);
    }

    private void StopObservingKeyInternal(string key, Action<object> callback)
    {

        if (observers.ContainsKey(key))
            observers[key] -= callback;
    }

    private void NotifyObservers(string key, object value)
    {
        if (observers.ContainsKey(key))
            observers[key].Invoke(value);
    }
}
