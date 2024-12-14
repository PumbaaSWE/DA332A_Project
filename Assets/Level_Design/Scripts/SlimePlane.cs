using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlimePlane : MonoBehaviour
{
    // Start is called before the first frame update

    public float slimeCounter;
    public float slimePerCollision = 0.01f;
    public float slimePartWay = 0.2f;
    public float slimeDone = 0.4f;


    public UnityEvent<Transform> OnPart;
    public UnityEvent<Transform> OnDone;

    bool part = false;
    bool done = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
       slimeCounter += slimePerCollision;

        if (slimeCounter > slimePartWay && slimeCounter < slimeDone && !part)
        {
            OnPart.Invoke(transform);
            part = true;
        }
        else if (slimeCounter > slimeDone && !done)
        {
            OnDone.Invoke(transform);
            done = true;
        }

    }

    public void IncreaseSlime(float value)
    {
        slimeCounter += value;

        if (slimeCounter > slimePartWay && slimeCounter < slimeDone && !part)
        {
            OnPart.Invoke(transform);
            part = true;
        }
        else if (slimeCounter > slimeDone && !done)
        {
            OnDone.Invoke(transform);
            done = true;
        }
    }
}
