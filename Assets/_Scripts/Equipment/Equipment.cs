using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{

    [SerializeField] protected EquipmentData _equipmentData;
    public EquipmentData EquipmentData => _equipmentData;
    protected Animator animator;
    protected PlayerControls input;

    public void Init(Animator animator)
    {
        this.animator = animator;
        input ??= new PlayerControls();
    }

    public void EnableInput()
    {
        input.Enable();
    }

    public void DisableInput()
    {
        input.Disable();
    }

    public virtual void BeginLower()
    {
    }

    public virtual void CompletedLower()
    {
    }

    public virtual void Reselect()
    {
    }

    public virtual void BeginRaise()
    {
    }

    public virtual void CompletedRaise()
    {
    }
}
