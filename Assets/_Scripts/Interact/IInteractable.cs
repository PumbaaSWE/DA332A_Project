using UnityEngine;

public interface IInteractable
{
    public string Tooltip { get; }
    public bool CanInteract { get; }

    public void Interact(Transform interactor);
    public void SpeculateInteract(Transform interactor);
}