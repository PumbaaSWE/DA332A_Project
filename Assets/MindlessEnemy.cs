using System.Collections.Generic;
using UnityEngine;

public class MindlessEnemy : MonoBehaviour
{
    
    Body body;
    MoveTowardsController controller;
    LookAt lookAt;
    float timer;
    public Transform eye;
    public PlayerDataSO player;
    [SerializeField] Transform target;
    SphereCollider sphereCollider;

    Vector3 avoid = Vector3.zero;
    List<Transform> avoids = new();

    void Start()
    {
        controller = GetComponent<MoveTowardsController>();
        lookAt = GetComponentInChildren<LookAt>();
        sphereCollider = GetComponent<SphereCollider>();
        player.NotifyOnPlayerChanged(OnPlayer);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    ////Debug.Log("Touching" + other.gameObject.name);
    //    //if(other.TryGetComponent(out Flare flare))
    //    //{
    //    //    Debug.Log("Touching" + other.gameObject.name);
    //    //    Vector3 delta = other.transform.position - transform.position;
    //    //    avoids.Add(other.transform);
    //    //    avoid += delta;
    //    //}
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Flare _))
        {
            //Debug.Log("Touching" + other.gameObject.name);
            avoids.Add(other.transform);
        }
    }

    private void OnDestroy()
    {
        player.UnsubscribeOnPlayerChanged(OnPlayer);
    }

    private void OnPlayer(Transform obj)
    {
        target = obj;
        if (target)
        {
            //do if not null
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            //Vector3 delta = eye.position - transform.position+Vector3.up;

            //if(Physics.Raycast(eye.position, delta, out RaycastHit hit, 100, ~0, QueryTriggerInteraction.Ignore))
            //{
            //    if(hit.collider.transform == target)
            //    {
            //        controller.SetTarget(target.position);
            //        lookAt.lookAtTargetPosition = target.position;
            //    }
            //    else
            //    {

            //    }
            //}
            //else
            //{

            //}
            avoids.RemoveAll(a => a == null);

            for (int i = 0; i < avoids.Count; i++)
            {
                Transform t = avoids[i];
                Vector3 delta = t.position - transform.position;
                if (delta.sqrMagnitude < sphereCollider.radius * sphereCollider.radius)
                {
                    avoid -= t.position - transform.position;
                }
            }

            if(avoid != Vector3.zero)
            {
                controller.SetTarget(avoid + transform.position);
            }
            else
            {
                controller.SetTarget(target.position);

            }
            avoid = Vector3.zero;
            lookAt.lookAtTargetPosition = target.position;
        }
        else
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                timer = 10;
                Vector3 pos = transform.position + Random.onUnitSphere * 10;
                controller.SetTarget(pos);
                lookAt.lookAtTargetPosition = pos;
            }
        }
    }
}
