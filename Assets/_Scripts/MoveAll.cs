using UnityEngine;

public class MoveAll : MonoBehaviour
{
    AdvancedController[] controllers;
    void Start()
    {
        controllers = FindObjectsByType<AdvancedController>(FindObjectsInactive.Include ,FindObjectsSortMode.None);
    }

    public void SetTarget(Vector3 point)
    {
        foreach (var c in controllers)
        {
            c.SetTarget(point);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
