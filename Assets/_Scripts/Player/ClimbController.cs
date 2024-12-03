using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class ClimbController : MonoBehaviour
{
    [SerializeField] Transform head;

    [Header("Looking")]
    [SerializeField] float minLookUpAngle;
    [SerializeField] float maxLookUpAngle;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float camOffset;
    [SerializeField] float rotationSpeed;

    [Header("Movement")]
    [SerializeField] float acceleration;
    [SerializeField] float maxSpeed;
    [Tooltip("Only applies to horizontal movement")]
    [SerializeField] float drag;
    [SerializeField] float gravity;

    [Header("Collision")]
    [SerializeField] float radius;
    [Tooltip("Which layers will the player collide with?")]
    [SerializeField] LayerMask collideWith;
    [SerializeField] LayerMask climbOn;
    [Tooltip("To prevent float point errors, keep a low value (< 0.1) or it will behave weirdly")]
    [SerializeField] float skinWidth;
    [Tooltip("Max amount of iterations when collision checks and bouncing off walls")]
    [SerializeField] int maxBounces;

    [Header("IsGrounded")]
    [SerializeField] float radiusDiff;
    [SerializeField] float distDiff;

    [Header("Debugging")]
    [SerializeField] bool drawGizmos;
    [SerializeField] bool grounded;
    [SerializeField] Vector3 velocity;
    [SerializeField] float speed;

    public Vector2 LookDelta { get; private set; }

    public float Radius => radius;
    public float CamOffset => camOffset;

    // inputs
    Vector2 look;
    Vector2 move;

    // components
    CapsuleCollider cc;

    float xRot;
    bool wasGrounded;

    void Start()
    {
        cc = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        if (Time.deltaTime == 0)
        {
            look = Vector2.zero;
            return;
        }

        cc.radius = radius;
        cc.height = radius * 2;
        cc.center = Vector3.zero;

        head.localPosition = camOffset * Vector3.up;

        wasGrounded = grounded;
        grounded = IsGrounded();

        if (!grounded)
            SetUp(Vector3.up);

        Look();
        Move(Time.deltaTime);
    }
    void FixedUpdate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    bool IsGrounded()
    {
        float r = cc.radius;
        //bool grounded = Physics.SphereCast(transform.position, r + radiusDiff, -groundNormal, out RaycastHit hit, distDiff, collideWith, QueryTriggerInteraction.Ignore);
        bool grounded = Physics.SphereCast(transform.position + transform.up * skinWidth, r + radiusDiff, -transform.up, out RaycastHit hit, distDiff, collideWith, QueryTriggerInteraction.Ignore);

        //if (/*this.grounded && */!grounded)
        //{
        //    grounded = Physics.SphereCast(transform.position - transform.up * distDiff, r + radiusDiff, -transform.forward, out hit, distDiff, collideWith, QueryTriggerInteraction.Ignore);
        //}

        if (grounded)
        {
            SetUp(hit.normal);
            transform.position = hit.point + transform.up * radius;
        }

        return grounded;
    }

    float rTime;
    Vector3 startHeadUp;

    void Look()
    {
        Vector2 preRot = Rotation();
        look *= mouseSensitivity;
        Rotate(look.y, look.x);
        LookDelta = Rotation() - preRot;
        look = Vector2.zero;

        // Snap head rotation if transition should be finished
        if (rTime > rotationSpeed)
        {
            head.MatchUp(transform.up);
            return;
        }

        // Slerp head rotation when climbing onto new wall
        rTime += Time.deltaTime;
        head.MatchUp(Vector3.Slerp(startHeadUp, transform.up, rTime / rotationSpeed));
        head.localRotation = Quaternion.Euler(head.localRotation.eulerAngles.WithY());
    }

    void Move(float dt)
    {
        // Apply drag
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(velocity, transform.up);


        Vector3 dragForce = -horizontalVelocity * drag * dt;

        if (dragForce.sqrMagnitude > horizontalVelocity.sqrMagnitude)
            dragForce = -horizontalVelocity;

        velocity += dragForce;

        // Apply acceleration
        Vector3 wishDir = transform.right * move.x + transform.forward * move.y;
        Vector3 deltaVel = wishDir * acceleration * dt;
        deltaVel = deltaVel.normalized * Mathf.Min(deltaVel.magnitude, Mathf.Max(0, maxSpeed - Vector3.Dot(velocity, deltaVel.normalized)));
        velocity += deltaVel;

        if (!wasGrounded && grounded)
            velocity = horizontalVelocity;

        // Apply gravity
        if (!grounded)
            velocity += Vector3.down * gravity * dt;

        // Collide and slide algorithm
        Vector3 deltaPos = CollideAndRotate(velocity * dt, transform.position, 0, dt);
        speed = velocity.magnitude;

        if (grounded)
            velocity = Vector3.ProjectOnPlane(velocity, transform.up);

        // Set position
        transform.position += deltaPos;
    }

    Vector3 CollideAndRotate(Vector3 vel, Vector3 pos, int depth, float dt)
    {
        if (depth >= maxBounces)
            return Vector3.zero;

        float dist = vel.magnitude + skinWidth;
        float r = cc.radius;

        bool collides = Physics.SphereCast(pos, r, vel.normalized, out RaycastHit hit, dist, collideWith, QueryTriggerInteraction.Ignore);

        if (collides)
        {
            Vector3 snap = vel.normalized * (hit.distance - skinWidth);
            Vector3 leftOver = vel - snap;

            grounded = true;

            float fwdVel = Vector3.Dot(velocity, transform.forward);
            float rghVel = Vector3.Dot(velocity, transform.right);

            if (Vector3.Distance(transform.up, head.up) > 0.1f)
                return snap;

            if (climbOn.Contains(hit.transform.gameObject.layer))
            {
                SetUp(hit.normal);
                velocity = transform.forward * fwdVel + transform.right * rghVel;
            }

            return snap + CollideAndRotate(velocity * leftOver.magnitude, pos + snap, depth + 1, dt);
        }

        // don't have to do anything fancy because we didn't hit anything
        return vel;
    }

    void SetUp(Vector3 direction)
    {
        if (transform.up == direction)
            return;

        Vector3 headUp = head.transform.up;
        transform.MatchUp(direction);
        head.MatchUp(headUp);

        rTime = 0;
        startHeadUp = head.up;
    }

    void Rotate(float x = 0f, float y = 0f)
    {
        // Rotate body Y
        transform.Rotate(Vector3.up, y);

        // Rotate head X
        if (head != null)
        {
            xRot -= x;
            xRot = Mathf.Clamp(xRot - x, minLookUpAngle, maxLookUpAngle);
            Camera.main.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        }
    }

    public void SetRotation(float x, float y)
    {
        xRot = Mathf.Clamp(x, minLookUpAngle, maxLookUpAngle);
        transform.rotation = Quaternion.Euler(0, y, 0);
        Rotate();
    }

    public void SetVelocity(Vector3 newVel)
    {
        velocity = newVel;
    }

    public Vector2 Rotation()
    {
        return new Vector2(-xRot, transform.rotation.eulerAngles.y);
    }

    public void Move(CallbackContext c)
    {
        move = c.ReadValue<Vector2>();

        if (move.sqrMagnitude > 1)
            move.Normalize();
    }

    public void Look(CallbackContext c)
    {
        if (!enabled)
            return;

        look += c.ReadValue<Vector2>();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        if (cc == null)
        {
            if (TryGetComponent(out CapsuleCollider c))
                cc = c;
            else
                return;
        }

        // Look direction
        Gizmos.DrawRay(head.position, head.forward);

        // IsGrounded
        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position - transform.up * distDiff, cc.radius + radiusDiff);
    }
}
