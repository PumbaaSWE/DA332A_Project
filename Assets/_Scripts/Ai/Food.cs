using System.Collections;
using System.Collections.Generic;
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
                
                findFood.prio = 0; 
            }
          

            // Inaktivera detta spelobjekt
            this.gameObject.SetActive(false);
        }
    }


    void ActivateEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            // Hämta fiendens script
            Goal_Stalk enemyScript = enemy.GetComponent<Goal_Stalk>();

            if (enemyScript != null)
            {
                // Anropa en metod på fiendens script eller ändra en variabel
                enemyScript.prio -= 5;
            }
        }

        // Deaktivera detta objekt (triggern)
        this.gameObject.SetActive(false);
    }
}
