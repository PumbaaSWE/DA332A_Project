using System.Collections.Generic;
using UnityEngine;

public class HiddenInventory : MonoBehaviour
{
    [SerializeField] List<KeyItem> items = new List<KeyItem>();
    [SerializeField] HashSet<KeyItemSO> keyCards = new();


    public void AddItem(KeyItem keyItem)
    {
        items.Add(keyItem);
        keyCards.Add(keyItem.M_SO);
    }

    public KeyItem[] ReturnItemsAsArray()
    {
        return items.ToArray();
    }


    public bool HasKeyCard(KeyItemSO key)
    {
        return keyCards.Contains(key);
    }
}
