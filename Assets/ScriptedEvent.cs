using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptedEvent : MonoBehaviour
{
    [SerializeField] UnityEvent OnTrigger;
    [SerializeField] bool triggerOnce;

    bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && triggered)
            return;

        if (!other.transform.TryGetComponent(out Player _))
            return;

        OnTrigger.Invoke();
        triggered = true;
    }
}
