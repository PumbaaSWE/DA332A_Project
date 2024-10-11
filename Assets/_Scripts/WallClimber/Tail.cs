using UnityEngine;

public class Tail : MonoBehaviour
{

    public int length = 9;
    public LineRenderer lineRenderer;
    public Vector3[] points;
    public Quaternion[] rotations;
    public Vector3[] pointVelocity;

    public float targetDist = 1;
    public float smoothSpeed = 1;




    void Start()
    {
        lineRenderer.positionCount = length;
        points = new Vector3[length];
        pointVelocity = new Vector3[length];
        rotations = new Quaternion[length];
    }

    // Update is called once per frame
    void Update()
    {
        points[0] = transform.position;
        rotations[0] = transform.rotation;
        for (int i = 1; i < length; i++)
        {
            Vector3 targetPos = points[i-1] + (points[i] - points[i - 1]).normalized * targetDist;
            points[i] = Vector3.SmoothDamp(points[i], targetPos, ref pointVelocity[i], smoothSpeed);
            //rotations[i] = Quaternion.RotateTowards(rotations[i], rotations[i-1], 9);
        }
        lineRenderer.SetPositions(points);
    }
}
