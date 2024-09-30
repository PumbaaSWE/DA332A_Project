using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ElevatorDoors))]
public class ElevatorWarp : MonoBehaviour
{

    [SerializeField] Vector3 start = Vector3.zero;
    [SerializeField] Vector3 end = Vector3.zero;
    [MakeButton]
    public void SetStartPos()
    {
        start = transform.position;
    }
    [MakeButton]
    public void SetEndPos()
    {
        end = transform.position;
    }

    public void Warp(List<GameObject> objects)
    {
        Vector3 delta = end - start;
        if(delta.sqrMagnitude < 0.001f) return;


        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].transform.position += delta;
        }
        transform.position += delta;
        GetComponent<ElevatorDoors>().ComputeDoorPos();
    }
}
