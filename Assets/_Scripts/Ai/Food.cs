using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Food : MonoBehaviour
{
    private GameObject[] enemies;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
     
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

         
            ActivateEnemies();
        }
      
        else if (other.CompareTag("Enemy"))
        {
            
            Goal_Eat findFood = other.GetComponent<Goal_Eat>();

            if (findFood != null)
            {
                
                findFood.CurrentPriority -= 50; 
            }
          
            this.gameObject.SetActive(false);
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        enemies = GameObject.FindGameObjectsWithTag("Enemy");

    //        Spawners();
    //    }
    //    else if (collision.collider.CompareTag("Enemy"))
    //    {
    //        Goal_Eat findFood = collision.collider.GetComponent<Goal_Eat>();

    //        if (findFood != null)
    //        {
    //            findFood.CurrentPriority -= 50;
    //        }

    //        this.gameObject.SetActive(false);
    //    }
    //}


    void ActivateEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            Goal_Stalk enemyScript = enemy.GetComponent<Goal_Stalk>();

            if (enemyScript != null)
            {
                enemyScript.prio -= 5;
            }
        }

        this.gameObject.SetActive(false);
    }
}
