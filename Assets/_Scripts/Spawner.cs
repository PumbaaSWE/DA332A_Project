using System.Collections;
using UnityEngine;
/// <summary>
/// Simple Spawner class to spawn various game objects
/// </summary>
public class Spawner : MonoBehaviour
{
    public GameObject PrefabToSpawn;
    [Tooltip("Start spawn timer when the Start method is called")]
    public bool spawnOnStart = true;
    [Tooltip("Delay for X amount of seconds before spawning")]
    public float spawnDelay = 0;
    public enum BlockedBehaviour { ForceSpawn, WaitThenSpawn, DontSpawn };
    [Tooltip("What do we do when something is inside the spawn volume?")]
    public BlockedBehaviour blockedBehaviour;

    [Tooltip("Size of volume to check for overlap")]
    public Vector3 blockedVolume = new Vector3(1, 2, 1);
    [Tooltip("Green is clear, red is blocked")]
    public bool showBlockBounds = true;

    static Mesh cube;


    static readonly Vector3[] arrow = new Vector3[]
    {
        new Vector3(  0,    0.01f,   1),
        new Vector3( .5f,   0.01f,  .5f),
        new Vector3( .3f,   0.01f,  .5f),
        new Vector3( .3f,   0.01f,  .0f),
        new Vector3(-.3f,   0.01f,  .0f),
        new Vector3(-.3f,   0.01f,  .5f),
        new Vector3(-.5f,   0.01f,  .5f),
    };

    static readonly Vector3[] arrowTransformed = new Vector3[7];

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        if (spawnOnStart)
        {
            SpawnWithDelay(spawnDelay);
        }
    }
    /// <summary>
    /// Will spawn the GameObject after seconds
    /// </summary>
    /// <param name="seconds"></param>
    public void SpawnWithDelay(float seconds)
    {
        if(seconds <= 0)
        {
            SpawnImmidiate();
        }
        else
        {
            StartCoroutine(SpawnIn(seconds));
        }
    }
    /// <summary>
    /// Will spawn the GameObject right away
    /// </summary>
    public void SpawnImmidiate()
    {
        if (PrefabToSpawn)
        {
            switch (blockedBehaviour)
            {
                case BlockedBehaviour.ForceSpawn:
                    Instantiate(PrefabToSpawn, transform.position, transform.rotation);
                    break;
                case BlockedBehaviour.WaitThenSpawn:
                    StartCoroutine(SpawnInWhenCleared());
                    break;
                case BlockedBehaviour.DontSpawn:
                    //callback?????
                    break;
                default:
                    break;
            }

            
        }
    }

    IEnumerator SpawnIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SpawnImmidiate();
    }

    IEnumerator SpawnInWhenCleared()
    {
        yield return new WaitUntil(()=>!CheckIfBlocked());
        Instantiate(PrefabToSpawn, transform.position, transform.rotation);
    }

    void CreateMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[8];
        float a = .5f;
        vertices[0] = new Vector3(-a,   -a,      a);
        vertices[1] = new Vector3( a,   -a,      a);
        vertices[2] = new Vector3(-a,    a,      a);
        vertices[3] = new Vector3( a,    a,      a);
        vertices[4] = new Vector3(-a,   -a,     -a);
        vertices[5] = new Vector3( a,   -a,     -a);
        vertices[6] = new Vector3(-a,    a,     -a);
        vertices[7] = new Vector3( a,    a,     -a);
        mesh.vertices = vertices;



        int[] indices = new int[] {
            //Top
            2, 6, 7,
            2, 3, 7,

            //Bottom
            0, 4, 5,
            0, 1, 5,

            //Left
            0, 2, 6,
            0, 4, 6,

            //Right
            1, 3, 7,
            1, 5, 7,

            //Front
            0, 2, 3,
            0, 1, 3,

            //Back
            4, 6, 7,
            4, 5, 7
        };

        mesh.triangles = indices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        cube = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Will return true if there is a non-trogger collider in the spawn volume
    /// </summary>
    /// <returns></returns>
    public bool CheckIfBlocked()
    {
        return Physics.CheckBox(transform.position + transform.up * blockedVolume.y / 2, blockedVolume*.499f, transform.rotation, ~0, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmos()
    {
        if (showBlockBounds)
        {
            Gizmos.color = CheckIfBlocked() ? Color.red : Color.green;

            //Gizmos.DrawWireCube(transform.position + Vector3.up, new Vector3(.5f, 2, .5f));
            if (!cube)
            {
                CreateMesh();
            }
            else
            {
                Gizmos.DrawWireMesh(cube, transform.position + transform.up * blockedVolume.y/2, transform.rotation, blockedVolume);
                //Gizmos.
            }
        }
        

        Gizmos.color = Color.yellow;
        transform.TransformPoints(arrow, arrowTransformed);
        Gizmos.DrawLineStrip(arrowTransformed, true);
    }
}
