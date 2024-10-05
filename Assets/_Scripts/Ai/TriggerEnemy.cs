using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemy : MonoBehaviour
{
    private GameObject[] enemies;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
         
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            // Aktivera fienderna
            ActivateEnemies();
        }
    }

    void ActivateEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            // H�mta fiendens script
            Goal_Stalk enemyScript = enemy.GetComponent<Goal_Stalk>();

            if (enemyScript != null)
            {
                // Anropa en metod p� fiendens script eller �ndra en variabel
                enemyScript.prio -= 10;
            }
        }

        // Deaktivera detta objekt (triggern)
        this.gameObject.SetActive(false);
    }
}
