using UnityEngine;
[CreateAssetMenu(fileName = "InputControllerData", menuName = "ScriptableObjects/ControllerData", order = 2)]
public class ControllerData : ScriptableObject
{
    [Header("Setup")]
    public float maxLookUpAngle = 90f;
    public float minLookUpAngle = -90f;
    //public float mass = 90;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float strafeMultiplier = .75f;
    public float sprintMultiplier = 1.625f;
    public float backMultiplier = .75f;
    [Tooltip("")]
    public float maxAccel = 90f;
    [Tooltip("Should be around 10x amount of kg you want to push")]
    public float maxPushForce = 900f;
    public float groundDrag = .5f;
    public float camOffset = .2f;
    public float height = 1.85f;
    public float radius = .3f;
    public LayerMask groundLayer;


    [Header("Jumping")]
    [Tooltip("If you want to use this directly, do set Jump Height <= 0")]
    public float jumpForce = 10;
    public float jumpHeight = 1;
    public float jumpCooldown = .25f;
    public float airDrag = .25f;

    [Header("Slope")]
    public float slopeAngle = 45;
    public float stepHeight = .4f;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float crouchSpeed = .1f;
}
