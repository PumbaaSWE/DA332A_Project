using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.InputSystem.Controls.AxisControl;

public class LightArrayHandler : MonoBehaviour
{
    
    public LightArray[] lightArrays;
    public bool lightOn;
    public float spacing = 2;
    public float arraySpacing = 2;

    // Start is called before the first frame update
    void Start()
    {
        lightArrays = GetComponentsInChildren<LightArray>();
        if(!lightOn) TurnOff();
    }

    public void TurnOn()
    {
        if (lightArrays == null) return;
        foreach (LightArray lightArray in lightArrays)
        {
            lightArray.LightUp();
        }
    }

    public void TurnOff()
    {
        if (lightArrays == null) return;
        foreach (LightArray lightArray in lightArrays)
        {
            lightArray.TurnOff();
        }
    }

    public void AdjustSpacing()
    {
        if (lightArrays == null) return;
        Vector3 pos = transform.position;
        for (int i = 0; i < lightArrays.Length; i++)
        {
            pos += transform.forward * spacing;
            lightArrays[i].transform.position = pos;
        }
    }

#if UNITY_EDITOR
    private float spacingEditor;
    private float arraySpacingEditor;
    private void OnValidate()
    {
        if (lightArrays != null && !spacingEditor.Approx(spacing))
        {
            //AdjustSpacing();
            spacingEditor = spacing;
        }

        //if (!arraySpacingEditor.Approx(arraySpacing))
        //{
        //    arraySpacingEditor = arraySpacing;
        //    for (int i = 0; i < lightArrays.Length; i++)
        //    {
        //        lightArrays[i].AdjustSpacing(arraySpacing);
        //    }
        //}
    }
#endif

    // Update is called once per frame
    void Update()
    {
        
    }
}
