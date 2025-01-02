using System.Collections;
using UnityEngine;

public class AmbientLight : MonoBehaviour
{

    [SerializeField] Color color;
    [SerializeField] bool onStart;
     Color currentColor; 
    
    // Start is called before the first frame update
    void Start()
    {
       if(onStart)
            StartCoroutine(LerpLight());
    }

    private IEnumerator LerpLight()
    {
        currentColor = RenderSettings.ambientLight;
        if (color == currentColor) yield break;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            RenderSettings.ambientLight = Color.Lerp(currentColor, color, t);
            yield return null;
        }
        
    }

    public void LightenUp()
    {
        SetAmbient(color);
    }

    public void LerpUp()
    {
        StartCoroutine(LerpLight());
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
