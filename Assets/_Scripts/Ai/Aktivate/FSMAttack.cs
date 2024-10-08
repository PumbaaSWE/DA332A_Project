using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FSMAttack : MonoBehaviour
{
    public UnityEvent OnInteract;
    List<CharacterAgent> enemies;

    Goal_Stalk_W stalkGoal;
    List<FSM> fSMs;


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
        IdleEnemy();
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

            FSM fSM = collider.GetComponent<FSM>();

            if (fSM != null)
            {
                fSM.agentState = FSM.AgentState.Idle;
                fSM.MoveTo(player.PlayerTransform.position);
                
            }

        }
    }

    public void IdleEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);


        foreach (Collider collider in hitColliders)
        {

            //CharacterAgent agent = collider.GetComponent<CharacterAgent>();

            //Goal_Idle_W idle = collider.GetComponent<Goal_Idle_W>();
            FSM fSM = collider.GetComponent<FSM>();

            if (fSM != null )
            {
                if (fSM.agentState != FSM.AgentState.Sleep)
                {
                    fSM.agentState = FSM.AgentState.Sleep;
                }
               
               
            }

        }

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
