using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class NonphysController : MonoBehaviour
{
    [SerializeField] Transform head;

    [Header("Look")]
    [SerializeField] float minLookUpAngle;
    [SerializeField] float maxLookUpAngle;
    [SerializeField] float mouseSensitivity;

    [Header("Move")]
    [SerializeField] float acceleration;
    [SerializeField] float walkSpeed;
    [Tooltip("Only applies to horizontal movement")]
    [SerializeField] float drag;
    [SerializeField] float jumpVel;
    [SerializeField] float gravity;
    [SerializeField] LayerMask collideWith;

    [Header("Debug")]
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

        float r = cc.radius;
        float halfHeight = cc.height / 2;
        grounded = Physics.SphereCast(transform.position + Vector3.up * halfHeight, r, Vector3.down, out RaycastHit hit, halfHeight - r, collideWith, QueryTriggerInteraction.Ignore);
        if (grounded && hit.point.y < transform.position.y)
            transform.position = transform.position.WithY(hit.point.y);

        Move(Time.deltaTime);
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

        speed = velocity.WithY().magnitude;

        // Set position
        // This works by casting your capsule towards your next desired position
        // If the capsule collides with anything, it will project the velocity using the hit normal and try to move again (otherwise you get stuck in the wall)
        // It tries to move and project max 3 times
        Vector3 deltaPos = velocity * dt;
        float r = cc.radius;
        float height = cc.height;
        for (int i = 0; i < 3; i++)
        {
            Vector3 point1 = transform.position + Vector3.up * r;
            Vector3 point2 = point1 + Vector3.up * (height - r - r);
            bool collides = Physics.CapsuleCast(point1, point2, r, deltaPos, out RaycastHit hit, deltaPos.magnitude, collideWith, QueryTriggerInteraction.Ignore);

            if (collides)
            {
                transform.position += deltaPos.normalized * hit.distance;
                deltaPos = Vector3.ProjectOnPlane(deltaPos, hit.normal);
            }
            else
            {
                transform.position += deltaPos;
                break;
            }
        }
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
