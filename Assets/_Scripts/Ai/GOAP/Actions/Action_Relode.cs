using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Action_Relode : Action_Base
{
    //[SerializeField] float bullets = 10f;

    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_RangedAttack) });
    Goal_RangedAttack attackGoal;
    float cost;
    public override List<System.Type> GetSupportedGoals()
    {
        return SupportedGoals;
    }

    public override float GetCost()
    {
        return 6; //6
    }

    //public override void OnDeactivated()
    //{
    //    // if to close it should melee otherwise it would be good if it finish reloding
    //    rangedWeapon.HandleWeaponAI(false, false, false, attackGoal.AttackTarget);
    //}
    public override void OnActivated(Goal_Base linkedGoal)
    {
        base.OnActivated(linkedGoal);
        attackGoal = (Goal_RangedAttack)LinkedGoal;

        // can not be negative or 0 and not bigger the 6
        // cost = Math.Max(1, Math.Min(6, 3 + (attackGoal.distance - 15) / 4));

    }

    public override void OnTick()
    {
        //rangedWeapon.HandleWeaponAI(false, true, false, attackGoal.AttackTarget); // Reload
    }
}
