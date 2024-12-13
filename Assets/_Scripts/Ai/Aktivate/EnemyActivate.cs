using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyActivate : MonoBehaviour
{
    public List<FSM_Walker> enemies = new List<FSM_Walker>();
    //public string enemyTag = "Enemy"; 

    public EnemySpawn spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AgroEnemies(); 
            this.gameObject.SetActive(false);
            if (spawner != null)
            {
                spawner.gameObject.SetActive(true);
            }
        }
    }

    public void AgroEnemies()
    {
        foreach (FSM_Walker enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }
}
