using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSpawner : MonoBehaviour
{

    public GameObject prefab;
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
            }
        }
    }
}
