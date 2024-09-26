using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{

    Light m_light;
    public float flickerSpeed = 1;
    public float lightRangeMax = 10;
    public float lightRangeMin = 8;

    public float lightIntensityMax = 1.5f;
    public float lightIntensityMin = .5f;

    public Color lightColor1 = Color.red;
    public Color lightColor2 = Color.yellow;

    // Start is called before the first frame update
    void Start()
    {
        m_light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PerlinNoise1D(Time.time * flickerSpeed);
        m_light.range = (t * (lightRangeMax - lightRangeMin)) + lightRangeMin;
        m_light.intensity = lightIntensityMin + (t * (lightIntensityMax - lightIntensityMin));
        m_light.color = Color.Lerp(lightColor1, lightColor2, t);
    }
}
