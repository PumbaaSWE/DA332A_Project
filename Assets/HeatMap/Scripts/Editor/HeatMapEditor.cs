using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeatMap))]
public class HeatMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HeatMap heatMap = (HeatMap)target;
        if(GUILayout.Button("CreateHeatMap"))
        {
            heatMap.CreateHeatMap();
        }



    }


}
