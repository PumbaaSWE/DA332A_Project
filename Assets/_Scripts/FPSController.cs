using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// Partly stolen from BaseController by pumbaaswe, partly stolen from Sourcelike FPS Character Controller by Majikayo Games and partly original af.
/// Author: Winn1337 / Johannes
/// </summary>
public class FPSController : MonoBehaviour
{
    public struct MoveCommand
    {
        public Vector2 move;
        public Vector2 mouse;
        public bool jump;
        public bool crouch;
        public bool crouchToggle;
        public bool sprint;
    }

    [SerializeField] private KeyBinds keyBinds;
    [SerializeField] private ControllerData controllerData;
    [SerializeField] private float MaxAirAccel;
    [SerializeField] private Transform head;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float speed;

    private MoveCommand command;
    private CapsuleCollider cc;
    private Rigidbody rb;
    private PlayerInput pInput;

    private bool crouchToggle;
    private bool grounded;
    private bool wasGrounded;
    private bool didJump;
    private float xRot;
    private Vector3 move;

    private void Start()
    {
        cc = GetComponent<CapsuleCollider>();
        cc.radius = controllerData.radius;
        cc.height = controllerData.height;
        cc.center = Vector3.up * controllerData.height / 2f;

        rb = GetComponent<Rigidbody>();
        pInput = GetComponent<PlayerInput>();
        move = Vector3.zero;

        if (controllerData.jumpHeight > 0)
        {
            /* Math to compute impulse (force) to jump a set height
             * impulse J = delta_p = m*v
             * mv^2/2 = p^2/(2*m) = mgh => p^2 = 2 * m^2 * h etc...
             */
            // but we don't consider drag, maybe that's why it doesn't work on my controller?
            float g = Physics.gravity.magnitude;
            float m = rb.mass;
            controllerData.jumpForce = Mathf.Sqrt(2 * m * m * g * controllerData.jumpHeight);
        }
    }

    /// <summary>
    /// Rotate player camera by <paramref name="x"/> (pitch) and <paramref name="y"/> (yaw) degrees.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Rotate(float x, float y)
    {
        // Rotate body Y
        transform.Rotate(Vector3.up, y);

        // Rotate head X
        if (head != null)
        {
            xRot = Mathf.Clamp(xRot - x, controllerData.minLookUpAngle, controllerData.maxLookUpAngle);
            head.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        }
    }

    private void Update()
    {
        command = GetInput();

        Rotate(command.mouse.y, command.mouse.x);

        // Crouch and uncrouch
        if (command.crouch)
        {
            cc.height = Mathf.Max(cc.height - controllerData.crouchSpeed * Time.deltaTime, controllerData.crouchHeight);
            cc.center = Vector3.up * cc.height / 2f;
        }
        else
        {
            // Check if we can uncrouch
            bool hitHead = Physics.SphereCast(transform.position + Vector3.up * cc.radius, cc.radius, Vector3.up, out RaycastHit hit, cc.height + controllerData.crouchSpeed * Time.deltaTime - cc.radius * 2f, controllerData.groundLayer, QueryTriggerInteraction.Ignore);

            if (!hitHead)
            {
                cc.height = Mathf.Min(cc.height + controllerData.crouchSpeed * Time.deltaTime, controllerData.height);
                cc.center = Vector3.up * cc.height / 2f;
            }
        }
        head.position = transform.position + Vector3.up * (cc.height - controllerData.camOffset);
        bool crouched = cc.height != controllerData.height;

        // Jump
        if (command.jump && !crouched && grounded && !didJump)
        {
            rb.velocity = rb.velocity.WithY();
            rb.AddForce(Vector3.up * controllerData.jumpForce, ForceMode.Impulse);
            didJump = true;
        }

        // Sum the move vector for fixed update
        if (command.move.sqrMagnitude > 1f)
            command.move.Normalize();

        command.move.x *= controllerData.strafeMultiplier;
        command.move.y *= command.move.y > 0f ? (crouched ? 1f : (command.sprint ? controllerData.sprintMultiplier : 1f)) : controllerData.backMultiplier;

        if (crouched)
            command.move *= controllerData.backMultiplier;

        Vector3 wishDir = transform.right * command.move.x + transform.forward * command.move.y;

        move += wishDir * Time.deltaTime;

        // Debug
        velocity = rb.velocity;
        speed = rb.velocity.magnitude;
    }

    private void FixedUpdate()
    {
        float r = cc.radius;
        float halfHeight = cc.height / 2;

        wasGrounded = grounded;

        grounded = Physics.SphereCast(transform.position + Vector3.up * halfHeight, r, Vector3.down, out RaycastHit hit, halfHeight + .1f - r, controllerData.groundLayer, QueryTriggerInteraction.Ignore);

        if (didJump)
        {
            grounded = false;
        }

        // Stair snapping disabled for now because it's jittery and not must have, right now!

        // Snap down if sent flying by ramps or stepping down small steps
        //if (rb.velocity.y < 0 && wasGrounded && !didJump)
        //{
        //    // p + h
        //    //
        //    Physics.SphereCast(transform.position + Vector3.up * halfHeight, r, Vector3.down, out RaycastHit hit, halfHeight - r + controllerData.stepHeight, controllerData.groundLayer, QueryTriggerInteraction.Ignore);

        //    if (hit.distance > 0f && hit.point.y + .01f < transform.position.y)
        //    {
        //        Debug.Log("snap down");

        //        rb.MovePosition(transform.position.WithY(hit.point.y));

        //        //If we move, we want to keep our Y velocity
        //        //if (move.sqrMagnitude > 0f)
        //        //{
        //        //    float normalVel = Vector3.Dot(rb.velocity, hit.normal);
        //        //    rb.velocity -= hit.normal * normalVel;
        //        //}
        //        // Otherwise, we want to stay on the stairs. this is a bit hacky but it's to prevent the capsule collider to make us slide off the steps
        //        //else
        //        {
        //            rb.velocity = rb.velocity.WithY(0f);
        //        }

        //        grounded = true;
        //    }
        //}

        //Snap up small steps
        //if (grounded && !didJump)
        //{
        //    Physics.SphereCast(transform.position + rb.velocity.WithY(0) * Time.fixedDeltaTime + Vector3.up * halfHeight, r, Vector3.down, out RaycastHit hit, halfHeight - r, controllerData.groundLayer, QueryTriggerInteraction.Ignore);

        //    if (hit.distance > 0f && hit.point.y - .01f > transform.position.y && hit.point.y <= transform.position.y + controllerData.stepHeight)
        //    {
        //        Debug.Log("snap up");

        //        transform.position = transform.position.WithY(hit.point.y/* - Physics.gravity.y * Time.fixedDeltaTime * Time.fixedDeltaTime*/);

        //        //If we move, we want to keep our Y velocity
        //        //if (move.sqrMagnitude > 0f)
        //        //{
        //        //    float normalVel = Vector3.Dot(rb.velocity, hit.normal);
        //        //    rb.velocity -= hit.normal * normalVel;
        //        //}
        //        // Otherwise, we want to stay on the stairs. this is a bit hacky but it's to prevent the capsule collider to make us slide off the steps
        //        //else
        //        {
        //            rb.velocity = rb.velocity.WithY(0f);
        //        }

        //        grounded = true;
        //    }
        //}

        if (grounded)
        {
            Physics.Raycast(hit.point + Vector3.up * halfHeight, Vector3.down, out hit, halfHeight + .1f, controllerData.groundLayer, QueryTriggerInteraction.Ignore);

            if (hit.distance < 0f)
                grounded = false;

            float angle = Vector3.Angle(Vector3.up, hit.normal);

            if (angle > controllerData.slopeAngle)
            {
                // hacky way to prevent steep slope climbing, but it (kinda) works?
                Vector3 gravityFromSlope = Vector3.ProjectOnPlane(Physics.gravity, hit.normal);
                rb.AddForce(gravityFromSlope, ForceMode.Acceleration);

                float velocityFromSlopeMagnitude = Vector3.Dot(rb.velocity, gravityFromSlope.normalized);

                if (velocityFromSlopeMagnitude < 0f)
                {
                    Vector3 velocityFromSlope = rb.velocity.normalized * velocityFromSlopeMagnitude;
                    rb.AddForce(velocityFromSlope / Time.fixedDeltaTime, ForceMode.Acceleration);
                }

                grounded = false;
            }
            else
            {
                // i stole this line from jack, very good!
                rb.AddForce(-Vector3.ProjectOnPlane(Physics.gravity, hit.normal), ForceMode.Acceleration);
            }
        }

        if (grounded)
            DoGroundPhysics();
        else
            DoAirPhysics();

        move = Vector3.zero;
        didJump = false;
    }

    private void DoGroundPhysics()
    {
        rb.drag = controllerData.groundDrag;

        if (move.sqrMagnitude <= 0f)
            return;

        float speed = controllerData.moveSpeed;

        // v = v0 + at
        // a = (v - v0) / t
        // F = m * a

        // Gives player bunny hops by adding one ground frame with almost air movement
        if (wasGrounded)
        {
            Vector3 v = move / Time.fixedDeltaTime * speed;
            Vector3 a = (v - rb.velocity) / Time.fixedDeltaTime;
            Vector3 f = rb.mass * a;
            f = f.normalized * Mathf.Min(f.magnitude, rb.mass * controllerData.maxAccel);
            rb.AddForce(f.WithY(0));
        }
        else
        {
            rb.drag = controllerData.airDrag;
            float currentSpeedInWishDir = Vector3.Dot(rb.velocity, move / Time.fixedDeltaTime);

            float v = speed;
            float a = (v - currentSpeedInWishDir) / Time.fixedDeltaTime;
            float f = rb.mass * a;
            rb.AddForce((Mathf.Min(f, rb.mass * controllerData.maxAccel) * move / Time.fixedDeltaTime).WithY(0));
        }
    }
    
    private void DoAirPhysics()
    {
        rb.drag = controllerData.airDrag;

        if (move.sqrMagnitude <= 0f)
            return;

        float currentSpeedInWishDir = Vector3.Dot(rb.velocity, move.normalized);

        float v = controllerData.moveSpeed;
        float a = (v - currentSpeedInWishDir) / Time.fixedDeltaTime;
        float f = rb.mass * a;
        rb.AddForce((Mathf.Min(f, rb.mass * MaxAirAccel) * move.normalized).WithY(0));
    }

    private MoveCommand GetInput()
    {
        Vector2 mouse = pInput.actions.FindAction("Look").ReadValue<Vector2>();
        mouse.x *= keyBinds.sensX;
        mouse.y *= keyBinds.sensY;
        mouse *= Time.deltaTime * keyBinds.sens;

        Vector2 move = pInput.actions.FindAction("Move").ReadValue<Vector2>();
        bool jump = pInput.actions.FindAction("Jump").ReadValue<float>() > 0;
        bool crouch = pInput.actions.FindAction("Crouch").ReadValue<float>() > 0;
        bool sprint = pInput.actions.FindAction("Sprint").ReadValue<float>() > 0;

        return new MoveCommand()
        {
            mouse = mouse,
            move = move,
            jump = jump,
            crouch = crouch,
            crouchToggle = crouchToggle, // haha no crouch toggle for you!!
            sprint = sprint,
        };
    }
}
