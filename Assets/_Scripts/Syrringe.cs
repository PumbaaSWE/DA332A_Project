using UnityEngine;

public class Syrringe : MonoBehaviour, IInteractable
{


    public string TooltipOnCan = "Press {0} to pick up syringe";
    public string TooltipOnFull = "You max amount of syringes";


    public string Tooltip { get; set; }

    public string InteractedTooltip { get; set; }

    public bool CanInteract => true;

    public bool ShowInteractMessage => true;

    public int InteractedDisplayPriority => 1;

    public float InteractedTipDisplayTime => 1;

    public void Interact(Transform interactor)
    {
        PickUp(interactor);
    }

    public void PickUp(Transform obj)
    {
        if (obj.TryGetComponent(out SyrringeUser syrringeUser))
        {
            if (syrringeUser.NumSyrringes >= syrringeUser.MaxSyrringes)
                return;

            syrringeUser.NumSyrringes++;
            Destroy(gameObject);
        }
    }

    public void SpeculateInteract(Transform interactor)
    {
        if (interactor.TryGetComponent(out SyrringeUser syrringeUser))
        {
            if (syrringeUser.NumSyrringes >= syrringeUser.MaxSyrringes)
            {
                Tooltip = TooltipOnFull;
            }
            else
            {
                Tooltip = TooltipOnCan;
            }

            
        }
    }
}
