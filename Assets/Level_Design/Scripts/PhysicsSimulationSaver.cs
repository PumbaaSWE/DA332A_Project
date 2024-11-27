using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteAlways] // Ensures the script can execute in Edit Mode
public class PhysicsSimulationSaver : MonoBehaviour
{
    [SerializeField] private List<Transform> objects;

    public void Start()
    {
        objects.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            objects.Add(transform.GetChild(i));
        }

        RandomizeScales();
    }

    private void RandomizeScales()
    {
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                float randomScaleFactor = Random.Range(0.8f, 1.2f);
                obj.localScale *= randomScaleFactor;
            }
        }
    }

}