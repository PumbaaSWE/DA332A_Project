using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePlane : MonoBehaviour
{
    // Start is called before the first frame update

    public float slimeCounter;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
       slimeCounter += 0.01f;
    }
}
