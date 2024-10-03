using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCardRequrement : InteractRequrement
{
    [SerializeField] private KeyItemSO keyCard;
    
    public override bool Check(Transform interactor)
    {
        if(interactor.TryGetComponent(out HiddenInventory inventory)){
            return inventory.HasKeyCard(keyCard);
        }
        return false;
    }
}
