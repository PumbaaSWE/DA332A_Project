using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridHeatMap : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float cellSize;

    public string selectedFilePath = "";

    [SerializeField] HeatMapMesh heatMapMesh;

    // Start is called before the first frame update
    void Start()
    {
        GridMap gridMap = new GridMap(width, height, cellSize, transform.position);
        heatMapMesh.UpdateGridMesh(gridMap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[CustomEditor(typeof(GridHeatMap))]
public class FileExplorerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridHeatMap fileSelector = (GridHeatMap)target;

        GUILayout.Label("Select a file path", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Selected File Path", fileSelector.selectedFilePath);

        // Open the file dialog and update the file path in the component
        if (GUILayout.Button("Select File"))
            fileSelector.selectedFilePath = EditorUtility.OpenFilePanel("Select JSon File", "Assets/HeatMap/SessionData", "json"); //Title, directory, extension

        //Save changes
        if (GUI.changed)
            EditorUtility.SetDirty(fileSelector);

        DrawDefaultInspector();
    }
}


