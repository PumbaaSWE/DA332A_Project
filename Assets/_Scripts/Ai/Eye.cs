using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : MonoBehaviour
{
    public GameObject normalcolor;
    public GameObject normalcolor2;
    public GameObject bColor;
    public GameObject bColor2;



    public void AngryEye()
    {
        bColor.SetActive(true);
        bColor2.SetActive(true);
        normalcolor.SetActive(false);
        normalcolor2.SetActive(false);
      
    }

    public void NormalEye()
    {
        bColor.SetActive(false);
        bColor2.SetActive(false);
        normalcolor.SetActive(true);
        normalcolor2.SetActive(true);
       
    }
}
