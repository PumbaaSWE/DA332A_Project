using UnityEngine;


[RequireComponent(typeof(Health), typeof(FlareThrower))]
public class Player : MonoBehaviour
{
    [SerializeField] PlayerDataSO playerData;
    [SerializeField] Transform playerHead;
    public Vector3 LookDir => playerHead.forward;
    public Vector3 HeadPos => playerHead.position;
    static bool created;
    static int instances;

    public Health Health { get; private set; }

    void Awake()
    {


        instances++;
        //Debug.Assert(!created, "Player - multiple created/loaded");
        //A kind of singelton player...
        if (created)
        {
            //Debug.Log("Destroying duplicate and disabling audio");
            GetComponentInChildren<AudioListener>().enabled = false;
            Destroy(gameObject);
            return;
        }
        instances = 1;
        Health = GetComponent<Health>();
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
        instances--;
        created = instances > 0;
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
