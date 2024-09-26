using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedController : Controller
{
    public Vector3 point;
    [SerializeField] private float minDist = .1f;
    [SerializeField] private float minYDist = 1.1f;
    [SerializeField] private float turnSpeed = 1;
    public WallClimber wallClimber;

    [SerializeField] private LayerMask selfLayer = 0;

    static readonly Collider[] others = new Collider[20];
    public float radius = 1.5f;
    public float avoidStrength = 1.0f;

    public Transform target;

    private Vector3 targetDelta;
    EnemyNavigation navigation;
    PathQueue pathQueue = new();

    void Awake()
    {
        wallClimber = GetComponent<WallClimber>();
        point = transform.position;
        move = Vector2.zero;
        navigation = FindAnyObjectByType<EnemyNavigation>();
    }


    public void SetTarget(Vector3 point)
    {
        this.point = point;
        navigation.Pathfind(transform.position, point, SetPath);
    }

    //private Vector3 ToVector3((int x, int y, int z) i)
    //{
    //    return new Vector3(i.x, i.y, i.z);
    //}

    public void SetPath(Vector3[] points)
    {
        pathQueue.SetPath(points);
        if (pathQueue.HasNext())
        {
            point = pathQueue.Pop();
        }
    }


    void Update()
    {
        Vector3 delta = point - transform.position;

        Vector3 up = transform.up;
        float d = Vector3.Dot(up, delta);
        Vector3 flatDist = Vector3.ProjectOnPlane(delta, up);
        bool onCieling = Vector3.Dot(Vector3.up, delta) < 0;
        if (flatDist.sqrMagnitude < minDist * minDist)
        {
            //we are close flatWise
            if (Mathf.Abs(d) < minYDist + (onCieling ? 2 : 0))
            {
                // close y dist
                move = Vector2.zero;
                look = Vector2.zero;
                if (pathQueue.HasNext())
                {
                    point = pathQueue.Pop() + Vector3.up * (minYDist * Random.value);

                }
                return;
            }
            else
            {
                //we are close flat dist but we might be on roof or on a wall far away
                //start timer?
            }
        }

        


        Vector3 dir = delta.normalized;

        Vector3 avoid = Avoid();

        float speed = Mathf.Sign(Vector3.Dot(-avoid, transform.forward)) * .2f;

        move = Vector2.up * (0.9f + speed);

        //dir += Avoid() * avoidStrength;
        dir += avoid * avoidStrength;

        float angle = Vector3.SignedAngle(transform.forward, dir, up);



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
        Vector3 close = Vector3.zero;
        int n = Physics.OverlapSphereNonAlloc(transform.position, radius, others, selfLayer, QueryTriggerInteraction.Collide);
        if (n == 1) return Vector3.zero;
        for (int i = 0; i < n; i++)
        {
            if (others[i].transform == transform) continue;
            //Debug.Log("Self");
            Vector3 otherPos = others[i].transform.position;
            Vector3 d = otherPos - pos;
            close += pos - otherPos;
            avgPos -= d * 1 / d.sqrMagnitude;
            Debug.DrawLine(otherPos, pos, Color.blue);
        }
        return close / close.sqrMagnitude;
        //return avgPos / (n - 1);
        //Vector3 d = avgPos / n;
        //return Vector3.SignedAngle(transform.forward, d, transform.up);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, point);
        pathQueue.OnDrawGizmos();
    }
}

public class PathQueue
{
    public Vector3[] points;
    public int current;

    public void SetPath(Vector3[] points)
    {
        this.points = points;
        current = 0;
    }

    public bool IsValid()
    {
        return points != null;
    }

    public bool HasNext()
    {
        return IsValid() && current < points.Length;
    }

    public Vector3 Pop()
    {
        return points[current++];
    }

    public void OnDrawGizmos()
    {
        if(!IsValid())return;
        Vector3 p = points[0];
        Gizmos.color = Color.red;
        for (int i = 1; i < points.Length; i++)
        {
            Gizmos.DrawLine(p, points[i]);
            p = points[i];
        }
    }
}