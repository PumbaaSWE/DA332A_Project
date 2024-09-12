using UnityEngine;

public static class VectorExtensions
{
    
    
    public static bool IsZero(this Vector3 v)
    {
        return v.sqrMagnitude < MathUtility.Epsilon;
    }


    public static Vector3 WithX(this Vector3 v, float x = 0)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 WithY(this Vector3 v, float y = 0)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 WithZ(this Vector3 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }
    
    public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float ? z = null)
    {
        return new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
    }

    public static Vector3 Add(this Vector3 v, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(v.x + (x ?? 0), v.y + (y ?? 0), v.z + (z ?? 0));
    }

    public static bool InRangeOf(this Vector3 current, Vector3 target, float range)
    {
        return (current - target).sqrMagnitude <= range * range;
    }

    public static Vector3 Inverse(this Vector3 v)
    {
        return new Vector3(1.0f / v.x, 1.0f / v.y, 1.0f / v.z);
    }

    public static Vector3 Abs(this Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }

}
