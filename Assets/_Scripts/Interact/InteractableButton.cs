using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour, IInteractable
{
    public string tooltip = "Press {0} button!";
    public string interactedTooltip = string.Empty;
    public int displayPriority = 1;
    public float tipDisplayTime = 3.0f;

    public string Tooltip => tooltip;
    public string InteractedTooltip => interactedTooltip;

    public bool CanInteract => true;
    public bool ShowInteractMessage { get { return interactedTooltip != string.Empty; } }

    public int InteractedDisplayPriority => displayPriority;
    public float InteractedTipDisplayTime => tipDisplayTime;

    public UnityEvent<Transform> OnPress;
    public UnityEvent<Transform> OnSpeculate;
    public void Interact(Transform interactor)
    {
        OnPress?.Invoke(interactor);
    }

    public void SpeculateInteract(Transform interactor)
    {
        OnSpeculate?.Invoke(interactor);
    }
}
