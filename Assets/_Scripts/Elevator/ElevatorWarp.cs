using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ElevatorDoors))]
public class ElevatorWarp : MonoBehaviour
{

    [SerializeField] Vector3 start = Vector3.zero;
    [SerializeField] Vector3 end = Vector3.zero;
    [SerializeField] Quaternion startRot = Quaternion.identity;
    [SerializeField] Quaternion endRot = Quaternion.identity;


    private void Start()
    {
        transform.SetPositionAndRotation(start, startRot);
    }

    


    [MakeButton]
    public void SetStartPos()
    {
        start = transform.position;
        startRot = transform.rotation;
    }
    [MakeButton]
    public void SetEndPos()
    {
        end = transform.position;
        endRot = transform.rotation;
    }

    public void Warp(List<GameObject> objects)
    {
        Vector3 delta = end - start;
       // Quaternion deltaRot = Quaternion.RotateTowards(startRot, endRot, 360);
        float angle = Quaternion.Angle(startRot, endRot);
        if (delta.sqrMagnitude < 0.001f) return;


        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].transform.position += delta;
        }
        transform.position = end;

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].transform.RotateAround(end, Vector3.up, angle);

        }
        transform.rotation = endRot;

        GetComponent<ElevatorDoors>().ComputeDoorPos();
    }

    [MakeButton]
    public void GotoStartPos()
    {
        transform.SetPositionAndRotation(start, startRot);
    }
    [MakeButton]
    public void GotoEndPos()
    {
        transform.SetPositionAndRotation(end, endRot);
    }

}
