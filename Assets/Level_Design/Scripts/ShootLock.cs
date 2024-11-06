using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLock : MonoBehaviour, IDamageble
{
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody rb;
    public virtual void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        if (rb.isKinematic)
        {
            anim.SetTrigger("Open");
            rb.isKinematic = false;
        }
    }
}
