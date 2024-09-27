using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Objective : MonoBehaviour
{
    [SerializeField] KeyItemSO[] requiredItems;
    

    public bool CheckRequiredItems(KeyItem[] items)
    {
        return requiredItems.All(requiredItem => items.Any(item => item.M_SO == requiredItem));
    }
}
