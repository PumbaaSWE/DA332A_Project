using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeLight : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject lampsHolder;
    private List<LightFlicker> lightFlickers;
    [SerializeField] GameObject directional;


    bool isOn = false;
    private void OnEnable()
    {
        LevelSwitch.OnAction += TurnOnLight;
    }
    private void OnDisable()
    {
        LevelSwitch.OnAction -= TurnOnLight;
    }

    public void Start()
    {
        lightFlickers = lampsHolder.GetComponentsInChildren<LightFlicker>().ToList();
    }
    public void TurnOnLight()
    {
        if (!isOn)
        {
            Debug.Log("TurnOnLight on lights");
            foreach (LightFlicker l in lightFlickers)
            {
                l.myLight.intensity = 1f;
                l.enabled = false;
            }

            directional.SetActive(true);
            isOn = true;
        }
    }
}
