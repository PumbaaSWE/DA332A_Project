using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] bool onStart = true;
    [SerializeField] ISpawnBehaviour spawn;
    [Header("Draw Gizmos Settings")]
    [SerializeField] bool drawMesh = true;
    [SerializeField] Quaternion rotationOffset;
    [SerializeField] float scaleMultiplier = 100;
    [SerializeField] Color color = Color.yellow;
    [SerializeField] Mesh mesh;

    void Start()
    {
        Debug.Assert(prefab, "EnemySpawner - please assign a GameObject-prefab to spawn");
        EnemySpawnPoint[] points = GetComponentsInChildren<EnemySpawnPoint>();
        foreach (var point in points)
        {
            GameObject go = Instantiate(prefab, point.Position, point.Rotation);
            spawn?.OnSpawn(go);
        }
    }

    [MakeButton]
    private void SetMeshFromPrefab()
    {
        if (prefab)
        {
            mesh = prefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            if (!mesh) mesh = prefab.GetComponentInChildren<MeshFilter>().sharedMesh;
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawMesh || !mesh) return;
        EnemySpawnPoint[] points = GetComponentsInChildren<EnemySpawnPoint>();
        Gizmos.color = color;
        foreach (var point in points)
        {
            point.OnDrawGizmosFromParent(mesh, rotationOffset, scaleMultiplier);
        }
    }

    //private void OnGUI()
    //{
    //    Debug.Log("OnGUI - sdeghfsxfgh");

    //    EnemySpawnPoint[] points = GetComponentsInChildren<EnemySpawnPoint>();
    //    foreach (var point in points)
    //    {
    //        Vector3 p = Handles.DoPositionHandle(point.Position, point.Rotation);
    //        point.transform.position = p;

    //        Quaternion q = Handles.DoRotationHandle(point.Rotation, point.Position);
    //        point.transform.rotation = q;
    //    }
    //}

}

internal interface ISpawnBehaviour
{
    void OnSpawn(GameObject obj);
}