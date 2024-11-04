using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Regrow : MonoBehaviour
{
    public enum RegrowState
    {
        Default,
        RegrowLimbs,
        RemoveLimbs
    };
    public RegrowState state = RegrowState.Default;

    public List<Detachable> detached = new List<Detachable>();
    private Rigidbody[] rbs;
    private Transform hip;
    private Animator animator;

    [SerializeField] float regrowTime = 2;
    [SerializeField] float legRegrowTime = 7;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        hip = animator.GetBoneTransform(HumanBodyBones.Hips);
        rbs = hip.GetComponentsInChildren<Rigidbody>();

    }

    void Update()
    {

        switch (state)
        {
            case RegrowState.Default:
                DefaultBehaviour();
                break;

            case RegrowState.RegrowLimbs:
                //ReGrow();
                break;
            case RegrowState.RemoveLimbs:
                RegrowLimbsBehaviour();
                break;

            default:
                break;
        }
    }
    public Detachable Hit(Vector3 point)
    {
        Rigidbody rb = rbs.OrderBy(rb => (rb.position - point).sqrMagnitude).First();

        if (rb.TryGetComponent(out Detachable detachable))
        {
            TriggerRegrow(detachable);

        }

        return detachable;
    }
    public void TriggerRegrow(Detachable detachable)
    {

        detachable.Detatch();
        if(detachable.leg)
        {
            detachable.Regrow(legRegrowTime);
        }
        else
        {
            detachable.Regrow(regrowTime);

        }
        detached.Add(detachable);

        state = RegrowState.RemoveLimbs;
    }
    //public void ReGrow()
    //{
    //    foreach (var detachable in detached)
    //    {
    //        detachable.Regrow(regrowTime);
    //    }A
    //    state = RegrowState.RemoveLimbs;
    //}

    private void RegrowLimbsBehaviour()
    {
        if (detached.Count <= 0)
        {
            state = RegrowState.Default;
            return;
        }
        detached.RemoveAll(x => !x.DetatchedLimb());
    }
    private void DefaultBehaviour()
    {

    }


    public bool IsLegDetached()
    {
        foreach (var detachable in detached)
        {
            if (detachable.leg && detachable.detached)
            {
                return true;
            }
            if(detachable.child != null)
            {
                if (detachable.child.leg && detachable.child.detached)
                {
                    return true;
                }
            }
            
        }

        return false;
    }
    public bool IsHeadDetached()
    {
        foreach (var detachable in detached)
        {
            if (detachable.head && detachable.detached)
            {
                return true;
            }

        }

        return false;
    }
    public bool IsArmDetached()
    {
        foreach (var detachable in detached)
        {
            if (detachable.arm && detachable.detached)
            {
                return true;
            }
            if(detachable.child != null)
            {
                if (detachable.child.arm && detachable.child.detached)
                {
                    return true;
                }

            }
          
        }

        return false;
    }

}


