using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenInventory : MonoBehaviour
{
    [SerializeField] List<KeyItem> items = new List<KeyItem>();


    public void AddItem(KeyItem keyItem)
    {
        items.Add(keyItem);
    }

    public KeyItem[] ReturnItemsAsArray()
    {
        return items.ToArray();
    }

}
