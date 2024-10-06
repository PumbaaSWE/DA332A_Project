using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saftblandare : MonoBehaviour
{

    [SerializeField] Transform blanda;
    [SerializeField] float blandSpeed = 1;
    [SerializeField] Vector3 axis = Vector3.forward;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        blanda.Rotate(axis, blandSpeed * Time.deltaTime);
    }
}
