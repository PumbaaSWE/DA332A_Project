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

    public void Interact(Transform interactor)
    {
        Firearm firearm = interactor.GetComponentInChildren<Firearm>();
        if (firearm)
        {
            if(firearm.ReserveAmmo == firearm.MaxReserveAmmo)
            {
                return;
            }
            firearm.ReserveAmmo = Math.Min(firearm.ReserveAmmo + 100, firearm.MaxReserveAmmo);
            Destroy(gameObject);
        }
    }

    public void SpeculateInteract(Transform interactor)
    {
        
        Firearm firearm = interactor.GetComponentInChildren<Firearm>();
        if (firearm)
        {
            if (firearm.ReserveAmmo == firearm.MaxReserveAmmo)
            {
                //TooltipUtil.Display("Ammo full", 2);
                Tooltip = "Ammo full";
                return;
            }
            Tooltip = "Press {0} to pickup 100 ammo";
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
