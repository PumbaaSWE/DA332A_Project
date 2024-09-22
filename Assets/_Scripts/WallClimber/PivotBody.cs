using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotBody : MonoBehaviour
{
    public Transform pivot;
    public GroundSensor sensor;
    public float rotationSpeed = 90;

    private Vector3 prevUp;

    void Start()
    {
        prevUp = pivot.up;
    }

    // Update is called once per frame
    void Update()
    {
        List<SensorHit> sensors = sensor.Scan();

        //Debug.Assert(sensors.Count > 0, "no sedfhsfgjhd");

        Vector3 up = sensors.Count == 0 ? Vector3.up : Vector3.zero;

        for (int i = 0; i < sensors.Count; i++)
        {
            up += sensors[i].Normal * sensors[i].Distance;
        }
        //pivot.forward = transform.forward;

        //Quaternion q = ExtraMath.RotationMatchUp(pivot.rotation, up.normalized);
        //pivot.rotation = Quaternion.RotateTowards(pivot.rotation, Quaternion.FromToRotation(pivot.rotation * Vector3.up, up), rotationSpeed * Time.deltaTime);

        prevUp = Vector3.RotateTowards(prevUp, up.normalized, rotationSpeed * Mathf.Deg2Rad * Time.deltaTime, 0);
        //prevUp = Vector3.Mo(prevUp, up.normalized, rotationSpeed*Mathf.Deg2Rad * Time.deltaTime, 0);  


        //pivot.MatchUp(prevUp);


        Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, prevUp);
        pivot.rotation = Quaternion.LookRotation(fwd, prevUp);


        //pivot.forward = transform.forward;

        //pivot.localEulerAngles.WithY(0);

        //pivot.MatchUp(up.normalized);

        //Debug.DrawRay(pivot.position, up, Color.yellow);
        //Debug.DrawRay(pivot.position, prevUp, Color.green);
        //Debug.DrawRay(pivot.position, pivot.up, Color.magenta);
    }
}
