using UnityEngine;

public static class ExtraMath
{
    public static Quaternion RotationMatchUp(Quaternion rotation, Vector3 up) => Quaternion.FromToRotation(rotation * Vector3.up, up) * rotation;
    public static void MatchUp(this ref Quaternion rotation, Vector3 up) => rotation = RotationMatchUp(rotation, up);
    public static void MatchUp(this Transform transform, Vector3 up) => transform.rotation = RotationMatchUp(transform.rotation, up);
}
