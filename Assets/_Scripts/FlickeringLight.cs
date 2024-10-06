using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] float MinIntervalDelay, MaxIntervalDelay, MinFlickerDelay, MaxFlickerDelay;
    [SerializeField] int MinFlickers, MaxFlickers;
    [SerializeField] Material OnMaterial, OffMaterial;
    [SerializeField] List<Light> Lights;
    [SerializeField] bool IsOn;
    [SerializeField] bool FlickerOnStartup = true;
    [SerializeField] bool Burnout = false;
    [SerializeField] int IntervalsLeft;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Renderer>().material == OnMaterial)
            IsOn = true;

        if (FlickerOnStartup)
            StartCoroutine(Flicker());
    }

    public void StartFlickering()
    {
        StartCoroutine(Flicker());
    }

    public void StopFlickering()
    {
        StopAllCoroutines();
    }

    IEnumerator Flicker()
    {
        int flickers = Random.Range(MinFlickers, MaxFlickers + 1);

        for (int x = 0; x < flickers; x++)
        {
            Toggle();
            yield return new WaitForSeconds(Random.Range(MinFlickerDelay, MaxFlickerDelay));
            Toggle();
            yield return new WaitForSeconds(Random.Range(MinFlickerDelay, MaxFlickerDelay));
        }

        yield return new WaitForSeconds(Random.Range(MinIntervalDelay, MaxIntervalDelay));

        if (Burnout)
        {
            if (--IntervalsLeft == 0)
                Toggle(false);

            else
                StartCoroutine(Flicker());
        }

        else
            StartCoroutine(Flicker());
    }

    public void Toggle(bool value)
    {
        IsOn = value;

        foreach (Light light in Lights)
            light.enabled = IsOn;

        switch (IsOn)
        {
            case true:
                if (OnMaterial != null)
                    GetComponent<Renderer>().material = OnMaterial;
                break;
            case false:
                if (OffMaterial != null)
                    GetComponent<Renderer>().material = OffMaterial;
                break;
        }
    }

    public void Toggle()
    {
        Toggle(!IsOn);
    }

    public void InstantBurnout()
    {
        StopAllCoroutines();
        Toggle(false);
    }
}