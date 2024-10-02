using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angry : MonoBehaviour
{
    public GameObject normalcolor;
    public GameObject normalcolor2;
    public GameObject angrycolor;
    public GameObject angrycolor2;
    GOAPPlanner planner;
    Goal_Attack attack;
    Goal_Chase chase;
    private bool isAngry; // För att spåra nuvarande ögonstatus

    private void Awake()
    {
        planner = GetComponent<GOAPPlanner>();

        
        attack = GetComponent<Goal_Attack>(); 
        chase = GetComponent<Goal_Chase>();   
    }

    private void Update()
    {

        if (planner.ActiveGoal == attack || planner.ActiveGoal == chase)
        {
            if (!isAngry) 
            {
                AngryEye();
                isAngry = true; 
            }
        }
        else
        {
            if (isAngry) 
            {
                NormalEye();
                isAngry = false; 
            }
        }
    }

    public void AngryEye()
    {
        normalcolor.SetActive(false);
        normalcolor2.SetActive(false);
        angrycolor.SetActive(true);
        angrycolor2.SetActive(true);
    }

    public void NormalEye()
    {
        normalcolor.SetActive(true);
        normalcolor2.SetActive(true);
        angrycolor.SetActive(false);
        angrycolor2.SetActive(false);
    }
}
