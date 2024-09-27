using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveItem : MonoBehaviour, IDamageble
{
    public GameObject cube;
 
    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        cube.transform.position += direction*0.1f;
        cube.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

    }


}
