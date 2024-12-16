using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnCooldown = 5f;
    [SerializeField] private float minDistanceToPlayer = 10f;
    [SerializeField] private float maxDistanceToPlayer = 20f;
    private bool isSpawning = false;
    private bool canSpawn = false;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    public Transform secondSpawn;

    [SerializeField] Transform target;
    public PlayerDataSO player;
    public int spawnPool = 0;
    public int spawnCount = 0;

    private void Start()
    {
        
       
    }
    private void Update()
    {
        if(canSpawn)
        {
            Spawn();
        }
    }
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

    [MakeButton]
    public void AttachToGround()
    {
        if (Physics.Raycast(transform.position + transform.up, -transform.up, out RaycastHit hit, 200))
        {
            transform.position = hit.point;
            transform.MatchUp(hit.normal);
        }
    }
    public void EnemyDeath()
    {
        if (canSpawn)
        {
            spawnCount--;
        }
    }
    public void CanSpawn(bool isSpawning)
    {
        canSpawn = isSpawning;
    }

    public void Spawn()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.PlayerTransform.position);

        if (!isSpawning && distanceToPlayer <= maxDistanceToPlayer)
        {
            if(spawnCount < spawnPool)
            {
                StartCoroutine(SpawnEnemy());
            }
        }
    }

    private IEnumerator SpawnEnemy()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnCooldown);

        if (enemyPrefab != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.PlayerTransform.position);
            
            if (distanceToPlayer >= minDistanceToPlayer)
            {
                GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                GameObject enemy = Instantiate(enemyPrefab, secondSpawn.position, Quaternion.identity);
            }
           
            spawnCount++;
        }

        isSpawning = false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        GizmosExtra.DrawArrow(transform);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistanceToPlayer);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDistanceToPlayer);
    }

}
