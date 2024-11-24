using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootVent : MonoBehaviour, IDamageble
{
    Rigidbody rb;
    [SerializeField] HingeJoint[] joints;
    int jointNr;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jointNr = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        if (jointNr >= joints.Length) { return; }
        if (joints[jointNr])
        {

            Destroy(joints[jointNr]);
            jointNr++;
        }
    }

    public void DestroyJoints()
    {
        for (int i = 0;  i < joints.Length; i++)
        {
            if (joints[i] != null)
            {
                Destroy(joints[i]);
            }
        }
    }
}
