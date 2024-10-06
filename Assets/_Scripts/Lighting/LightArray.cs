using System.Collections;
using UnityEngine;

public class LightArray : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float delay = 0;
    [SerializeField] private float spacing = 1;
    [SerializeField] private int amount = 1;
    [SerializeField] private Light[] lights;
    [SerializeField] private GameObject[] lamps;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void SpawnLight(int amount, float distance)
    {
        Vector3 pos = transform.position;
        lights = new Light[amount];
        lamps = new GameObject[amount];
        for (int i = 0; i < amount; i++)
        {
            GameObject go = Instantiate(prefab, pos, transform.rotation, transform);
            Light light = go.GetComponentInChildren<Light>();
            lights[i] = light;
            lamps[i] = go;
            pos += transform.forward * distance;
        }
    }
    [MakeButton]
    public void SpawnLights()
    {
        if(lamps != null)
        {
            for (int i = 0; i < lamps.Length; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(lamps[i]);
                }
                else
                {
                    DestroyImmediate(lamps[i]);
                }
            }
        }
        //while(transform.childCount > 0)
        //{
        //    if (Application.isPlaying)
        //    {
        //        Destroy(transform.GetChild(0).gameObject);
        //    }
        //    else
        //    {
        //        DestroyImmediate(transform.GetChild(0).gameObject);
        //    }
        //}

        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        lights = new Light[amount];
        lamps = new GameObject[amount];
        Vector3 pos = transform.position;
        for (int i = 0; i < amount; i++)
        {
            GameObject go = Instantiate(prefab, pos, transform.rotation, transform);
            Light light = go.GetComponentInChildren<Light>();
            lights[i] = light;
            lamps[i] = go;
            pos += transform.forward * spacing;
        }

    }

    public void AdjustSpacing(float spacing)
    {
        this.spacing = spacing;
        Vector3 pos = transform.position;
        for (int i = 0; i < lamps.Length; i++)
        {
            lamps[i].transform.position = pos;
            pos += transform.forward * spacing;
        }
    }
#if UNITY_EDITOR
    private float spacingEditor;
    private void OnValidate()
    {
        amount = Mathf.Max(1, amount);
        if (!spacingEditor.Approx(spacing))
        {
            AdjustSpacing(spacing);
            spacingEditor = spacing;
        }
    }
#endif

    [MakeButton]
    public void LightUp()
    {
        if (delay == 0) LightUp(true);
        else StartCoroutine(LightUpCoroutine(true));
    }

    [MakeButton]
    public void TurnOff()
    {
        if (delay == 0) LightUp(false);
        else StartCoroutine(LightUpCoroutine(false));
    }

    private IEnumerator LightUpCoroutine(bool on)
    {
        WaitForSeconds wait = new(delay);
        
        foreach (Light light in lights)
        {
            light.enabled = on;
            yield return wait;
        }
    }

    public void LightUp(bool on)
    {
        foreach (Light light in lights)
        {
            light.enabled = on;
        }
    }
}
