using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEditor.PlayerSettings;

public class MoveTowardsController : Controller
{

    public Vector3 point;
    [SerializeField]private float minDist = .1f;
    [SerializeField] private float turnSpeed = 1;
    public WallClimber wallClimber;
    public float stoppingDistance = 0.5f;

    [SerializeField] private LayerMask selfLayer = 0;

    static readonly Collider[] others = new Collider[20];
    public float radius = 1.5f;
    public float avoidStrength = 1.0f;


    void Awake()
    {
        wallClimber = GetComponent<WallClimber>(); 
        point = transform.position;

    }


    public void SetTarget(Vector3 point)
    {
        this.point = point;

    }

    //private Vector3 ToVector3((int x, int y, int z) i)
    //{
    //    return new Vector3(i.x, i.y, i.z);
    //}

    
    void Update()
    {
        Vector3 delta = point - transform.position;



        
        if(delta.sqrMagnitude <= minDist * minDist)
        {
            move = Vector2.zero;
            look = Vector2.zero;
            return;
        }
        else
        {
            move = Vector2.up;
        }


        Vector3 dir = delta.normalized;

        dir += Avoid()*avoidStrength;

        float angle = Vector3.SignedAngle(transform.forward, dir, transform.up);

        

        if (Mathf.Abs(angle) < 0.01f)
        {
            angle = 0;
        }

        float x = Mathf.Clamp(angle, -1, 1) * wallClimber.Speed * turnSpeed;
        //float x = Mathf.MoveTowards(0, angle, wallClimber.Speed * turnSpeed * Time.deltaTime);

        look = new Vector2(Mathf.Clamp(x, -1, 1), 0);

    }
   
   
    private Vector3 Avoid()
    {
        Vector3 pos = transform.position;
        Vector3 avgPos = Vector3.zero;
        int n = Physics.OverlapSphereNonAlloc(transform.position, radius, others, selfLayer, QueryTriggerInteraction.Collide);
        if(n==1)return Vector3.zero;
        for (int i = 0; i < n; i++)
        {
            if (others[i].transform == transform) continue;
            //Debug.Log("Self");
            Vector3 otherPos = others[i].transform.position;
            Vector3 d = otherPos - pos;
            avgPos -= d * 1 / d.sqrMagnitude;
            Debug.DrawLine(otherPos, pos, Color.blue);
        }

        return avgPos / (n - 1);
        //Vector3 d = avgPos / n;
        //return Vector3.SignedAngle(transform.forward, d, transform.up);
    }

    public void ResetTarget()
    {
        point = transform.position;
        move = Vector2.zero;
        look = Vector2.zero;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, point);
    }
}
