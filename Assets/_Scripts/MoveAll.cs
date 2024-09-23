using UnityEngine;

public class MoveAll : MonoBehaviour
{
    MoveTowardsController[] controllers;
    void Start()
    {
        controllers = FindObjectsByType<MoveTowardsController>(FindObjectsInactive.Include ,FindObjectsSortMode.None);
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
