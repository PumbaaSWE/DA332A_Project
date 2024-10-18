using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeLevelController : MonoBehaviour
{
    // Start is called before the first frame update

    Material material;
    float fulness;
    public SlimePlane splane;
    void Start()
    {
        material = GetComponent<Renderer>().material;
        fulness = material.GetFloat("_Fulness");
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseSlime()
    {
        material.color = Color.red;
        
        fulness += splane.slimeCounter;
        material.SetFloat("_Fulness", fulness);
        splane.slimeCounter = 0f;
    }
}
