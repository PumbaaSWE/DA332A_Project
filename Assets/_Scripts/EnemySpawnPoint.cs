using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;


    public void OnDrawGizmosFromParent(Mesh mesh, Quaternion offset, float scale)
    {
        //GizmosExtra.DrawArrow(Position, Rotation);
        Gizmos.DrawWireMesh(mesh, 0, Position, Rotation * offset, Vector3.one * scale);
    }

    //public bool ValidateSpawn(NavMeshSurface navMeshSurface)
    //{
    //    //navMeshSurface.
    //    return true;
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        GizmosExtra.DrawArrow(Position, Rotation);
        //Gizmos.DrawWireCube(Position + Vector3.up, new Vector3(.5f, 2, .5f)); 
        //Gizmos.DrawWire
    }
}
