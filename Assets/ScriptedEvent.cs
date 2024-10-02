using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScriptedEvent : MonoBehaviour
{
    [SerializeField] UnityEvent OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.TryGetComponent(out Player _))
            return;

        OnTrigger.Invoke();
        enabled = false;
    }
}
