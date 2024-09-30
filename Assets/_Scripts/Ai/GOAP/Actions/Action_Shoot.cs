using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Action_Shoot : Action_Base
{
    //[SerializeField] float bullets = 10f;
    //[SerializeField] float SearchCoverRange = 10f;
    List<System.Type> SupportedGoals = new List<System.Type>(new System.Type[] { typeof(Goal_RangedAttack) });
    Goal_RangedAttack attackGoal;

    RangeWeapon rangeWeapon;
    private void Start()
    {
        rangeWeapon = GetComponentInChildren<RangeWeapon>();
    }
    public override List<System.Type> GetSupportedGoals()
    {
        return SupportedGoals;
    }

    public override float GetCost()
    {
        //Weapon currentWeapon = hand.GetCurrentWeapon();

        //if (currentWeapon != null)
        //{
        //    int bulletsInMagasin = currentWeapon.Ammo;
        //    return 7 - bulletsInMagasin;
        //}


        return 1f;
    }
    public override void OnDeactivated()
    {
        //rangeWeapon.HandleWeaponAI(false, false, false, attackGoal.AttackTarget);
    }

    public override void OnActivated(Goal_Base linkedGoal)
    {
        base.OnActivated(linkedGoal);
        attackGoal = (Goal_RangedAttack)LinkedGoal;
       // Vector3 location = Agent.PickCoverInRange(SearchCoverRange);
        //Debug.Log("agent pos" + Agent.transform.position);
        //Debug.Log("cover pos" + location);
        //Agent.MoveTo(location);

    }

    public override void OnTick()
    {
   
        Agent.SetLookDirection(attackGoal.AttackTarget);

      
        if (rangeWeapon != null && attackGoal != null)
        {
            rangeWeapon.HandleWeaponAI(true, attackGoal.AttackTarget); 
        }
    }
}
