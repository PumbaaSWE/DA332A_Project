using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PlayerSpawner");
        //GetComponent<Spawner>().SpawnImmidiate();
        Instantiate(new GameObject("Spawn!"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
