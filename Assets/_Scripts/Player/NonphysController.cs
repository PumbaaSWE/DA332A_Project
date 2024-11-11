using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class NonphysController : MovementController
{
    public enum PhysicsMode
    {
        [Tooltip("Ignore physics objects, meaning you run straight through them.")]
        Ignore,
        [Tooltip("Treat physics objects as solid static objects.")]
        Solid,
        [Tooltip("Attempt to simulate physics interactions.")]
        Simulated
    }

    [SerializeField] Transform head;

    [SerializeField] float sprintFOV;
    [SerializeField] float fovLerpTime;

    [Header("Looking")]
    [SerializeField] float minLookUpAngle = -90;
    [SerializeField] float maxLookUpAngle = 90f;
    [SerializeField] float mouseSensitivity = 0.05f;
    [SerializeField] float camOffset = 0.35f;

    [Header("Movement")]
    [SerializeField] float acceleration = 25f;
    [SerializeField] float maxWalkSpeed = 3f;
    [SerializeField] float maxSprintSpeed = 6f;
    [Tooltip("Only applies to horizontal movement")]
    [SerializeField] float drag = 7.5f;
    [SerializeField] float jumpVel = 4f;
    [SerializeField] float gravity = 9.81f;
    [SerializeField] float slopeAngle = 35f;

    [Header("Crouch")]
    [Tooltip("Time it takes to fully crouch / uncrouch")]
    [SerializeField] float crouchSpeed = 5f;
    [Tooltip("The players max speed when crouching")]
    [SerializeField] float maxCrouchSpeed = 1.5f;
    [Tooltip("Has to be at least twice as big as collider radius")]
    [SerializeField] float crouchHeight = 1f;

    [Header("Collision")]
    [SerializeField] float radius = 0.3f;
    [Tooltip("Has to be at least twice as big as radius")]
    [SerializeField] float height = 1.85f;
    [Tooltip("Which layers will the player collide with?")]
    [SerializeField] LayerMask collideWith;
    [Tooltip("To prevent float point errors, keep a low value (< 0.1) or it will behave weirdly")]
    [SerializeField] float skinWidth = 0.015f;
    [Tooltip("Max amount of iterations when collision checks and bouncing off walls")]
    [SerializeField] int maxBounces = 3;
    [SerializeField] float pushForce = 0f;
    [SerializeField] PhysicsMode physicsMode;

    [Header("IsGrounded")]
    [SerializeField] float radiusDiff = -0.01f;
    [SerializeField] float distDiff = 0.03f;

    [Header("Stamina")]
    [SerializeField] float maxStamina = 8f;
    [Tooltip("Amount per second")]
    [SerializeField] float sprintCost = 5f;
    [Tooltip("Consumes stamina instantly")]
    [SerializeField] float jumpCost = 2f;
    [Tooltip("Amount per second")]
    [SerializeField] float rechargeRate = 1f;
    [SerializeField] float rechargeCooldown = 1f;

    [Header("Debugging")]
    [SerializeField] bool drawGizmos;
    [SerializeField] bool grounded;
    [SerializeField] Vector3 velocity;
    [SerializeField] float speed;
    [SerializeField] float stamina;

    public float Radius => radius;
    public float Height => height;
    public float CurrentHeight => cc.height;
    public float CamOffset => camOffset;
    public float MaxStamina => maxStamina;
    public float Stamina => stamina;
    public float MouseSensitivity => mouseSensitivity;
    public bool Grounded => grounded;
    public float Speed => speed;
    public float MaxSpeed => maxSprintSpeed;

    // inputs
    Vector2 look;
    Vector2 move;
    bool crouch;
    bool sprint;
    bool jump;

    // components
    CapsuleCollider cc;
    PlayerCamera pc;

    // misc member variables
    bool isCrouched;
    bool wasGrounded;
    float xRot;
    float prevStamina;
    float rechargeTimer;
    Vector3 groundNormal;

    void Start()
    {
        cc = GetComponent<CapsuleCollider>();
        pc = GetComponent<PlayerCamera>();

        cc.center = Vector3.up * Height / 2f;
        cc.height = Height;
        cc.radius = Radius;
        head.localPosition = Vector3.up * (Height - CamOffset);

        stamina = maxStamina;
    }

    void Update()
    {
        if (Time.deltaTime == 0)
        {
            look = Vector2.zero;
            return;
        }

        cc.radius = radius;

        Debug.Assert(height >= radius * 2, "Height is too small! Has to be at least twice as big as radius.");
        Debug.Assert(crouchHeight >= radius * 2, "Crouch height is too small! Has to be at least twice as big as radius.");

        wasGrounded = grounded;
        grounded = IsGrounded();

        // look
        Vector2 preRot = Rotation();
        look *= mouseSensitivity;
        Rotate(look.y, look.x);
        LookDelta = Rotation() - preRot;
        LookDelta = new Vector3(Mathf.DeltaAngle(0, LookDelta.x), Mathf.DeltaAngle(0, LookDelta.y));

        Move(Time.deltaTime);
        Crouch(crouch, Time.deltaTime);
    }

    void FixedUpdate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    bool IsGrounded()
    {
        float r = cc.radius;
        float halfHeight = cc.height / 2;
        bool grounded = Physics.SphereCast(transform.position + Vector3.up * halfHeight, r + radiusDiff, Vector3.down, out RaycastHit hit, halfHeight - r + distDiff, collideWith, QueryTriggerInteraction.Ignore);

        groundNormal = hit.normal;

        if (grounded)
            return Vector3.Angle(hit.normal, Vector3.up) <= slopeAngle;

        return false;
    }

    void Move(float dt)
    {
        // recharge stamina
        if (stamina < prevStamina)
        {
            rechargeTimer = 0;
        }
        else if (rechargeTimer < rechargeCooldown)
        {
            rechargeTimer += dt;
        }
        else
        {
            stamina += dt * rechargeRate;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
        prevStamina = stamina;

        // What is our current max speed?
        float maxSpeed = maxWalkSpeed;
        float forwardSpeed = Vector3.Dot(velocity, transform.forward);

        if (isCrouched)
        {
            float t = Mathf.InverseLerp(crouchHeight, height, cc.height);
            maxSpeed = Mathf.Lerp(maxCrouchSpeed, maxWalkSpeed, t);
        }
        else if (sprint)
        {
            float staminaToRemove = sprintCost * dt;
            if (forwardSpeed > 0 && stamina >= staminaToRemove)
            {
                // deplete stamina
                stamina -= staminaToRemove;

                maxSpeed = maxSprintSpeed;
            }
        }

        // Apply drag
        Vector3 velocityInGroundPlane = velocity.WithY();

        if (grounded)
            velocityInGroundPlane = Vector3.ProjectOnPlane(velocity, groundNormal);

        Vector3 dragForce = -velocityInGroundPlane * drag * dt;

        if (dragForce.sqrMagnitude > velocityInGroundPlane.sqrMagnitude)
            dragForce = -velocityInGroundPlane;

        velocity += dragForce;

        // Apply acceleration
        Vector3 wishDir = transform.right * move.x + transform.forward * move.y;

        float acceleration = this.acceleration;
        if (sprint && Vector3.Dot(wishDir, transform.forward) > 0)
            acceleration *= maxSprintSpeed / maxWalkSpeed;

        Vector3 deltaVel = wishDir * acceleration * dt;

        deltaVel = deltaVel.normalized * Mathf.Min(deltaVel.magnitude, Mathf.Max(0, maxSpeed - Vector3.Dot(velocity, deltaVel.normalized)));

        velocity += deltaVel;

        if (!wasGrounded && grounded)
            velocity = velocity.WithY();

        // Apply gravity
        if (!grounded)
            velocity += Vector3.down * gravity * dt;
        else if (!jump)
            velocity = Vector3.ProjectOnPlane(velocity, groundNormal).normalized * velocity.magnitude; // Slope stuff

        // FOV (will probably change this later depending on which other system interact with FOV)
        float mainFOV = pc.DefaultFov;
        float fpsFOV = pc.DefaultFpsFov;
        if (forwardSpeed > maxWalkSpeed)
        {
            float t = Mathf.InverseLerp(maxWalkSpeed, maxSprintSpeed, speed);
            mainFOV = Mathf.Lerp(mainFOV, sprintFOV, t);
            fpsFOV = Mathf.Lerp(fpsFOV, sprintFOV, t);
        }

        pc.LerpMainFov(mainFOV, fovLerpTime);
        pc.LerpFpsFov(fpsFOV, fovLerpTime);

        // Collide and slide algorithm
        float velY = velocity.y;
        Vector3 deltaPos = CollideAndSlide(velocity * dt, transform.position, 0, dt);
        velocity = deltaPos / dt;
        velocity.y = Mathf.Min(velocity.y, velY); // Otherwise we might slide up alot in the air. For example if we jump and hit a corner, not very grounded!
        speed = velocity.WithY().magnitude;

        // Set position
        transform.position += deltaPos;

        jump = false;
    }

    void Crouch(bool crouch, float dt)
    {
        // Crouch and uncrouch
        if (crouch)
        {
            cc.height = Mathf.Max(cc.height - crouchSpeed * dt, crouchHeight);
            cc.center = Vector3.up * cc.height / 2f;
        }
        else if (isCrouched)
        {
            // Check if we can uncrouch
            bool hitHead = Physics.SphereCast(transform.position + Vector3.up * cc.radius, cc.radius, Vector3.up, out RaycastHit hit, cc.height + crouchSpeed * dt - cc.radius * 2f, collideWith, QueryTriggerInteraction.Ignore);

            if (!hitHead)
            {
                cc.height = Mathf.Min(cc.height + crouchSpeed * Time.deltaTime, height);
                cc.center = Vector3.up * cc.height / 2f;
            }
        }
        head.position = transform.position + Vector3.up * (cc.height - camOffset);

        isCrouched = cc.height != height;
    }

    Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth, float dt)
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

            // lots of messy code here, but basically rigidbody interaction stuff
            if (hit.transform.TryGetComponent(out Rigidbody rb))
            {
                switch (physicsMode)
                {
                    case PhysicsMode.Ignore:
                            snap += vel.normalized * skinWidth;
                            leftOver = vel - snap;
                            return snap + CollideAndSlide(leftOver, pos + snap, depth + 1, dt);

                    case PhysicsMode.Simulated:
                        Vector3 distance = RigidbodyCollide(rb, hit.point, leftOver, pushForce, 0, dt);
                        rb.transform.position += distance;
                        rb.AddForceAtPosition(-distance.magnitude * hit.normal / dt, hit.point, ForceMode.VelocityChange);
                        Vector3 pushSnap = snap + distance;
                        leftOver = vel - pushSnap;
                        leftOver = Vector3.ProjectOnPlane(leftOver, hit.normal);
                        return pushSnap + CollideAndSlide(leftOver, pos + pushSnap, depth + 1, dt);

                    case PhysicsMode.Solid:
                        break;
                }
            }

            // recursive stuff!
            leftOver = Vector3.ProjectOnPlane(leftOver, hit.normal);
            return snap + CollideAndSlide(leftOver, pos + snap, depth + 1, dt);
        }

        // don't have to do anything fancy because we didn't hit anything
        return vel;
    }

    /// <summary>
    /// Returns the distance you can move an object. Depends if it hits other rigidbodies or solids etc.
    /// </summary>
    /// <returns></returns>
    public Vector3 RigidbodyCollide(Rigidbody rb, Vector3 point, Vector3 distance, float pushForce, int depth, float dt)
    {
        // rigidbody collide and not slide... maybe it should slide

        if (depth >= maxBounces)
            return Vector3.zero;

        float requiredForce = (rb.mass * distance / dt).magnitude;
        float strength = Mathf.Lerp(1, 0, requiredForce / pushForce);
        pushForce -= requiredForce;

        float dist = distance.magnitude * strength + skinWidth;

        if (rb.SweepTest(distance, out RaycastHit hitInfo, dist, QueryTriggerInteraction.Ignore))
        {
            Vector3 clearedDistance = distance.normalized * (hitInfo.distance - skinWidth);

            // rb hits another rb
            if (hitInfo.transform.TryGetComponent(out Rigidbody otherRb))
                return clearedDistance + RigidbodyCollide(otherRb, hitInfo.point, distance - clearedDistance, pushForce, depth +1, dt);

            // rb hits a solid
            return clearedDistance;
        }

        // rb doesn't hit anything
        return distance * strength;
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
        //Gizmos.DrawRay(head.position, Camera.main.transform.forward);

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
    public override void Rotate(float x = 0f, float y = 0f)
    {
        // Rotate body Y
        transform.Rotate(Vector3.up, y);

        // Rotate head X
        if (head != null)
        {
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

    #region Inputs
    public void Move(CallbackContext c)
    {
        move = c.ReadValue<Vector2>();

    }

    public void Look(CallbackContext c)
    {
        if (!enabled)
            return;

        look += c.ReadValue<Vector2>();
    }

    public void Crouch(CallbackContext c)
    {
        if (!enabled)
            return;

        if (c.started)
            crouch = true;

        if (c.canceled)
            crouch = false;

        Blackboard.Instance.Set("hasCrouched", true);
    }

    public void Jump(CallbackContext c)
    {
        if (!enabled)
            return;

        if (!c.started) return;

        if (!grounded) return;

        if (isCrouched) return;

        if (stamina < jumpCost) return;

        // deplete stamina
        stamina -= jumpCost;

        velocity = velocity.WithY(jumpVel);
        jump = true;
    }

    public void Sprint(CallbackContext c)
    {
        if (!enabled)
            return;

        if (c.started)
            sprint = true;

        if (c.canceled)
            sprint = false;
    }
    #endregion
}
