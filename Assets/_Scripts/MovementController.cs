using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementController : MonoBehaviour
{
    public Vector2 LookDelta {  get; protected set; }

    /// <summary>
    /// Rotate player camera by <paramref name="x"/> (pitch) and <paramref name="y"/> (yaw) degrees.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public abstract void Rotate(float x, float y);
}
