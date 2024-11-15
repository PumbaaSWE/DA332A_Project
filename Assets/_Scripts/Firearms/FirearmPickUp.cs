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
   // public string Name;
    public int LoadedAmmo;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.AddForce(Vector3.up, ForceMode.Impulse);
            rb.AddTorque(Vector3.right * .125f, ForceMode.Impulse);
        }
    }

    public void Interact(Transform interactor)
    {
        if (interactor.TryGetComponent(out WeaponHandler weaponHandler))
        {
            if (weaponHandler.HasGun(GunPrefab.Id))
            {
                int left = weaponHandler.AmmunitionPool.AddAmmo(GunPrefab.AmmoType, LoadedAmmo);
                LoadedAmmo = left;
            }
            else
            {
                weaponHandler.PickupGun(GunPrefab, LoadedAmmo);
                Destroy(gameObject);
            }
        }
        // Who added this? Don't remember adding this - Alex
        //lol
        //if (rb)
        //{
        //    rb.AddForce(Vector3.up, ForceMode.Impulse);
        //    rb.AddTorque(interactor.right * .125f, ForceMode.Impulse);
        //}
        
    }

    public void SpeculateInteract(Transform interactor)
    {
        if (interactor.TryGetComponent(out WeaponHandler weaponHandler))
        {
            
            if (weaponHandler.HasGun(GunPrefab.Id))
            {
                if (LoadedAmmo == 0)
                {
                    Tooltip = $"You have {GunPrefab.Name} already (this is empty)";
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
