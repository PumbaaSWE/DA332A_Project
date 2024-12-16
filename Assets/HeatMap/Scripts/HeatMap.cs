using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering;
using static HeatMapDataJson;

public class HeatMap : MonoBehaviour
{

    
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

    Vector2[,] convertedXZ;

   

    public void InitializeComponents()
    {
        ReadJson();
        dataPosition = new Vector3[data.Length];

        convertedXZ = new Vector2[200, 200];
        
        CreateEmptyTexture();
        
        lineRenderer = GetComponent<LineRenderer>();
        listObjects = new List<GameObject>();

        Create2DArrayOfVectors();
    }
    
   
    public void CreateCubes()
    {
        for (int i = 0; i < data.Length; i++)
        {
            var texture = new Texture2D(10, 10);
            Color color = Color.white;
            Color[] colors = new Color[10 * 10];
            for(int j = 0; j < 10; j++)
            {
                colors[j] = color;
            }

            Vector3 lastPosition = data[i].playerPos;
            GameObject obj1 = Instantiate(prefab);
            obj1.transform.position = lastPosition;
          

            texture.filterMode = FilterMode.Point;
            texture.Apply();
            Renderer rnd = obj1.GetComponent<Renderer>();
            texture.SetPixels(colors);
            rnd.sharedMaterial.mainTexture = texture;


            listObjects.Add(obj1);
        }
    }
    public void ChangeCubesColor()
    {
        var texture = new Texture2D(10, 10);
        Color color = Color.white;
        Color[] colors = new Color[10 * 10];
        

        for (int i = 0; i < listObjects.Count; i++)
        {
            if(i > listObjects.Count - 2)
            {
                break;
            }

            float distance = Vector3.Distance(listObjects[i].transform.position, listObjects[i + 1].transform.position);

            if (distance < 0.5)
            {
                color = Color.red;
            }
            else if (distance < 0.8)
            {
                //listObjects[i].GetComponent<MeshRenderer>().material.color = Color.yellow;
                color = Color.yellow;
            }
            else if (distance < 1.3)
            {
                //listObjects[i].GetComponent<MeshRenderer>().material.color = Color.cyan;
                color = Color.cyan;
            }
            else if (distance < 1.5)
            {
                //listObjects[i].GetComponent<MeshRenderer>().material.color = Color.blue;
                color = Color.blue;
            }
            else if (distance < 1.8)
            {
                //listObjects[i].GetComponent<MeshRenderer>().material.color = Color.green;
                color = Color.green;
            }
            else if (distance < 2)
            {
                //listObjects[i].GetComponent<MeshRenderer>().material.color = Color.grey;
                color = Color.grey;
            }

            else
            {
                //listObjects[i].GetComponent<MeshRenderer>().material.color = Color.black;
                color = Color.black;
            }
            for (int j = 0; j < 10; j++)
            {
                colors[j] = color;
            }

            texture.filterMode = FilterMode.Point;
            texture.Apply();
            Renderer rnd = listObjects[i].GetComponent<Renderer>();
            texture.SetPixels(colors);
            rnd.sharedMaterial.mainTexture = texture;
        }

        
    }
    public void DeleteCubes()
    {
        foreach (GameObject o in listObjects)
        {
            DestroyImmediate(o);
        }

        listObjects.Clear();
    }
    public void DrawLines()
    {
        lineRenderer.positionCount = data.Length;
        lineRenderer.SetPositions(dataPosition);
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.red;
    }
    public void DeleteLines()
    {
        lineRenderer.positionCount = 0;
    }
    public void CreateTexture()
    {
        var texture = new Texture2D(200, 200);
        Color color = Color.white;
        Vector2[,] values = convertedXZ;
        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < 200; j++)
            {
                if (values[i, j].x == 0)
                {
                    //color = Color.grey;
                    color = new Color(0f, 0f, 0f, 0f);
                }
                if (values[i, j].x == 1)
                {
                    color = Color.green;
                }
                if (values[i, j].x == 2)
                {
                    color = Color.blue;
                }
                if (values[i, j].x == 3)
                {
                    color = Color.red;
                }
                if (values[i, j].x >= 4)
                {
                    color = Color.black;
                }
                texture.SetPixel(i, j, color);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply();
        Renderer rnd = plane.GetComponent<Renderer>();
        rnd.sharedMaterial.mainTexture = texture;
    }

    //methods not btns
    public void ReadJson()
    {
        Debug.Log("calling readjson");

        //string fileString = File.ReadAllText(name);
        string fileString = file.text;
        data = JsonUtility.FromJson<Wrapper>(fileString).dataContainers;
    }
   
    public Vector2[,] ConvertXZ(Vector3[] data)
    {
        Vector2[,] intArray = new Vector2[200, 200];

        foreach (Vector3 position in data)
        {
            int x = (int)(100-position.x);
            int y = (int)(100 - position.z);

            if (x >= 0 && x < 200 && y >= 0 && y < 200)
            {
                intArray[x, y] += new Vector2(1, 1);
            }
            else
            {
                Debug.LogWarning("Out-of-bounds position: " + position);
            }
        }
               return intArray;
    }

    public void CreateEmptyTexture()
    {
        var texture = new Texture2D(200, 200);
        Color color = Color.white;
        Vector2[,] values = convertedXZ;
        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < 200; j++)
            {
                color = Color.grey;
                texture.SetPixel(i, j, color);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply();
        Renderer rnd = plane.GetComponent<Renderer>();
        rnd.sharedMaterial.mainTexture = texture;
    }

    public void Create2DArrayOfVectors()
    {
        for (int i = 0; i < data.Length; i++)
        {
            dataPosition[i] = data[i].playerPos;
        }
        convertedXZ = ConvertXZ(dataPosition);
    }

  


    //unused
    /*
    public int[] HowManyIntsX(int[] data)
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
    public int[] HowManyIntsZ(int[] data)
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
    private float[,] InitializeColorValues(int[] ints)
    {
        float[,] colorValues = new float[200, 200];
        //float valueX = 0;
        float valueY = 0;
        for (int i = 0; i < 200; i++)
        {
            valueY = 0f;
            for (int j = 0; j < 200; j++)
            {
                if (ints[i] > 0)
                {
                    colorValues[i, j] = 1f;
                    Debug.Log("x value is more than 0");
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
    public int[] ConvertX(Vector3[] data)
    {
        int[] intArray = new int[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            intArray[i] = (int)(100 - data[i].x);

        }

        return intArray;
    }
    public int[] ConvertZ(Vector3[] data)
    {
        int[] intArray = new int[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            intArray[i] = (int)(data[i].z + 100);
        }

        return intArray;
    }
    */



}
