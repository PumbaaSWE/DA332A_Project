using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxItem : MonoBehaviour
{
    public GameObject cube;
    public void ChangeColor()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    public void SetActiveCubes()
    {
        cube.SetActive(true);
    }

    public void SetUnActiveCubes()
    {
        cube.SetActive(false);
    }

}
