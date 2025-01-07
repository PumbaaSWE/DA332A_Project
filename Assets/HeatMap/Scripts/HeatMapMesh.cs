using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.Rendering.DebugUI.Table;

public class HeatMapMesh : MonoBehaviour
{
    GridMap grid;
    Mesh mesh;
    //Material mat;

    public GridMap Grid { get => grid; set => grid = value; }

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
    }

    public void UpdateGridMesh(GridMap grid)
    {
        this.grid = grid;
        //grid.AddValue(1, 1, 10);
        //grid.SetValue(1, 1, 90);
        UpdateMesh();

        grid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, GridMap.OnGridValueChangedEventArgs e)
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        //Creates new vertices etc for mesh
        Vector3[] vertices = new Vector3[4 * (grid.Width * grid.Height)];
        Vector2[] uv = new Vector2[4 * (grid.Width * grid.Height)];
        int[] triangles = new int[6 * (grid.Width * grid.Height)];

        //Cycles through grid to update mesh for heatmap
        for(int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                int i = x * grid.Height + y;
                //0.99f for z axis alignment purposes
                Vector3 quadSize = new Vector3(1, 1, 0.99f) * grid.CellSize;

                int gridValue = grid.GetValue(x, y);
                float gridValueNormalized = (float)gridValue / 100;
                Vector2 uvValue = new Vector2(gridValueNormalized, 0f);

                AddToMeshArrays(vertices, uv, triangles, i, grid.GetWorldPosition(x,y) + quadSize * 0.5f, 0f, quadSize, uvValue, uvValue);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    #region Mesh helper methods
    private Quaternion[] cachedQuaternionEulerArray;
    private void CacheQuaternionEuler()
    {
        if (cachedQuaternionEulerArray != null) 
            return;

        cachedQuaternionEulerArray = new Quaternion[360];

        for (int i = 0; i < 360; i++)
            cachedQuaternionEulerArray[i] = Quaternion.Euler(0, 0, i);
    }
    private Quaternion GetQuaternionEuler(float rotationFloat)
    {
        int rotation = Mathf.RoundToInt(rotationFloat);
        rotation = rotation % 360;

        if (rotation < 0)
            rotation += 360;

        if (cachedQuaternionEulerArray == null) 
            CacheQuaternionEuler();

        return cachedQuaternionEulerArray[rotation];
    }

    public void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
    {
        //Moves vertices
        int vIndex = index * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        baseSize *= .5f;

        bool skewed = baseSize.x != baseSize.z;
        if (skewed)
        {
            // Here, the rotation and positioning are along XZ, so y = 0
            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, 0f, baseSize.y);
            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, 0f, -baseSize.y);
            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, 0f, -baseSize.y);
            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, 0f, baseSize.y);
        }
        else
        {
            //Vertices placed on the XZ plane
            vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * new Vector3(baseSize.x, 0f, baseSize.y);
            vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * new Vector3(baseSize.x, 0f, baseSize.y);
            vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * new Vector3(baseSize.x, 0f, baseSize.y);
            vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * new Vector3(baseSize.x, 0f, baseSize.y);
        }

        //Moves UVs
        uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
        uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
        uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
        uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

        //Creates triangles
        int tIndex = index * 6;

        triangles[tIndex + 0] = vIndex0;
        triangles[tIndex + 1] = vIndex3;
        triangles[tIndex + 2] = vIndex1;

        triangles[tIndex + 3] = vIndex1;
        triangles[tIndex + 4] = vIndex3;
        triangles[tIndex + 5] = vIndex2;
    }
    #endregion
}
