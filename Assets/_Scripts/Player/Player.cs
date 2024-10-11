using System;
using UnityEngine;


[RequireComponent(typeof(Health), typeof(FlareThrower))]
public class Player : MonoBehaviour
{
    [SerializeField] PlayerDataSO playerData;
    [SerializeField] Transform playerHead;
    public Vector3 LookDir => playerHead.forward;
    public Vector3 HeadPos => playerHead.position;
    static bool created;

    void Awake()
    {


        //Debug.Assert(!created, "Player - multiple created/loaded");
        //A kind of singelton player...
        if (created)
        {
            //Debug.Log("Destroying duplicate and disabling audio");
            GetComponentInChildren<AudioListener>().enabled = false;
            Destroy(gameObject);
            return;
        }

        gameObject.tag = "Player";
        playerData.PlayerTransform = transform;

        created = true;
    }
    public void EnableAudio()
    {
        //Debug.Log("Enabeling audio");
        GetComponentInChildren<AudioListener>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        created = false;
    }
#if UNITY_EDITOR
    [MakeButton]
    public void SetSpawnPoint()
    {
        PlayerSpawn spawn = FindFirstObjectByType<PlayerSpawn>();
        if(!spawn)
        {
            spawn = new GameObject("PlayerSpawnPoint").AddComponent<PlayerSpawn>();
        }
        spawn.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }


#endif
}
