using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivateEnemy : MonoBehaviour
{
    
    public UnityEvent OnInteract;

    public void Spawners()
    {

        if (OnInteract != null)
        {
            OnInteract.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Spawners();
        }
        this.gameObject.SetActive(false);
    }
}
