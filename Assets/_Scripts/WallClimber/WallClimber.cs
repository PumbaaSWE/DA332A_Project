using UnityEngine;

public class WallClimber : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 4;
    [SerializeField] private float acceleration = 50;
    [SerializeField] private float friction = 5;
    [SerializeField] private float rotationSpeed = 90;
    [SerializeField] private float arcAngle = 180;
    [SerializeField] private int arcResolution = 4;
    [SerializeField] private LayerMask groundLayer = ~0;
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private Transform pivot;
    [SerializeField] private float pivotSpeed = 180;
    private Quaternion pivotParentLast;
    // private bool grounded;

    private float rotationDelta;
    private Vector2 moveCommand;

    private float speed;
    private Vector2 velocity;

    public Vector2 Velocity { get => velocity; }
    public Vector3 Velocity3 { get => new(velocity.x, 0, velocity.y); }
    public float Speed { get => speed; }

    [SerializeField] private Controller controller;
    public bool debugDraw;



    // Start is called before the first frame update
    void Start()
    {
        // grounded = true;
        if (!controller)
        {
            controller = GetComponent<Controller>();
        }

        pivotParentLast = transform.localRotation;
    }

    void Update()
    {

        moveCommand = controller.Move;
        rotationDelta = controller.Look.x;

        float dt = Time.deltaTime;
        ApplyVelocity(dt);
        Rotate(dt);
        HandlePivot(dt);
    }

    private void Rotate(float dt)
    {
        rotationDelta = float.IsNaN(rotationDelta) ? 0 : rotationDelta;
        if (rotationDelta != 0)
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

        Vector3 pos = transform.position;
        Debug.DrawLine(pos + transform.up * radius, pos + transform.up * radius + worldVelocity.normalized * (radius + arcRadius), Color.green);
        //this checks wall infront
        //if (Physics.Raycast(pos + transform.up * radius, worldVelocity, out RaycastHit hit, radius + arcRadius, groundLayer, QueryTriggerInteraction.Ignore))
        //{
        //    Debug.DrawLine(pos + transform.up * radius, hit.point, Color.red);
        //    transform.MatchUp(hit.normal);
        //    pivot.localRotation = Quaternion.identity;
        //    pivotParentLast = transform.localRotation;
        //    transform.position = hit.point;
        //}
        ////walk around convex edges
        //else
        if (ExtraPhysics.ArcCast(transform.position, Quaternion.LookRotation(worldVelocity, transform.up), arcAngle, arcRadius, arcResolution, groundLayer, out RaycastHit hit))
        {
            transform.position = hit.point;
            transform.MatchUp(hit.normal);
            //velocity = Vector3.zero;
            return;
        }

        EmergencyTeleportToGround();

    }

    private void EmergencyTeleportToGround()
    {
        Vector3 pos = transform.position;
        if (Physics.Raycast(pos, -transform.up, out RaycastHit hit, 10, groundLayer, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
            transform.MatchUp(hit.normal);
        }
        else if (Physics.Raycast(pos + Vector3.up, Vector3.down, out hit, 10, groundLayer, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
            transform.MatchUp(hit.normal);
        }
    }

    private void HandlePivot(float dt)
    {
        pivot.localRotation = Quaternion.Inverse(transform.localRotation) * pivotParentLast * pivot.localRotation;
        pivotParentLast = transform.localRotation;
        pivot.localRotation = Quaternion.RotateTowards(pivot.localRotation, Quaternion.identity, pivotSpeed * dt);
    }


    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        ApplyFriction(dt);
        ApplyAcceleration(dt);

        speed = velocity.magnitude;
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
            if (velocity.sqrMagnitude > maxSpeed * maxSpeed) velocity = velocity.normalized * maxSpeed;
            moveCommand = Vector2.zero;
        }
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw) return;
        Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(transform.position, .2f);
        //Gizmos.DrawLine(transform.position, transform.position + transform.up * .5f);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawRay(transform.position, transform.forward);
        GizmosExtra.DrawArrow(transform);


        Gizmos.DrawLine(transform.position, transform.position + transform.up * radius);
        Gizmos.DrawWireSphere(transform.position + transform.up * radius, .1f);

    }
}

