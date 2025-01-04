using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] grid;

    Vector3 origin;

    public GridMap(int width, int height, float cellSize, Vector3 origin)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;

        grid = new int[width, height];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                DisplayTextOnWorld(null, grid[x, y].ToString(), GetWorldPosition(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f, 20, Color.white, TextAnchor.MiddleCenter, TextAlignment.Left, 5000);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 1000f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 1000f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 1000f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 1000f);   
    }

    public void GetXZToGridXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - origin).z / cellSize);
    }

    public void SetValue(int x, int y, int value)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
            grid[x, y] = value;
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y;
        GetXZToGridXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
            return grid[x, y];
        else
            return 0;
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXZToGridXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + origin;
    }

    private TextMesh DisplayTextOnWorld(Transform parent, string text, Vector3 localPosition, int fontSize,
        Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.rotation = Quaternion.Euler(90, 0, 0);
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}
