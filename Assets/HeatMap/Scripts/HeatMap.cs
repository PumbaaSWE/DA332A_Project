using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
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

    float[,] colorValues;

    int[] convertedXes;
    int[] convertedZes;
    Vector2[,] convertedXZ;

    int[] intsX;
    int[] intsZ;


    public void InitializeComponents()
    {
        colorValues = new float[200,200];
        convertedXes = new int[200];
        convertedZes = new int[200];
        convertedXZ = new Vector2[200, 200];
        intsX = new int[200];
        CreateArrayOfVectors();
        //colorValues = InitializeColorValues(ints);
        lineRenderer = GetComponent<LineRenderer>();
        listObjects = new List<GameObject>();
        dataPosition = new Vector3[data.Length];
        fileName = file.name;
        //Debug.Log("file Name getting from json file.name is: " + fileName);
        DirectoryInfo parentDirectory = Directory.GetParent(fileName);
        fileParentName = parentDirectory.Name;
        //Debug.Log("directory filePARENT name:" + fileParentName);
        filePath = Path.GetDirectoryName(Application.dataPath) + "/Assets/HeatMap/SessionData/" + fileParentName + "/ " + fileName + ".json";
        CreateTexture(convertedXZ);
        //Debug.Log("Initializing");
       


        
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
        //Debug.Log("creating vecxtor3 array Positions");
        //convertedXes = ConvertX(dataPosition);
        //convertedZes = ConvertZ(dataPosition); //make code
        convertedXZ = ConvertXZ(dataPosition);

        //intsX = HowManyIntsX(convertedXes);
        //intsZ = HowManyIntsZ(convertedZes); //make code
        //Debug.Log(intsX.Length);
        /*
        for (int i = 0;i < intsX.Length;i++)
        {
            Debug.Log(intsX[i]);
        }
        */
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

    /*
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
                //Debug.Log("Color is: " + a);
                Color color = new Color(a, a, a);
                texture.SetPixel(i, j, color);
            }
        }
        

        texture.filterMode = FilterMode.Point;
        texture.Apply();
        Renderer rnd = plane.GetComponent<Renderer>();
        rnd.sharedMaterial.mainTexture = texture;
    }
    */
    public void CreateTexture(Vector2[,] values)
    {
        var texture = new Texture2D(200, 200);
        Color color = Color.white;
        
        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < 200; j++)
            {
                if (values[i,j].x == 0)
                {
                    color = Color.grey;
                }
                if(values[i, j].x == 1)
                {
                        color = Color.green;
                }
               if(values[i, j].x == 2)
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

    public int[] ConvertX(Vector3 [] data)
    {
        int[] intArray = new int[data.Length];
        for(int i = 0; i <data.Length; i++)
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

    public Vector2[,] ConvertXZ(Vector3[] data)
    {
        Vector2[,] intArray = new Vector2[200, 200];

        foreach (Vector3 position in data)
        {
            // Map position.x and position.z to the grid
            //int x = (int)(100 - position.x);
            int x = (int)(100-position.x);
            //int y = (int)(position.z + 100);
            int y = (int)(100 - position.z);

            // Ensure indices are within bounds
            if (x >= 0 && x < 200 && y >= 0 && y < 200)
            {
                // Increment the value at (x, y) by (1, 1)
                intArray[x, y] += new Vector2(1, 1);
                Debug.Log("Updated cell (" + x + ", " + y + "): " + intArray[x, y]);
            }
            else
            {
                Debug.LogWarning("Out-of-bounds position: " + position);
            }
        }
        /*
        foreach (Vector3 position in data)
        {
            int x = (int)(100 - position.x);
            int y = (int)(position.z + 100);

            for (int i = 0; i < 200; i++)
            {
                for (int j = 0; j < 200; j++)
                {
                    
                    if (x == i && y == j)
                    {
                        intArray[i, j] += new Vector2(1, 1);
                        Debug.Log("..." + intArray[i, j]);
                    }
                    //intArray[i, j] = new Vector2(x, y);
                    

                }
            }

        }
        */



        return intArray;
    }

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


    /*
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
    */

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






}
