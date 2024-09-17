using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSControllerCapsule : MonoBehaviour
{
    [SerializeField] private CapsuleCollider cc;

    private void Update()
    {
        if (!cc) return;

        transform.localPosition = Vector3.up * cc.height / 2f;
        transform.localScale = new Vector3(cc.radius * 2f, cc.height / 2f, cc.radius * 2f);
    }
}
