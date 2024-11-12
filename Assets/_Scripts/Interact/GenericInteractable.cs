using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] public string tooltip;
    [SerializeField] public string interactedTooltip;
    [SerializeField] public bool canInteract;
    [SerializeField] public bool showInteractedMessage;
    [SerializeField] public int interactedDisplayPriority;
    [SerializeField] public int interactedTipDisplayTime;

    public string Tooltip => tooltip;

    public string InteractedTooltip => interactedTooltip;

    public bool CanInteract => canInteract;

    public bool ShowInteractMessage => showInteractedMessage;

    public int InteractedDisplayPriority => interactedDisplayPriority;

    public float InteractedTipDisplayTime => interactedTipDisplayTime;

    public UnityEvent interact;
    public UnityEvent speculateInteract;

    public void Interact(Transform interactor)
    {
        interact?.Invoke();
    }

    public void SpeculateInteract(Transform interactor)
    {
        speculateInteract?.Invoke();
    }
}
