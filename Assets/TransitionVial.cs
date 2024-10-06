using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionVial : MonoBehaviour, IInteractable
{
    public string Tooltip => "Press {0} to drink vial.";

    public string InteractedTooltip { get; private set; }

    public bool CanInteract => true;

    public bool ShowInteractMessage { get; private set; }

    public int InteractedDisplayPriority { get; private set; }

    public float InteractedTipDisplayTime { get; private set; }

    public void Interact(Transform interactor)
    {
        interactor.GetComponent<SwitchController>().Transition();
        Destroy(gameObject);
    }

    public void SpeculateInteract(Transform interactor)
    {

    }
}
