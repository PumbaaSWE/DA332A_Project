using System;
using UnityEngine;

public class EquipmentSwapper : MonoBehaviour
{

    [SerializeField] private Animator animator;  
    private enum SwitchState { Raising, Holding, Lowering, WaitForEvent };
    private SwitchState switchState = SwitchState.Holding;
    private float timer;
    //private bool shouldDrop;
    private Equipment replaceEquipment;
    private Equipment current;
    private static readonly int lowerBoolHash = Animator.StringToHash("LowerBool");
    private static readonly int equipmentIdHash = Animator.StringToHash("EquipmentId");

    public event Action<Equipment> OnLowerCompleted;
    public event Action<Equipment> OnRaiseCompleted;

    public Equipment Current => current;

    void Awake()
    {
        Debug.Assert(animator, "EquipmentSwapper - animator is null, please assign it in inspector");
        switchState = SwitchState.Holding;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        switch (switchState)
        {
            case SwitchState.Raising:
                HandleRaiseing(dt);
                break;
            case SwitchState.Holding:
                break;
            case SwitchState.Lowering:
                HandleLowering(dt);
                break;
            case SwitchState.WaitForEvent:
                break;
            default:
                break;
        }
    }

    public void LowerCurrent()
    {
        animator.SetBool(lowerBoolHash, true);
        current.BeginLower();
        current.DisableInput();
        if (current.EquipmentData.AnimationEventLower)
        {
            //we wait for an animation event;
            switchState = SwitchState.WaitForEvent;
        }
        else
        {
            timer = current.EquipmentData.LowerTime;
            switchState = SwitchState.Lowering;
        }
    }

    private void HandleLowering(float dt)
    {
        timer -= dt;
        if (timer < 0) LowerComplete();
    }

    private void LowerComplete()
    {
        animator.SetBool(lowerBoolHash, false);
        current.CompletedLower();
        current.gameObject.SetActive(false);
        OnLowerCompleted?.Invoke(current);
        current = replaceEquipment;
        RaiseCurrent();
    }

    private void RaiseCurrent()
    {
        if (current == null) {
            //RaiseComplete();
            return;
        }

        current.gameObject.SetActive(true);
        current.BeginRaise();

        if (current.EquipmentData.RaiseStateHash != 0)
        {
            animator.SetInteger(equipmentIdHash, 0);
            animator.CrossFade(current.EquipmentData.RaiseStateHash, 0, 0);
        }
        else
        {
            animator.SetInteger(equipmentIdHash, current.EquipmentData.EquipmentId);
        }


        if (current.EquipmentData.AnimationEventRaise)
        {
            switchState = SwitchState.WaitForEvent;
        }
        else
        {
            timer = current.EquipmentData.RaiseTime;
            switchState = SwitchState.Raising;
        }

    }

    public void Raise(Equipment equipment)
    {
        if (equipment == null) return;
        if(equipment == current)
        {
            current.Reselect();
            return;
        }
        
        if (current != null)
        {
            LowerCurrent();
            replaceEquipment = equipment;
        }
        else
        {
            current = equipment;
            RaiseCurrent();
        }
    }

    private void HandleRaiseing(float dt)
    {
        timer -= dt;
        if (timer < 0) RaiseComplete();
    }

    private void RaiseComplete()
    {
        current.EnableInput();
        current.CompletedRaise();
        OnRaiseCompleted?.Invoke(current);
        switchState = SwitchState.Holding;
    }
}
