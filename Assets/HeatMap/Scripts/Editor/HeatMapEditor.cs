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

        if (GUILayout.Button("CreateDataContainerArrayFromJson"))
        {
            heatMap.CreateDataContainerArrayFromJson();
        }

        if (GUILayout.Button("CreateGameObjList"))
        {
            heatMap.CreateGameObjList();
        }
        if (GUILayout.Button("ChangeObjColor"))
        {
            heatMap.ChangeObjColor();
        }
        if (GUILayout.Button("EmptyGameObjList"))
        {
            heatMap.EmptyGameObjList();
        }

        if (GUILayout.Button("CreateArrayOfVectors"))
        {
            heatMap.CreateArrayOfVectors();
        }

        if (GUILayout.Button("DrawLines"))
        {
            heatMap.DrawLines();
        }
        
        if (GUILayout.Button("DeleteLines"))
        {
            heatMap.DeleteLines();
        }

        /*
        if (GUILayout.Button("CreateTexture"))
        {
            //heatMap.CreateTexture();
        }
        */







    }


}
