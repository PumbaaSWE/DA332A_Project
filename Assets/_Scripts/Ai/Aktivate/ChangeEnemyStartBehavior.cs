using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeEnemyStartBehavior : MonoBehaviour
{
    public UnityEvent OnInteract;
    List<CharacterAgent> enemies;

    Goal_Stalk_W stalkGoal;


    public PlayerDataSO player;
    [SerializeField] Transform target;
   
   
    private void OnDestroy()
    {
        player.UnsubscribeOnPlayerChanged(OnPlayer);
    }
    private void OnPlayer(Transform obj)
    {
        target = obj;
        if (target)
        {

            //do if not null
        }
    }
    private void Start()
    {
        IdleEnemy();
        player.NotifyOnPlayerChanged(OnPlayer);
        
    }
    private void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AgroEnemies();
        }
        this.gameObject.SetActive(false);
    }


    public float detectionRadius = 10f; 
    public LayerMask enemyLayer;

    public void AgroEnemies()
    {
      
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

     
        foreach (Collider collider in hitColliders)
        {
           
            CharacterAgent agent = collider.GetComponent<CharacterAgent>();
            
            Goal_Stalk_W stalk = collider.GetComponent<Goal_Stalk_W>();
            Goal_Idle_W idle = collider.GetComponent<Goal_Idle_W>();

            if (agent != null && stalk != null)
            {

                agent.MoveTo(player.PlayerTransform.position);
                stalk.prio  -= 30;
                idle.Priority -= 90;
            }
          
        }
    }

    public void IdleEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);


        foreach (Collider collider in hitColliders)
        {

            CharacterAgent agent = collider.GetComponent<CharacterAgent>();

            Goal_Idle_W idle = collider.GetComponent<Goal_Idle_W>();


            if (agent != null && idle != null)
            {

                agent.MoveTo(player.PlayerTransform.position);
                idle.Priority += 90;
            }

        }

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); 
    }
}
