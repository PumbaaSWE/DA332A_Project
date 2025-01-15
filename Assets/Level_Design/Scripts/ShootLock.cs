using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootLock : MonoBehaviour, IDamageble
{
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody[] rb;
    public virtual void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        if (rb[0].isKinematic)
        {
            anim.SetTrigger("Open");
            for (int i = 0; i < rb.Length; i++)
            {
                rb[i].isKinematic = false;
            }
        }
    }
}
