using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClimberAct : MonoBehaviour
{
    public List<FSMClimber> enemies = new List<FSMClimber>(); 
    //public string enemyTag = "Enemy"; 

    private void Start()
    {
       
      
       
    }

    // När spelaren går in i triggern
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AgroEnemies(); 
            this.gameObject.SetActive(false); 
        }
    }

    public void AgroEnemies()
    {
        // Gå igenom alla fiender i listan och aktivera dem
        foreach (FSMClimber enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }
}
