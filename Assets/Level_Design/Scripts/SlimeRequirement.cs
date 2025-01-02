using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeRequirement : InteractRequrement
{
    [SerializeField] private SlimePlane splane;

    public override bool Check(Transform interactor)
    {
        if (splane.slimeCounter >= splane.slimeDone)
        {
            return true;
        }
        return false;
    }
}
