using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanRequirement : InteractRequrement
{
    public override bool Check(Transform interactor)
    {
        if (interactor.TryGetComponent(out WeaponHandler handler))
        {
            return handler.enabled;
        }
        return false;
    }
}
