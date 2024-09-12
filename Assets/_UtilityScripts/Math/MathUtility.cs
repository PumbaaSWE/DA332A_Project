using UnityEngine;

public static class MathUtility
{
    public const float Epsilon = 0.0001f;

    /// <summary>
    /// MathUtility Mathf.Abs(f) < MathUtility.Epsilon
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static bool IsZero(this float f) => Mathf.Abs(f) < Epsilon;
    public static bool IsApprox(this float f, float a) => Mathf.Abs(f - a) < Epsilon;

    /// <summary>
    /// Set angle in degrees between -180 to 180, like clamp but for angles
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float FixAngle(float angle)
    {
        return Mathf.Repeat(angle + 180f, 360f) - 180f;
    }


    public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return Mathf.Sqrt(SqDistancePointLine(point, lineStart, lineEnd));
    }

    /// <summary>
    /// Computes the closest point to specified point on line segement defined by lineStart and lineEnd.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="lineStart"></param>
    /// <param name="lineEnd"></param>
    /// <returns>a Vector3 that is the closest point on the line segment</returns>
    public static Vector3 ClosestPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 ab = lineEnd - lineStart;
        if (ab.sqrMagnitude < Epsilon) return lineStart; //in case the line segment is degenerate 
        float t = Vector3.Dot(point - lineStart, ab) / ab.sqrMagnitude;
        if (t < 0.0f) t = 0.0f;
        if (t > 1.0f) t = 1.0f;
        return lineStart + t * ab;
    }

    /// <summary>
    /// Computes the distance squared to specified point on line segement defined by lineStart and lineEnd.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="lineStart"></param>
    /// <param name="lineEnd"></param>
    /// <returns>a float that is distance squared</returns>
    public static float SqDistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 ab = lineEnd - lineStart;
        Vector3 ap = point - lineStart;
        Vector3 bp = point - lineEnd;
        float e = Vector3.Dot(ap, ab);
        // Handle cases where the point projects outside ab
        if (e <= 0.0f) return Vector3.Dot(ap, ap);
        float f = Vector3.Dot(ab, ab);
        if (e >= f) return Vector3.Dot(bp, bp);
        // Handle cases where the point projects onto ab
        return Vector3.Dot(ap, ap) - e * e / f;
    }
}