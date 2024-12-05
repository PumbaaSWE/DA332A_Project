using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static HeatMapDataJson;

public class HeatMap : MonoBehaviour
{

    //public Vector3[] positions; // read from Json and convert to array
    public DataContainer [] data;
    private string filePath;
    private string fileName;
    private string fileParentName;
    public TextAsset file;

    public GameObject prefab;
    private List<GameObject> listObjects;
    public Vector3[] dataPosition;

    LineRenderer lineRenderer;
   
    public GameObject plane;

    float[,] colorValues; //20 20

    int[] convertedXes;

    int[] ints;


    public void InitializeComponents()
    {
        colorValues = new float[200,200];
        convertedXes = new int[200];
        ints = new int[200];
        colorValues = InitializeColorValues(ints);
        lineRenderer = GetComponent<LineRenderer>();
        listObjects = new List<GameObject>();
        dataPosition = new Vector3[data.Length];
        fileName = file.name;
        Debug.Log("file Name getting from json file.name is: " + fileName);
        DirectoryInfo parentDirectory = Directory.GetParent(fileName);
        fileParentName = parentDirectory.Name;
        Debug.Log("directory filePARENT name:" + fileParentName);
        filePath = Path.GetDirectoryName(Application.dataPath) + "/Assets/HeatMap/SessionData/" + fileParentName + "/ " + fileName + ".json";
        CreateTexture();
        Debug.Log("Initializing");
       


        
    }

    public void CreateDataContainerArrayFromJson()
    {
        Debug.Log(filePath);
        ReadJson(filePath);
        Debug.Log("string filePath " + filePath.Length);
        Debug.Log(filePath);
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
            if(i > listObjects.Count - 2)
            {
                break;
            }

            float distance = Vector3.Distance(listObjects[i].transform.position, listObjects[i + 1].transform.position);

            if (distance < 0.5)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = Color.red;
            }
            else if (distance < 0.8)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = Color.yellow;
            }
            else if (distance < 1.3)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = Color.cyan;
            }
            else if (distance < 1.5)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = Color.blue;
            }
            else if (distance < 1.8)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else if (distance < 2)
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = Color.grey;
            }

            else
            {
                listObjects[i].GetComponent<MeshRenderer>().material.color = Color.black;
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
        convertedXes = ConvertX(dataPosition);
    
        ints = HowManyInts(convertedXes);
        Debug.Log(ints.Length);
        for (int i = 0;i < ints.Length;i++)
        {
            Debug.Log(ints[i]);
        }
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
        string fileString = File.ReadAllText(name);
        data = JsonUtility.FromJson<Wrapper>(fileString).dataContainers;
        Debug.Log("ReadJson: " + data.Length);

    }

    public void CreateTexture()
    {
        var texture = new Texture2D(200, 200);
        float a;
        //Color color = new Color(a, a, a);
        //Debug.Log(color);
        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < 200; j++)
            {
                //color = color.grayscale;
                a = colorValues[i, j];
                //Debug.Log(a);
                Color color = new Color(a, a, a);
                texture.SetPixel(i, j, color);
            }
        }
        

        texture.filterMode = FilterMode.Point;
        texture.Apply();
        Renderer rnd = plane.GetComponent<Renderer>();
        rnd.sharedMaterial.mainTexture = texture;
    }

    public int[] ConvertX(Vector3 [] data)
    {
        int[] intArray = new int[data.Length];
        for(int i = 0; i <data.Length; i++)
        {
            intArray[i] = (int)(100 - data[i].x);
           
        }

        return intArray;
    }

    public int[] HowManyInts(int[] data)
    {
        int[] ints = new int[200];
        for (int i = 0; i < 200; i++)
        {
            foreach (int x in data)
            {
                if (x == i)
                {
                    ints[i] += 1;
                }
            }
        }

        return ints;
    }



    /*
    public void CreateTexture()
    {
        var texture = new Texture2D(20, 20);
        float r = 0f;
        float g = 0f;
        float b = 0f;
        Color color = new Color(r, g, b);
        Debug.Log(color);
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                color = Color.red;

                texture.SetPixel(i, j, color);
            }
        }
        for (int i = 10; i < 15; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                color = Color.green;

                texture.SetPixel(i, j, color);
            }
        }
        for (int i = 15; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                color = Color.blue;

                texture.SetPixel(i, j, color);
            }
        }
        for (int i = 15; i < 20; i++)
        {
            for (int j = 10; j < 20; j++)
            {
                color = Color.black;

                texture.SetPixel(i, j, color);

            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        Renderer rnd = plane.GetComponent<Renderer>();
        rnd.sharedMaterial.mainTexture = texture;
    }
    */


    
    private float[,] InitializeColorValues(int[] valueX)
    {
        float[,] colorValues = new float[200, 200];
        //float valueX = 0;
        float valueY = 0;
        for (int i = 0; i < 200; i++)
        {
            valueY = 0f;
            for(int j = 0; j < 200; j++)
            {
                if (valueX[i] > 0)
                {
                    colorValues[i, j] = 1f;
                }
                else
                {
                    colorValues[i, j] = 0f;
                }
                
                //valueY+= 0.005f;
            }
            
        }
        return colorValues;
    }







}
