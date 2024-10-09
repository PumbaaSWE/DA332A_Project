using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Action<string> OnCanInteract;
    public Action<string, float> OnCanInteractTimed;
    public Action<string, float> OnInteracted;
    public Action<string, float, int> OnInteractedPriority;

    [Tooltip("Will auto reference child with name LookDir")][SerializeField] private Transform lookDir;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;

    [SerializeField] PlayerInput playerInput;
    InputAction action;
    string key = "[error]";

    IInteractable interactable;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(lookDir, "Interactor - lookDir not assigned!");

        action = playerInput.actions.FindAction("Interact");
        //playerInput.actions.
        //InputBinding binding 
        key = "[" + action.bindings.First().ToDisplayString() + "]";
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(lookDir.position, lookDir.forward, out RaycastHit hit, range, layerMask))
        {
            IInteractable item = hit.transform.GetComponentInParent<IInteractable>();
            //Debug.Log("Hit:" + hit.transform.gameObject.name);
            
            if (item != null)
            {
                if(interactable != item)
                {
                    interactable = item;
                    item.SpeculateInteract(transform);
                }
                OnCanInteractTimed?.Invoke(string.Format(item.Tooltip, key), 0.0f); // when hovering over an interactable object tooltip is displayed for 0 seconds (one frame i think)
                //OnCanInteract?.Invoke(string.Format(item.Tooltip, key)); // this Action works as well, time is automatically set to 0

                if (action.triggered)
                {
                    item.Interact(transform);
                    if (item.ShowInteractMessage)
                    {
                        OnInteractedPriority?.Invoke(item.InteractedTooltip, item.InteractedTipDisplayTime, item.InteractedDisplayPriority);
                    }
                }
            }
        }
        else
        {
            interactable = null;
        }
    }
}
