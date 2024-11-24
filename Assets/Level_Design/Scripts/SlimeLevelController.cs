using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlimeLevelController : MonoBehaviour
{
    // Start is called before the first frame update

    Material material;
    float fulness;
    public SlimePlane splane;
    bool done = false;
    public UnityEvent<Transform> OnDone;

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

        if (done) return;
        material.color = Color.red;
        
        fulness += splane.slimeCounter;
        material.SetFloat("_Fulness", fulness);
        splane.slimeCounter = 0f;

        if (fulness > splane.slimeDone)
        {
            done = true;
            OnDone.Invoke(transform);
        }
    }
}
