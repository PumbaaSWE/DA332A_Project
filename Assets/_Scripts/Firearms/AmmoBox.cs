using System;
using System.Collections;
using UnityEngine;

public class AmmoBox : MonoBehaviour, IInteractable
{
    public string Tooltip { get; private set; }
    public string InteractedTooltip { get; private set; }
    public int displayPriority { get; private set; }
    public float tipDisplayTime { get; private set; }
    public bool CanInteract => true;
    public bool ShowInteractMessage { get { return InteractedTooltip != string.Empty; } }
    public int InteractedDisplayPriority => displayPriority;
    public float InteractedTipDisplayTime => tipDisplayTime;

    public Cartridgetype AmmoType;
    public int Ammo;
    [SerializeField] bool UnlimitedUse;
    [SerializeField] float BoxCooldown;
    bool CanUse = true;

    public void Interact(Transform interactor)
    {
        if (CanUse)
        {
            interactor.GetComponent<WeaponHandler>().AddAmmo(AmmoType, Ammo);
            CanUse = false;

            if (UnlimitedUse)
                StartCoroutine(Cooldown());

            else
                Destroy(gameObject);
        }
    }

    public void SpeculateInteract(Transform interactor)
    {
        switch (AmmoType)
        {
            case Cartridgetype.Rifle:
                Tooltip = "Pickup Rifle Bullets";
                break;
            case Cartridgetype.Pistol:
                Tooltip = "Pickup Pistol Bullets";
                break;
            case Cartridgetype.ShotgunShell:
                Tooltip = "Pickup Shotgun Shells";
                break;
        }

        //Firearm firearm = interactor.GetComponentInChildren<Firearm>();
        //if (firearm)
        //{
        //    if (firearm.ReserveAmmo == firearm.MaxReserveAmmo)
        //    {
        //        //TooltipUtil.Display("Ammo full", 2);
        //        Tooltip = "Ammo full";
        //        return;
        //    }
        //    Tooltip = "Press {0} to pickup 100 ammo";
        //}

    }

    IEnumerator Cooldown()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Rigidbody>().useGravity = false;

        yield return new WaitForSeconds(BoxCooldown);

        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        CanUse = true;
    }
}
