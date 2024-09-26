using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavigation : MonoBehaviour
{
    
    public NavMeshAgent agent;
    Action<Vector3[]> pathCallback;
    bool computing;

    public int q = 0;
    //NavMeshPath path;

    record PathQueueItem
    {
        public Vector3 start;
        public Vector3 end; 
        public Action<Vector3[]> pathCallback;

        public PathQueueItem(Vector3 start, Vector3 end, Action<Vector3[]> pathCallback)
        {
            this.start = start;
            this.end = end;
            this.pathCallback = pathCallback;
        }
    }

    Queue<PathQueueItem> pathQueue = new();
    
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        
    }

    public void Pathfind(Vector3 start, Vector3 end, Action<Vector3[]> pathCallback)
    {
        pathQueue.Enqueue(new PathQueueItem(start, end, pathCallback));
        q = pathQueue.Count;
    }

    public bool ComputePath(Vector3 pos)
    {
        if(computing) return false;
        computing = true;
        agent.SetDestination(pos);
        return true;
    } 

    public void SetPos(Vector3 pos)
    {
        agent.Warp(pos);
    }

    // Update is called once per frame
    void Update()
    {
        if (!computing)
        {
            if (pathQueue.TryDequeue(out PathQueueItem item))
            {
                if (!agent.Warp(item.start))
                {
                    //Debug.Log("Failed warp to");
                    Debug.DrawLine(item.start, item.start + Vector3.up * 6, Color.magenta, 3f);
                }
                computing = true;
                agent.SetDestination(item.end);
                pathCallback = item.pathCallback;
                q = pathQueue.Count;
            }
        }
        //if (agent.pathStatus is NavMeshPathStatus.PathComplete or NavMeshPathStatus.PathPartial)
        //{
        //    computing = false;
        //    pathCallback?.Invoke(agent.path.corners);
        //}

        if (agent.hasPath)
        {
            computing = false;
            pathCallback?.Invoke(agent.path.corners);
        }


    }

    private void OnDrawGizmos()
    {
        if(!agent) return;
        Vector3[] points = agent.path.corners;
        if (points == null) return;
        Vector3 p = agent.transform.position;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawSphere(points[i], .1f);
            Gizmos.DrawLine(p, points[i]);
            p = points[i];

        }
    }
}
