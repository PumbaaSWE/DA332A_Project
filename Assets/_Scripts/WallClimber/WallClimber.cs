using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimber : MonoBehaviour
{

    public float acceleration = 50;
    public float friction = 5;
    public float rotationSpeed = 90;

    public float arcAngle = 180;
    public int arcResolution = 4;
    public LayerMask groundLayer = ~0;

    public float stickyDistance = 0.2f;
    private float fallVelocity = 0;
   // private bool grounded;

    private float rotationDelta;
    private Vector2 moveCommand;

    private float speed;
    private Vector2 velocity;

    public Vector2 Velocity { get => velocity; }
    public Vector3 Velocity3 { get => new Vector3(velocity.x, 0, velocity.y); }
    public float Speed { get => speed; }

    public Controller controller;
    public bool debugDraw;



    // Start is called before the first frame update
    void Start()
    {
       // grounded = true;
    }

    void Update()
    {

        moveCommand = controller.Move;
        rotationDelta = controller.Look.x;

        float dt = Time.deltaTime;
        ApplyVelocity(dt);
        Rotate(dt);
    }

    private void Rotate(float dt)
    {
        if(rotationDelta != 0)
        {
            transform.Rotate(0, rotationSpeed * dt * rotationDelta, 0);
            rotationDelta = 0;
        }
    }

    private void ApplyVelocity(float dt)
    {
        

        if (velocity == Vector2.zero)
            return;

        float arcRadius = speed * dt;
        Vector3 worldVelocity = transform.TransformVector(Velocity3);

        if (ExtraPhysics.ArcCast(transform.position, Quaternion.LookRotation(worldVelocity, transform.up), arcAngle, arcRadius, arcResolution, groundLayer, out RaycastHit hit))
        {
            Vector3 pos = transform.position;
            transform.position = hit.point;
            transform.MatchUp(hit.normal);
            fallVelocity = 0;
          //  grounded = true;
        }
        else
        {
            //grounded = false;
            if (Physics.Raycast(transform.position + 0.5f * stickyDistance * transform.up, -transform.up, out RaycastHit ghit, stickyDistance, groundLayer, QueryTriggerInteraction.Ignore))
            {
                //this is because we can get stuck in the air if something happen 
                //transform.position = ghit.point;
                //transform.MatchUp(ghit.normal);
            }
            else
            {
            //    grounded = false;
            }
        }

    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        ApplyFriction(dt);
        ApplyAcceleration(dt);
        //if (grounded)
        //{
        //    ApplyFriction(dt);
        //    ApplyAcceleration(dt);

        //}
        //else
        //{
        //    ApplyFalling(dt);
        //}
        speed = velocity.magnitude;
        //UpdateVeclocity();
    }

    private void ApplyFalling(float dt)
    {
        //do faling logic
        fallVelocity += 9.82f * dt; //do in fixed update :grimacing:

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, fallVelocity*dt+stickyDistance, groundLayer, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
            transform.MatchUp(hit.normal);
           // grounded = true;
            //Debug.Log("we landed");
        }
        else
        {
            //rotate upwards?
            //Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            //Quaternion desired = Quaternion.LookRotation(fwd, Vector3.up);

            //transform.rotation *= Quaternion.RotateTowards(transform.rotation, desired, 90 * dt);

            transform.position += dt * fallVelocity * Vector3.down;
        }

    }

    private void ApplyFriction(float dt)
    {
        velocity -= dt * friction * velocity;
    }

    private void ApplyAcceleration(float dt)
    {
        if (moveCommand != Vector2.zero)
        {
            velocity += acceleration * dt * moveCommand;
            moveCommand = Vector2.zero;
        }     
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, .2f);
        Gizmos.DrawLine(transform.position, transform.position + transform.up * .5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward);

    }
}
