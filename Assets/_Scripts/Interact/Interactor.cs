using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public Action<string, float> OnCanInteract;
    public Action<string, float> OnInteracted;

    [Tooltip("Will auto reference child with name LookDir")][SerializeField] private Transform lookDir;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;

    [SerializeField] PlayerInput playerInput;
    InputAction action;
    string key = "[error]";

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
                //OnCanInteract?.Invoke("Press [E] to interact with " + item.Name, 0);
                OnCanInteract?.Invoke(string.Format(item.Tooltip, key), 0);
                //Debug.Log("Item != null");
                if (action.triggered)
                {
                    OnInteracted?.Invoke(item.Tooltip, 0);
                    item.Interact(transform);
                }
            }
        }
    }
}
