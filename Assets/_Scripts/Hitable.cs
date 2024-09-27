using UnityEngine;
using UnityEngine.Events;

public class Hitable : MonoBehaviour
{

    public UnityEvent<Vector3, Vector3> unityEvent;
    public UnityEvent<Vector3, Vector3> meleeEvent;

    public void RecieveHit(Vector3 from, Vector3 point)
    {
        unityEvent?.Invoke(from, point);
    }

    internal void Melee(Vector3 from, Vector3 direction)
    {
        meleeEvent?.Invoke(from, direction);
    }
}
