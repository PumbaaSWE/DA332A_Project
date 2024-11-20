using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static HeatMapDataJson;

public class HeatMap : MonoBehaviour
{

    //public Vector3[] positions; // read from Json and convert to array
    public DataContainer [] data;
    public string filePath;

    public GameObject prefab;
    private List<GameObject> listObjects;
    public Vector3[] dataPosition;

    LineRenderer lineRenderer;
 


    public void InitializeComponents()
    {
        Debug.Log("Initializing");
        lineRenderer = GetComponent<LineRenderer>();
        listObjects = new List<GameObject>();
        dataPosition = new Vector3[data.Length];
    }

    public void CreateDataContainerArrayFromJson()
    {
        ReadJson(filePath);                
    }

    public void CreateGameObjList()
    {
        for (int i = 0; i < data.Length; i++)
        {
            Vector3 lastPosition = data[i].playerPos;
            GameObject obj1 = Instantiate(prefab);
            obj1.transform.position = lastPosition;
            //change renderer material color
            obj1.GetComponent<MeshRenderer>().material.color = Color.red;
            listObjects.Add(obj1);
        }
        Debug.Log("CreateGameObjList: " + listObjects.Count);
    }

    public void ChangeObjColor()
    {
        for (int i = 0; i < listObjects.Count; i++)
        {
            float distance = Vector3.Distance(listObjects[i].transform.position, listObjects[i + 1].transform.position);

            if (distance < 0.5)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = Color.red;
            }
            else if (distance < 0.8)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = new Color(255,69,0);
            }
            else if (distance < 1.3)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = Color.yellow;
            }
            else if (distance < 1.5)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = new Color(4, 2, 115);
            }
            else if (distance < 1.8)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = new Color(144, 213, 255);
            }
            else if (distance < 2)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = new Color(6, 64, 43);
            }

            else
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = Color.green;
            }
        }
    }

    public void EmptyGameObjList()
    {
        foreach (GameObject o in listObjects)
        {
            DestroyImmediate(o);
        }

        listObjects.Clear();
        Debug.Log("EmptyGameObjList: " + listObjects.Count);
    }

    public void CreateArrayOfVectors()
    {
        for (int i = 0; i < data.Length; i++)
        {
            dataPosition[i] = data[i].playerPos;
        }
        Debug.Log("creating vecxtor3 array Positions");
    }

    public void DrawLines()
    {
        
        Debug.Log("Making lines");
        lineRenderer.positionCount = data.Length;
        lineRenderer.SetPositions(dataPosition);
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.red;
    
       
    }

    public void DeleteLines()
    {

        Debug.Log("Deleting lines");

        lineRenderer.positionCount = 0;

    }

    public void ReadJson(string name)
    {
        name = Path.GetDirectoryName(Application.dataPath) + "/Assets/HeatMap/SessionData/Level0.json";
        string fileString = File.ReadAllText(name);
        data = JsonUtility.FromJson<Wrapper>(fileString).dataContainers;
        Debug.Log("ReadJson: " + data.Length);

    }

    






}
