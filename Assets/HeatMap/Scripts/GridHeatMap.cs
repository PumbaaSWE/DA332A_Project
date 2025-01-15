using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static HeatMapDataJson;

public class GridHeatMap : MonoBehaviour
{
    //public string selectedFilePath = "";

    [SerializeField] HeatMap CMData;
    [SerializeField] bool dontUseCubeHeatMapData;
    public TextAsset dataFile;
    DataContainer[] data;

    [SerializeField] int widthAndHeight;
    [SerializeField] float cellSize;

    [SerializeField] bool ShowNumbersAndBoxes;

    GridMap gridMap;
    [SerializeField] HeatMapMesh heatMapMesh;

    

    [SerializeField]
    [Range(0, 90)] int maxValue;
    [SerializeField]
    [Range(0, 90)] int minValue;

    // Start is called before the first frame update
    void Start()
    {
        if (dontUseCubeHeatMapData)
        {
            string fileString = dataFile.text;
            data = JsonUtility.FromJson<Wrapper>(fileString).dataContainers;
        }
        else
            data = CMData.data;

        //data = JsonUtility.FromJson<Wrapper>(selectedFilePath).dataContainers;

        gridMap = new GridMap(widthAndHeight, widthAndHeight, cellSize, transform.position);

        foreach (DataContainer position in data)
        {
            gridMap.AddValue(position.playerPos, 25);
        }

        gridMap.ScaleData(maxValue, minValue);

        if (ShowNumbersAndBoxes)
            gridMap.DrawDebug();

        heatMapMesh.UpdateGridMesh(gridMap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void ScaleNumbers(DataContainer data)
}

//[CustomEditor(typeof(GridHeatMap))]
//public class FileExplorerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        GridHeatMap fileSelector = (GridHeatMap)target;

//        GUILayout.Label("Select a file path", EditorStyles.boldLabel);
//        EditorGUILayout.LabelField("Selected File Path", fileSelector.selectedFilePath);

//        // Opens the file dialog and update the file path in the component
//        if (GUILayout.Button("Select File"))
//            fileSelector.selectedFilePath = EditorUtility.OpenFilePanel("Select JSon File", "Assets/HeatMap/SessionData", "json"); //Title, directory, extension

//        //Saves changes
//        if (GUI.changed)
//            EditorUtility.SetDirty(fileSelector);

//        DrawDefaultInspector();
//    }
//}


