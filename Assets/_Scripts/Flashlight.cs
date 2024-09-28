using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is temporary
public class Flashlight : MonoBehaviour
{

    public Light m_light;
    
    // Start is called before the first frame update
    void Start()
    {
        m_light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            m_light.enabled = !m_light.enabled;
        }
    }
}
