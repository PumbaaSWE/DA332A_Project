using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOfThis : MonoBehaviour
{
    public GameObject objectToDisable;

    public void DisableObject()
    {
        objectToDisable.SetActive(false);
    }
}
