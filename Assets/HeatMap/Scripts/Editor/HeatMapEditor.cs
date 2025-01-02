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


        if (GUILayout.Button("InitializeComponents"))
        {
            heatMap.InitializeComponents();
        }
       
        if (GUILayout.Button("CreateCubes"))
        {
            heatMap.CreateCubes();
        }

        if (GUILayout.Button("ChangeCubesColor"))
        {
            heatMap.ChangeCubesColor();
        }
        if (GUILayout.Button("DeleteCubes"))
        {
            heatMap.DeleteCubes();
        }

        if (GUILayout.Button("DrawLines"))
        {
            heatMap.DrawLines();
        }
        
        if (GUILayout.Button("DeleteLines"))
        {
            heatMap.DeleteLines();
        }
        
        if (GUILayout.Button("CreateTexture"))
        {
            heatMap.CreateTexture();
        }
        
        







    }


}
