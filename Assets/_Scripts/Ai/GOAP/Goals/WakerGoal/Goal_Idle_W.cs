using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Idle_W : Goal_Base
{
    public int Priority = 10;
    public override int CalculatePriority()
    {
        return Priority;
    }
    public override bool CanRun()
    {
        return true;
    }
}
