
using UnityEngine;

public class Detachable : MonoBehaviour
{
    public GameObject disposableJoint;
    public bool detached;
    public bool growing;
    public Detachable parent;
    public Detachable child;
    float timer;
    float time;
    Rigidbody rbAnchor;
    float[] limits = new float[4];
    public bool leg;
    public bool head;
    public bool arm;
    private void Awake()
    {
        if (transform.parent != null && transform.parent.TryGetComponent(out Detachable detachable))
        {
            detachable.child = this;
            parent = detachable;
        }
        enabled = false;
    }

    public void Regrow(float t)
    {
        if (parent == null)
        {
            //we can regrow
            RegrowSelf(t);
        }
        else if (parent.detached)
        {
            parent.Regrow(t);
            //we cannot regrow
        }
        else
        {
            RegrowSelf(t);
        }
    }

    public void Detatch()
    {
        if (detached) return;
        detached = true;
        if (parent == null || !parent.detached)
        {
            //Destroy(go.GetComponent<Joint>());
            //CharacterJoint joint = GetComponent<CharacterJoint>();
            //if(joint != null)
            //{
            //    //SoftJointLimit ht = joint.highTwistLimit;
            //    //limits[0] = ht.limit;
            //    //ht.limit = 0;
            //    //joint.highTwistLimit = ht;
            //    //SoftJointLimit lt = joint.lowTwistLimit;
            //    //limits[1] = lt.limit;
            //    //lt.limit = 0;
            //    //joint.lowTwistLimit = lt;
            //    //SoftJointLimit s1 = joint.swing1Limit;
            //    //limits[2] = s1.limit;
            //    //s1.limit = 0;
            //    //joint.swing1Limit = s1;
            //    //SoftJointLimit s2 = joint.swing2Limit;
            //    //limits[3] = s2.limit;
            //    //s2.limit = 0;
            //    //joint.swing2Limit = s2;
            //    Debug.Log("Enabled prjoregdfgh");
            //    joint.enableProjection = true;
            //}
            //rbAnchor = joint.connectedBody;
            //joint.connectedBody = null;
        }

        if (disposableJoint)
        {
            GameObject go = Instantiate(disposableJoint, transform.position, transform.rotation);
            if (go.TryGetComponent(out Rigidbody rb) && TryGetComponent(out Rigidbody originalRb))
            {
                //commented off to check correct pos and rot

                //rb.velocity = originalRb.velocity;
                //rb.angularVelocity = originalRb.angularVelocity;
                //rb.AddForce(originalRb.GetAccumulatedForce());
                //rb.AddTorque(originalRb.GetAccumulatedTorque());
            }
            Destroy(go, 30);
        }

        transform.localScale = Vector3.zero;
        if (child != null)
        {
            child.Detatch();
        }
    }

    private void RegrowSelf(float t)
    {
        growing = true;
        enabled = true;
        time = t;
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / time;

        if (t > 1)
        {
            t = 1;
            enabled = false;
            detached = false;
            if (child != null)
            {
        
                child.Regrow(time);
            }
            growing = false;

            //CharacterJoint joint = GetComponent<CharacterJoint>();
            //if (joint != null)
            //{
            //    //SoftJointLimit ht = joint.highTwistLimit;
            //    //ht.limit = limits[0];
            //    //joint.highTwistLimit = ht;
            //    //SoftJointLimit lt = joint.lowTwistLimit;
            //    //lt.limit = limits[1];
            //    //joint.lowTwistLimit = lt;
            //    //SoftJointLimit s1 = joint.swing1Limit;
            //    //s1.limit = limits[2];
            //    //joint.swing1Limit = s1;
            //    //SoftJointLimit s2 = joint.swing2Limit;
            //    //s2.limit = limits[3];
            //    //joint.swing2Limit = s2;
            //    Debug.Log("Disable prjoregdfgh");
            //    joint.enableProjection = false;
            //}

            //CharacterJoint joint = GetComponent<CharacterJoint>();
            //joint.connectedBody = rbAnchor;
        }

        transform.localScale = Vector3.one * t;

    }

    internal bool DetatchedLimb()
    {
        if (detached) return true;
        if (!detached && child == null) return false;
        return child.DetatchedLimb();
    }
}
