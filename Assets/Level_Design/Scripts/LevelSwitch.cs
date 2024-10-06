using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwitch : Objective
{

    [SerializeField] Animator anim;
    public HiddenInventory inventoryCopy;
    private InteractableButton interactableButton;
    public static event Action OnAction;

    private void Start()
    {
        anim = GetComponent<Animator>();
        inventoryCopy = GameObject.FindGameObjectWithTag("Player").GetComponent<HiddenInventory>();
        interactableButton = GetComponent<InteractableButton>();
        if(inventoryCopy == null )
        {
            Debug.LogWarning("HIDDEN INVENTORY IS NULL");
        }
    }
    public void Animate()
    {
       
        if (CheckRequiredItems(inventoryCopy.ReturnItemsAsArray()))
        {
            interactableButton.interactedTooltip = "fix Hud later: you got all keys. it works";
            anim.enabled = true;
            OnAction?.Invoke();
            
        }
        else
        {
            interactableButton.interactedTooltip = "fix Hud later: you need more keys";
        }
    }




}
