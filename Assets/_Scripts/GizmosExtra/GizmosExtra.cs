using UnityEngine;

public static class GizmosExtra
{
    static readonly Vector3[] arrow = new Vector3[]
    {
        new Vector3(  0,    0.01f,   1),
        new Vector3( .5f,   0.01f,  .5f),
        new Vector3( .3f,   0.01f,  .5f),
        new Vector3( .3f,   0.01f,  .0f),
        new Vector3(-.3f,   0.01f,  .0f),
        new Vector3(-.3f,   0.01f,  .5f),
        new Vector3(-.5f,   0.01f,  .5f),
    };

    static readonly Vector3[] arrowTransformed = new Vector3[7];


    public static void DrawArrow(Transform transform)
    {
        transform.TransformPoints(arrow, arrowTransformed);
        Gizmos.DrawLineStrip(arrowTransformed, true);
    }

    public static void DrawArrow(Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < arrow.Length; i++)
        {
            arrowTransformed[i] = (rotation * arrow[i]) + position;
        }
        Gizmos.DrawLineStrip(arrowTransformed, true);
    }
}
