using UnityEngine;

public interface IInteractable
{
    public string Tooltip { get; }
    public string InteractedTooltip { get; }
    public bool CanInteract { get; }
    public bool ShowInteractMessage { get; }

    public void Interact(Transform interactor);
    public void SpeculateInteract(Transform interactor);
}