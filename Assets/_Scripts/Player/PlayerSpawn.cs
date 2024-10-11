using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSpawn : MonoBehaviour
{
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    
    
    [MakeButton]
    public void AttachToGround()
    {
        if(Physics.Raycast(transform.position + transform.up, -transform.up, out RaycastHit hit, 200))
        {
            transform.position = hit.point;
            transform.MatchUp(hit.normal);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        GizmosExtra.DrawArrow(transform);
    }
}
