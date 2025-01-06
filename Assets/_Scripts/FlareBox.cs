using UnityEngine;

public class FlareBox : MonoBehaviour, IInteractable
{

    public string TooltipOnCan = "Press {0} to pick up flares";
    public string TooltipOnFull = "You max amount of flares";


    public string Tooltip { get; set; }

    public string InteractedTooltip { get; set; }

    public bool CanInteract => true;

    public bool ShowInteractMessage => true;

    public int InteractedDisplayPriority => 1;

    public float InteractedTipDisplayTime => 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    public void PickUp(Transform obj)
    {
        if (obj.TryGetComponent(out FlareThrower flareThrower)) {
            if (flareThrower.NumFlares >= flareThrower.MaxNumFlares)
            {
                InteractedTooltip = "You have to many flares";
                return;
            }
            flareThrower.NumFlares += 5;
            Destroy(gameObject);
            InteractedTooltip = string.Empty;
        }
    }

    public void Interact(Transform interactor)
    {
        PickUp(interactor);
    }

    public void SpeculateInteract(Transform interactor)
    {
        if (interactor.TryGetComponent(out FlareThrower flareThrower))
        {
            if(flareThrower.NumFlares >= flareThrower.MaxNumFlares)
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
