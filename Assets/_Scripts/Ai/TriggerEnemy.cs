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
            // Hämta fiendens script
            Goal_Stalk enemyScript = enemy.GetComponent<Goal_Stalk>();

            if (enemyScript != null)
            {
                // Anropa en metod på fiendens script eller ändra en variabel
                enemyScript.prio -= 10;
            }
        }

        // Deaktivera detta objekt (triggern)
        this.gameObject.SetActive(false);
    }
}
