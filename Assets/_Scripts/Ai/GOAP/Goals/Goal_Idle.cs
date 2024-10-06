using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Idle : Goal_Base
{
    public int Priority = 10;
   

    public override int CalculatePriority()
    {
        if(climber)
        {
            if (ragdoll.active)
            {
                Priority = 10;
            }
            //else
            //{
            //    Priority = 100;
            //}
        }
       
        return Priority;
    }
    public override bool CanRun()
    {
        return true;
    }
}
