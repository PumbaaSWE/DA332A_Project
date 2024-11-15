using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syrringe : MonoBehaviour
{
    public void PickUp(Transform obj)
    {
        if (obj.TryGetComponent(out SyrringeUser syrringeUser))
        {
            if (syrringeUser.NumSyrringes >= syrringeUser.MaxSyrringes)
                return;

            syrringeUser.NumSyrringes++;
            Destroy(gameObject);
        }
    }
}
