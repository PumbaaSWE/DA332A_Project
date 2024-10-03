using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : PersistentSingleton<Blackboard>
{
    Dictionary<string, object> dictionary;

    protected override void Awake()
    {
        base.Awake();

        dictionary = new Dictionary<string, object>();
    }

    public void Set(string input, object obj)
    {
        if (dictionary.ContainsKey(input))
            dictionary[input] = obj;
        else
            dictionary.Add(input, obj);
    }

    public T Get<T>(string input)
    {
        if (!dictionary.ContainsKey(input))
            return default;

        return (T)dictionary[input];
    }
}
