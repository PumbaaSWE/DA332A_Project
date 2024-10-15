using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class LightController : MonoBehaviour
{
    [SerializeField] Light[] lights;
    [SerializeField] Color ambientColor;
    [SerializeField] Color lightColor;


    public void Start()
    {
        lights = GetComponentsInChildren<Light>();
    }
    [MakeButton]
    public void TurnOnAllLights()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = true;
            lights[i].color = lightColor;
        }
        RenderSettings.ambientLight = ambientColor;
    }


    [MakeButton]
    public void TurnOffAllLights()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = false;
        }
        RenderSettings.ambientLight = Color.black;
    }
}
