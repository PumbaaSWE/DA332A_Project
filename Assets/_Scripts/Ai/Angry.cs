using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angry : MonoBehaviour
{
    public GameObject normalcolor;
    public GameObject normalcolor2;
    public GameObject bColor;
    public GameObject bColor2;
    public GameObject angrycolor;
    public GameObject angrycolor2;
    GOAPPlanner planner;
    Goal_Attack attack;
    Goal_Chase chase;
    Goal_Attack_W attackW;
    Goal_Chase_W chaseW;
    Goal_Stalk_W stalkW;
    private bool isAngry; 

    private void Awake()
    {
        planner = GetComponent<GOAPPlanner>();

        
        attack = GetComponent<Goal_Attack>(); 
        chase = GetComponent<Goal_Chase>();
        attackW = GetComponent<Goal_Attack_W>();
        chaseW = GetComponent<Goal_Chase_W>();
        stalkW = GetComponent<Goal_Stalk_W>();
    }

    private void Update()
    {

        if (planner.ActiveGoal == attack || planner.ActiveGoal == chase|| planner.ActiveGoal == attackW || planner.ActiveGoal == chaseW)
        {
            if (!isAngry) 
            {
                AngryEye();
                isAngry = true; 
            }
        }
        else if (planner.ActiveGoal == stalkW)
        {

                BleuEye();
                
            
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
        bColor.SetActive(false);
        bColor2.SetActive(false);
        normalcolor.SetActive(false);
        normalcolor2.SetActive(false);
        angrycolor.SetActive(true);
        angrycolor2.SetActive(true);
    }

    public void NormalEye()
    {
        bColor.SetActive(false);
        bColor2.SetActive(false);
        normalcolor.SetActive(true);
        normalcolor2.SetActive(true);
        angrycolor.SetActive(false);
        angrycolor2.SetActive(false);
    }
    public void BleuEye()
    {
        bColor.SetActive(true);
        bColor2.SetActive(true);
        angrycolor.SetActive(false);
        angrycolor2.SetActive(false);
        angrycolor.SetActive(false);
        angrycolor2.SetActive(false);
    }
}
