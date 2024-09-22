using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    
    private readonly List<SensorHit> sensors = new List<SensorHit>();
    public float sensorAngle = 180;
    public int sensorResolution = 4;
    public int sensorAmount = 4;
    public LayerMask sensorMask = ~0;
    public bool debugDraw;
    // Update is called once per frame
    void Update()
    {
        //Scan();
    }

    public List<SensorHit> Scan(bool drawGizmo = false)
    {
        sensors.Clear();
        ScanRing(sensorAmount, .4f, 1, drawGizmo);
        ScanRing(sensorAmount*2, .8f, .5f, drawGizmo);
        //ScanRing(8, .8f, .5f);
        return sensors;
    }

    private void ScanRing(int num, float dist, float weight, bool drawGizmo = false)
    {
        float angle = 360.0f / num;

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        for (int i = 0; i < num; i++)
        {
            if (ExtraPhysics.ArcCast(pos, rot, sensorAngle, dist, sensorResolution, sensorMask, out RaycastHit hit, drawGizmo))
            {
                sensors.Add(new SensorHit(hit.point, hit.normal, weight));
                rot *= Quaternion.Euler(0,angle,0);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!debugDraw) return;
        Gizmos.color = Color.red;
        Scan(true);
        for (int i = 0; i < sensors.Count; i++)
        {
            Gizmos.DrawSphere(sensors[i].Position, 0.05f); 
            Gizmos.DrawRay(sensors[i].Position, sensors[i].Normal);
        }
    }
}

public readonly struct SensorHit
{
    public readonly Vector3 Position;
    public readonly Vector3 Normal;
    public readonly float Distance;

    public SensorHit(Vector3 position, Vector3 normal, float distance)
    {
        this.Position = position;
        this.Normal = normal;
        this.Distance = distance;
    }
}
