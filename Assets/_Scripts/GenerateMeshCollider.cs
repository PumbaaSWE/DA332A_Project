using UnityEngine;


/**
 * To help create MeshColliders for game objects that need them for bullet collision or other fine details
 * Generally colliders should be low detail(sphere, box, capsule etc) to be fast
 * but sometimes we need the detail and this could add it while keeping the low detail for large object such as 
 * enemies, players and stuff
 * 
 * Maybe certain objects need special cases for flares and such... future issue
 * 
 * -Jack
 * 
 * 
*/
public class GenerateMeshCollider : MonoBehaviour
{
    [Tooltip("HighDetailLayer that is applied to child (of applyTo) as its layer")]
    [SerializeField] LayerMask highDetailLayer;
    [Tooltip("Should LowDetailLayer be applied to applyTo GameObject as its layer?")]
    [SerializeField] bool applyLowDetailLayer = true;
    [Tooltip("LowDetailLayer that is applied to applyTo GameObject as its layer")]
    [SerializeField] LayerMask lowDetailLayer;
    [Tooltip("The GameObject applyTo is the gameobject that will get a child with a meshCollider on it with layer HighDetailLayer. If null then gameObject this is attached to is used.")]
    [SerializeField] GameObject applyTo;


    [MakeButton]
    public void GenerateHighDetailAsChild()
    {
        if(!applyTo) applyTo = gameObject;
        GenerateHighDetailAsChild(applyTo);
    }

    public void GenerateHighDetailAsChild(GameObject parent)
    {
        MeshCollider meshCollider = parent.AddComponent<MeshCollider>();
        Mesh mesh = meshCollider.sharedMesh;
        DestroyImmediate(meshCollider);
        if(applyLowDetailLayer) parent.layer = ToLayer(lowDetailLayer.value);
        GameObject go = new GameObject(parent.name + "_HighDetailCollider");
        go.isStatic = parent.isStatic;
        Transform t = go.transform;
        t.parent = parent.transform;
        //t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        t.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        meshCollider = go.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        go.layer = ToLayer(highDetailLayer.value);
        Debug.Log("highDetailLayer = " + highDetailLayer + ", highDetailLayer.value = " + highDetailLayer.value);
        Debug.Log("TwoPowerOf(highDetailLayer.value) = " + ToLayer(highDetailLayer.value) );
    }

    /// <summary> Converts given bitmask to layer number </summary>
    /// <returns> layer number </returns>
    public static int ToLayer(int bitmask)
    {
        int result = bitmask > 0 ? 0 : 31;
        while (bitmask > 1)
        {
            bitmask >>= 1;
            result++;
        }
        return result;
    }

}
