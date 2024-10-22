using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmPickUp : MonoBehaviour, IInteractable
{
    public string Tooltip { get; private set; }
    public string InteractedTooltip { get; }
    public bool CanInteract { get; private set; }
    public bool ShowInteractMessage { get; private set; }
    public int InteractedDisplayPriority { get; private set; }
    public float InteractedTipDisplayTime { get; private set; }

    [SerializeField] GameObject GunPrefab;
    public string Name;
    public int LoadedAmmo;

    public void Interact(Transform interactor)
    {
        if (interactor.GetComponent<WeaponHandler>().PickupGun(GunPrefab, LoadedAmmo))
            Destroy(gameObject);
    }

    public void SpeculateInteract(Transform interactor)
    {
        Tooltip = "Pickup " + Name;
    }
}
