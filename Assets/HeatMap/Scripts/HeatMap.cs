using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static HeatMapDataJson;

public class HeatMap : MonoBehaviour
{

    //public Vector3[] positions; // read from Json and convert to array
    public HeatMapDataJson.DataContainer [] data;
    public string filePath;

    public GameObject prefab;
    public void CreateHeatMap()
    {
        Debug.Log("creating heat map");
    }

    /*
    public void DrawLines()
    {
        
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 lastPosition = positions[i];
            Vector3 newPosition = positions[i + 1];
            Gizmos.DrawLine(lastPosition, newPosition);
        }

    }
    */

    public void CreateObjects()
    {
        ReadJson(filePath);
        
        for (int i = 0; i < data.Length; i++)
        {
            Vector3 lastPosition = data[i].playerPos;
            GameObject obj1 = Instantiate(prefab);
            obj1.transform.position = lastPosition;
        }
    }

    public void ReadJson(string name)
    {
        name = Path.GetDirectoryName(Application.dataPath) + "/Assets/HeatMap/SessionData/test.json";
        string fileString = File.ReadAllText(name);
        data = JsonUtility.FromJson<Wrapper>(fileString).dataContainers;
        Debug.Log(data.Length);

    }


 
}
