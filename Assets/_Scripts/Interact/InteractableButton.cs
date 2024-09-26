using UnityEngine;
using UnityEngine.Events;

public class InteractableButton : MonoBehaviour, IInteractable
{
    public string tooltip = "Press {0} button!";
    
    public string Tooltip => tooltip;

    public bool CanInteract => true;

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
