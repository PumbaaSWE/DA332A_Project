using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientLight : MonoBehaviour
{

    [SerializeField] Color color; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LightenUp()
    {
        SetAmbient(color);
    }

    public void SetAmbient(Color color)
    {
        RenderSettings.ambientLight = color;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
