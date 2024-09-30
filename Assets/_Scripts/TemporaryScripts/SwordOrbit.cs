using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordOrbit : MonoBehaviour
{
    public Transform enemy;  
    public float orbitDistance = 2f;  
    public float orbitSpeed = 50f;    

    void Update()
    {
      
    }

    public void Orbit()
    {
      
        transform.RotateAround(enemy.position, Vector3.up, orbitSpeed * Time.deltaTime);

        Vector3 desiredPosition = (transform.position - enemy.position).normalized * orbitDistance + enemy.position;
        transform.position = desiredPosition;
    }
}
