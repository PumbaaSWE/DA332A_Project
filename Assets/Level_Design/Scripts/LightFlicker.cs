using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light myLight;
    public float maxInterval = 1;
    public float maxFlicker = 0.2f;

    float defaultIntensity;
    bool isOn;
    float timer;
    float delay;

    void Start()
    {
        myLight = GetComponent<Light>();
        defaultIntensity = myLight.intensity;


        //
    }

    void Update()
    {
        if (myLight == null)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer > delay)
        {
            ToggleLight();
        }
    }

    void ToggleLight()
    {
        isOn = !isOn;

        if (isOn)
        {
            myLight.intensity = defaultIntensity;
            delay = Random.Range(0, maxInterval);
        }
        else
        {
            myLight.intensity = Random.Range(0.6f, defaultIntensity);
            delay = Random.Range(0, maxFlicker);
        }

        timer = 0;
    }

}
