using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBox : MonoBehaviour
{
    [SerializeField] string tooltip = "Press {0} to heal!";
    [SerializeField] string tooltipFull = "Health is full!";
    [SerializeField] float healAmount = 25;

    public void PickUp(Transform obj)
    {
        if (obj.TryGetComponent(out Health health))
        {
            if (health.Value >= health.MaxHealth)
                return;

            health.Heal(healAmount);
            Destroy(gameObject);
        }
    }

    public void Speculate(Transform obj)
    {
        if (obj.TryGetComponent(out Health health))
        {
            if (health.Value >= health.MaxHealth)
                GetComponent<InteractableButton>().tooltip = tooltipFull;
            else
                GetComponent<InteractableButton>().tooltip = tooltip;
        }
    }
}
