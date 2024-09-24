using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.InputSystem.InputAction;

public class NonphysController : MonoBehaviour
{
    [SerializeField] Transform head;

    [Header("Looking")]
    [SerializeField] float minLookUpAngle;
    [SerializeField] float maxLookUpAngle;
    [SerializeField] float mouseSensitivity;

    [Header("Movement")]
    [SerializeField] float acceleration;
    [SerializeField] float walkSpeed;
    [Tooltip("Only applies to horizontal movement")]
    [SerializeField] float drag;
    [SerializeField] float jumpVel;
    [SerializeField] float gravity;

    [Header("Collision")]
    [Tooltip("Which layers will the player collide with?")]
    [SerializeField] LayerMask collideWith;
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

    // inputs
    Vector2 move;
    bool crouch;
    bool sprint;

    // components
    CapsuleCollider cc;

    // misc member variables
    float xRot;

    void Start()
    {
        cc = GetComponent<CapsuleCollider>();
    }


    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        grounded = IsGrounded();
        Move(Time.deltaTime);
    }

    bool IsGrounded()
    {
        float r = cc.radius;
        float halfHeight = cc.height / 2;
        return Physics.SphereCast(transform.position + Vector3.up * halfHeight, r + radiusDiff, Vector3.down, out RaycastHit _, halfHeight - r + distDiff, collideWith, QueryTriggerInteraction.Ignore);
    }

    void Move(float dt)
    {
        // Apply drag
        Vector3 dragForce = -velocity.WithY() * drag * dt;

        if (dragForce.sqrMagnitude > velocity.WithY().sqrMagnitude)
            dragForce = -velocity.WithY();

        velocity += dragForce;

        // Apply acceleration
        Vector3 wishDir = transform.right * move.x + transform.forward * move.y;
        velocity += wishDir * acceleration * dt;

        // Apply gravity
        if (!grounded)
            velocity += Vector3.down * gravity * dt;
        else if (velocity.y < 0)
            velocity = velocity.WithY();

        // Clamp speed in X and Z
        if (velocity.WithY().sqrMagnitude > walkSpeed * walkSpeed)
            velocity = velocity.WithY().normalized * walkSpeed + velocity.y * Vector3.up;

        // Collide and slide algorithm
        Vector3 deltaPos = CollideAndSlide(velocity * dt, transform.position, 0);
        velocity = deltaPos / dt;
        speed = velocity.WithY().magnitude;

        // Set position
        transform.position += deltaPos;
    }

    Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth)
    {
        if (depth >= maxBounces)
            return Vector3.zero;

        float dist = vel.magnitude + skinWidth;
        float r = cc.radius;
        float height = cc.height;
        Vector3 point1 = pos + Vector3.up * r;
        Vector3 point2 = point1 + Vector3.up * (height - r - r);

        bool collides = Physics.CapsuleCast(point2, point1, r, vel.normalized, out RaycastHit hit, dist, collideWith, QueryTriggerInteraction.Ignore);

        if (collides)
        {
            Vector3 snap = vel.normalized * (hit.distance - skinWidth);
            Vector3 leftOver = vel - snap;
            leftOver = Vector3.ProjectOnPlane(leftOver, hit.normal)/*.normalized * leftOver.magnitude*/; // Uncomment this if you want more sliding action. I don't like it because player can slide up walls!

            return snap + CollideAndSlide(leftOver, pos + snap, depth + 1);
        }

        return vel;
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

        // Collider
        Gizmos.color = Color.yellow;
        Vector3 point1 = transform.position + Vector3.up * cc.radius;
        Vector3 point2 = point1 + Vector3.up * (cc.height - cc.radius - cc.radius);
        DrawCapsule(point1, point2, cc.radius);

        // Look direction
        Gizmos.DrawRay(head.position, head.forward);

        // IsGrounded
        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * (cc.radius - distDiff), cc.radius + radiusDiff);
    }

    void DrawCapsule(Vector3 point1, Vector3 point2, float radius)
    {
        Gizmos.DrawWireSphere(point1, radius);
        Gizmos.DrawWireSphere(point2, radius);

        Vector3[] dirs = new Vector3[]
        {
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right
        };

        foreach (var dir in dirs)
            Gizmos.DrawLine(point1 + dir * radius, point2 + dir * radius);
    }

    /// <summary>
    /// Rotate player camera by <paramref name="x"/> (pitch) and <paramref name="y"/> (yaw) degrees.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Rotate(float x = 0f, float y = 0f)
    {
        // Rotate body Y
        transform.Rotate(Vector3.up, y);

        // Rotate head X
        if (head != null)
        {
            xRot = Mathf.Clamp(xRot - x, minLookUpAngle, maxLookUpAngle);
            head.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        }
    }

    #region Inputs
    public void Move(CallbackContext c)
    {
        move = c.ReadValue<Vector2>();

        if (move.sqrMagnitude > 1)
            move.Normalize();
    }

    public void Look(CallbackContext c)
    {
        Vector2 look = c.ReadValue<Vector2>() * mouseSensitivity;
        Rotate(look.y, look.x);
    }

    public void Crouch(CallbackContext c)
    {
        if (c.started)
            crouch = true;

        if (c.canceled)
            crouch = false;
    }

    public void Jump(CallbackContext c)
    {
        if (!c.started) return;

        if (!grounded) return;

        velocity = velocity.WithY(jumpVel);
    }

    public void Sprint(CallbackContext c)
    {
        //bool input = c.ReadValue<bool>();
        Debug.Log("sprint");
    }
    #endregion
}
