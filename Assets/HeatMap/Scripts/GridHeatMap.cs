using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static HeatMapDataJson;

public class GridHeatMap : MonoBehaviour
{
    public string selectedFilePath = "";

    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float cellSize;

    [SerializeField] bool ShowNumbersAndBoxes;

    GridMap gridMap;
    [SerializeField] HeatMapMesh heatMapMesh;

    //[SerializeField] Transform testPos;
    public TextAsset file;
    DataContainer[] data;

    // Start is called before the first frame update
    void Start()
    {
        string fileString = file.text;
        data = JsonUtility.FromJson<Wrapper>(fileString).dataContainers;
        //data = JsonUtility.FromJson<Wrapper>(selectedFilePath).dataContainers;

        gridMap = new GridMap(width, height, cellSize, transform.position);

        foreach (DataContainer position in data)
        {
            gridMap.AddValue(position.playerPos, 25);
        }

        if (ShowNumbersAndBoxes)
            gridMap.DrawDebug();

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


