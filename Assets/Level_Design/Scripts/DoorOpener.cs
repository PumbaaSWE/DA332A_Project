using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : Objective
{
    [SerializeField] Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && CheckRequiredItems(other.GetComponent<HiddenInventory>().ReturnItemsAsArray()))
        {
            anim.SetTrigger("Open");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && CheckRequiredItems(other.GetComponent<HiddenInventory>().ReturnItemsAsArray()))
        {
            anim.SetTrigger("Exit");
        }
    }
}
