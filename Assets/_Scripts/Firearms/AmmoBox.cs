using System;
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

    public void Interact(Transform interactor)
    {
        interactor.GetComponent<WeaponHandler>().AddAmmo(AmmoType, Ammo);
        Destroy(gameObject);
    }

    public void SpeculateInteract(Transform interactor)
    {
        switch(AmmoType)
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
}