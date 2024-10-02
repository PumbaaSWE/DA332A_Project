using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmPickUp : MonoBehaviour, IInteractable
{
    public string Tooltip { get; }
    public string InteractedTooltip { get; }
    public bool CanInteract { get; }
    public bool ShowInteractMessage { get; }
    public int InteractedDisplayPriority { get; }
    public float InteractedTipDisplayTime { get; }

    [SerializeField] GameObject GunPrefab;

    public void Interact(Transform interactor)
    {
        if (interactor.GetComponent<WeaponHandler>().PickupGun(GunPrefab))
            Destroy(gameObject);
    }

    public void SpeculateInteract(Transform interactor)
    {

    }
}
