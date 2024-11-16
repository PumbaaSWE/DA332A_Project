using UnityEngine;

public class TailChain : MonoBehaviour
{
    [SerializeField] Transform tailEnd;
    [SerializeField] Transform[] tailParts;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [MakeButton]
    public void BuildTail()
    {
        tailParts = transform.GetComponentsInChildren<Transform>();
        int length = tailParts.Length - 1;
        Vector3 p1 = Vector3.zero;
        Transform current = tailParts[0];
        for (int i = 0; i < length; i++)
        {
            Transform next = tailParts[i+1];
            CapsuleCollider cc = current.gameObject.GetOrAdd<CapsuleCollider>();

            


            Vector3 p2 = current.InverseTransformPoint(next.position);
            Vector3 c = (p1 + p2) / 2;
            //c = current.InverseTransformPoint(c);
            cc.center = c;
            cc.height = Vector3.Distance(p1, p2);
            cc.radius = 0.0004f;

            current = next;
            //p1 = p2;

        }
    }

    [MakeButton]
    public void RagdollifyTail()
    {
        tailParts = transform.GetComponentsInChildren<Transform>();
        int length = tailParts.Length - 1;
        Vector3 p1 = Vector3.zero;
        Transform current = tailParts[0];
        
        for (int i = 0; i < length; i++)
        {
            Transform next = tailParts[i + 1];
            CharacterJoint cj = next.gameObject.GetOrAdd<CharacterJoint>();
            Rigidbody prb = current.gameObject.GetOrAdd<Rigidbody>();
            cj.connectedBody = prb;
            cj.swing1Limit = new SoftJointLimit()
            {
                limit = 45
            };
            cj.swing2Limit = new SoftJointLimit()
            {
                limit = 45
            };

            cj.lowTwistLimit = new SoftJointLimit()
            {
                limit = 45
            };
            cj.highTwistLimit = new SoftJointLimit()
            {
                limit = 135
            };

            current = next;
            //p1 = p2;

        }
    }
}
