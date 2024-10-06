using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class DisableColliders : MonoBehaviour
{
    // Start is called before the first frame update
    List<MeshCollider> colliders = new();
    List<Rigidbody> rgs = new();
    void Start()
    {
        colliders = GetComponentsInChildren<MeshCollider>().ToList();
        rgs = GetComponentsInChildren<Rigidbody>().ToList();
        Invoke("DisableMesh", 3f);
    }

    // Update is called once per frame


    void DisableMesh()
    {
        foreach (var collider in colliders)
        {
            collider.enabled = false;
           
        }
        foreach (var collider in rgs)
        {
            collider.isKinematic = true;

        }
    }

}
