using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour, IInteractable
{
    public string tooltip = "Press {0} button!";
    public string interactedTooltip = "You pressed that button";

    public string Tooltip => tooltip;
    public string InteractedTooltip => interactedTooltip;

    public bool CanInteract => true;
    public bool ShowInteractMessage { get { return interactedTooltip != string.Empty; } }

    public UnityEvent<Transform> OnPress;
    public void Interact(Transform interactor)
    {
        OnPress?.Invoke(interactor);
    }

    public void SpeculateInteract(Transform interactor)
    {
        //noop
    }
}
