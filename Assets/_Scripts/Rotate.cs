using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] Vector3 axis;
    [SerializeField] float anglePerSec;

    private void Update()
    {
        transform.Rotate(axis, anglePerSec * Time.deltaTime, Space.Self);
    }
}
