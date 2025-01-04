using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHeatMap : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float cellSize;

    // Start is called before the first frame update
    void Start()
    {
        GridMap gridMap = new GridMap(width, height, cellSize, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
