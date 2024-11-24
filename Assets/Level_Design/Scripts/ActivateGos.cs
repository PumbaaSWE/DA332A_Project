using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGos : MonoBehaviour
{
    [SerializeField] GameObject[] gos;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [MakeButton]
    public void ActivateGOs()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
