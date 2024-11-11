using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;

public class CaseEjector : MonoBehaviour
{
    [SerializeField] GameObject Casing;
    [SerializeField] float EjectionForce = 1;
    [SerializeField] float RotationalForce = 1;
    [SerializeField] int CasingLayer = 16;
    [SerializeField] LayerMask InteractWith;
    [SerializeField] float LifeTime = 5;
    [SerializeField] float Mass = 1;
    [SerializeField] float LinearDrag = 0.05f;
    [SerializeField] float AngularDrag = 0.05f;
    [SerializeField] PhysicMaterial PhysicMat;

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Eject()
    {
        GameObject newCasing = Instantiate(Casing,transform.position,transform.rotation);
        newCasing.name = "Casing Particle";
        newCasing.layer = CasingLayer;

        // Creates and sets the values for the collider
        MeshCollider collider = newCasing.AddComponent<MeshCollider>();
        collider.convex = true;
        collider.includeLayers = InteractWith;
        collider.material = PhysicMat;

        // Creates and sets the values for the rigidbody
        Rigidbody rb = newCasing.AddComponent<Rigidbody>();
        rb.mass = Mass;
        rb.drag = LinearDrag;
        rb.angularDrag = AngularDrag;
        rb.includeLayers = InteractWith;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        rb.AddForce(transform.right * EjectionForce, ForceMode.Impulse);
        rb.AddTorque(transform.up * RotationalForce, ForceMode.Impulse);

        Destroy(newCasing, LifeTime);
    }
}

//[CustomEditor(typeof(CaseEjector))]
//public class CaseEjectorInspectorUI : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//    }
//}

//public class LayerAttribute : PropertyAttribute { }