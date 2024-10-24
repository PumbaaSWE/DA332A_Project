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

    [SerializeField] Firearm GunPrefab;
    public string Name;
    public int LoadedAmmo;

    public void Interact(Transform interactor)
    {
        if (interactor.GetComponent<WeaponHandler>().PickupGun(GunPrefab, LoadedAmmo))
        {
            Destroy(gameObject);
        }
        else if(LoadedAmmo > 0)
        {
            LoadedAmmo = 0;
        }
        //lol cache it
        GetComponent<Rigidbody>().AddForce(Vector3.up, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddTorque(transform.right*.5f, ForceMode.Impulse);
    }

    public void SpeculateInteract(Transform interactor)
    {
        if (interactor.TryGetComponent(out WeaponHandler weaponHandler))
        {
            
            if (weaponHandler.HasGun(GunPrefab.Id))
            {
                if (LoadedAmmo == 0)
                {
                    Tooltip = "You have this already (this is empty)";
                }
                else
                {
                    Tooltip = "{0} to pickup " + GunPrefab.AmmoType + " ammo";
                }
            }
            else if(weaponHandler.Guns.Count == weaponHandler.MaxGuns)
            {
                Tooltip = "{0} to replace " + weaponHandler.EquippedGun.Name + " with " + GunPrefab.Name;
            }
            else
            {
                Tooltip = "{0} to pickup " + GunPrefab.Name;
            }
        }
    }
}
