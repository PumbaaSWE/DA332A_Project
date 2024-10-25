using UnityEngine;

[DefaultExecutionOrder(-10)]
public class HandIKHelper : MonoBehaviour
{

    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform rightHandGrip;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform leftHandGrip;

    void Start()
    {
        HandIKTracker handIKTracker = GetComponentInParent<HandIKTracker>();
        if (handIKTracker)
        {
            rightHandTarget = handIKTracker.RightHandIKTarget;
            leftHandTarget = handIKTracker.LeftHandIKTarget;
            //leftHandIK = handIKTracker.leftHandIK;
            //rightHandIK = handIKTracker.rightHandIK;
        }
        Debug.Assert(handIKTracker, "HandIKHelper - No hand tracker found in parent");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        rightHandTarget.SetPositionAndRotation(rightHandGrip.position, rightHandGrip.rotation);
        leftHandTarget.SetPositionAndRotation(leftHandGrip.position, leftHandGrip.rotation);
    }

    public void SetTarget(Transform rightHandTarget, Transform leftHandTarget)
    {
        this.rightHandTarget = rightHandTarget;
        this.leftHandTarget = leftHandTarget;
    }

#if UNITY_EDITOR

    [ContextMenu("Find grips")]
    public void Setup()
    {
        rightHandGrip = transform.FindChildRecursively("RightGrip");
        leftHandGrip = transform.FindChildRecursively("LeftGrip");
        Debug.Assert(rightHandGrip, "HandIKHelper - no gameObject found with name RightGrip");
        Debug.Assert(leftHandGrip, "HandIKHelper - no gameObject found with name LeftGrip");
    }

#endif
}

