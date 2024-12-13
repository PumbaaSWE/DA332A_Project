using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnCooldown = 5f;
    [SerializeField] private float minDistanceToPlayer = 10f;
    [SerializeField] private float maxDistanceToPlayer = 20f;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool isSpawning = false;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    [SerializeField] Transform target;
    public PlayerDataSO player;

    private void Update()
    {
        ActivateSpawner();
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

  

    public void ActivateSpawner()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.PlayerTransform.position);

        if (!isSpawning && distanceToPlayer >= minDistanceToPlayer && distanceToPlayer <= maxDistanceToPlayer)
        {
            StartCoroutine(SpawnEnemy());
        }
    }

    private IEnumerator SpawnEnemy()
    {
        isSpawning = true;
        yield return new WaitForSeconds(spawnCooldown);

        if (enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            spawnedEnemies.Add(enemy);
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
