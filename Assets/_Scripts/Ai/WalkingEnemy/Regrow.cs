using System.Collections;
using System.Collections.Generic;
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

    public void TriggerRegrow(Vector3 point)
    {
        Rigidbody rb = rbs.OrderBy(rb => (rb.position - point).sqrMagnitude).First();

        if (rb.TryGetComponent(out Detachable detachable))
        {
            detachable.Detatch();
            detachable.Regrow(regrowTime);
            detached.Add(detachable);
        }
        state = RegrowState.RemoveLimbs;
    }
    //public void ReGrow()
    //{
    //    foreach (var detachable in detached)
    //    {
    //        detachable.Regrow(regrowTime);
    //    }
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

}
