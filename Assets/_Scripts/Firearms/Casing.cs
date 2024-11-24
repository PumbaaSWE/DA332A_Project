using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    public Action PlaySound;
    public float MinEmitVelocity;

    public void SetDestroy(float lifeTime)
    {
        StartCoroutine(Destructor(lifeTime));
    }

    IEnumerator Destructor(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GetComponent<Rigidbody>().velocity.magnitude <= MinEmitVelocity)
            PlaySound();
    }
}
