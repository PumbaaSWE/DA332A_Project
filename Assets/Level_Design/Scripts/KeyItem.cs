using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [SerializeField] KeyItemSO m_SO;

    public KeyItemSO M_SO
    {
        get { return m_SO; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HiddenInventory>().AddItem(this);
            this.gameObject.SetActive(false);
        }
    }
}
