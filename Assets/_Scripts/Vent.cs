using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{

    public Transform venticle;
    public Transform[] venticles;
    public float spacing = 0.05f;
    public float angle = 45;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MakeButton]
    public void Place()
    {
        Vector3 p = transform.position;
        for (int i = 0; i < venticles.Length; i++)
        {
            venticles[i].position = p;
            p += Vector3.right * spacing;
            venticles[i].eulerAngles = new Vector3(0, 0, angle);
        }
    }
    
}
