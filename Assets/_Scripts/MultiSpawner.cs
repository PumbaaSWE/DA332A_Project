using UnityEngine;

public class MultiSpawner : MonoBehaviour
{

    public GameObject prefabToSpawn;
    public float spawnDelay = 0;
    public Spawner[] spawners;
    public bool spawnOnStart;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Collect Child Spawners")]
    public void CollectSpawners()
    {
        spawners = GetComponentsInChildren<Spawner>();
        if(spawners != null)
        {
            for (int i = 0; i < spawners.Length; i++)
            {
                spawners[i].spawnOnStart = spawnOnStart;
                spawners[i].spawnDelay = spawnDelay;
                spawners[i].prefabToSpawn = prefabToSpawn;
            }
        }
    }

    public void Spawn()
    {
        foreach (Spawner spawner in spawners)
        {
            //spawner.blockedBehaviour = Spawner.BlockedBehaviour.ForceSpawn;
            spawner.SpawnImmidiate();
        }
    }
}
