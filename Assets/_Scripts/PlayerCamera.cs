using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Tooltip("Should be child of lookDir in player")]
    [SerializeField] private Camera fpsCam;
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform lookDir;

    [SerializeField] private bool lateUpdate = false;
    [SerializeField] private bool fixedUpdate = false;

    Rigidbody rb;
    private float dt = 0;

    void Awake()
    {
        if (!fpsCam)
        {
            fpsCam = GetComponentInChildren<Camera>();
        }
        if (!lookDir)
        {
            lookDir = fpsCam.transform.parent;
        }
        if (!mainCam)
        {
            mainCam = Camera.main;
        }
        rb = GetComponent<Rigidbody>();
    }

    public void SetMainFov(float fov)
    {
        mainCam.fieldOfView = fov;
    }

    public void SetFpsFov(float fov)
    {
        fpsCam.fieldOfView = fov;
    }

    void Update()
    {
        if (!lateUpdate && !fixedUpdate)
        {
            Vector3 offset = rb.velocity * dt;
            mainCam.transform.SetPositionAndRotation(lookDir.position + offset, lookDir.rotation);
            dt += Time.deltaTime;


        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //if(Time.)
    }

    void LateUpdate()
    {
        if (lateUpdate)
        {
            mainCam.transform.SetPositionAndRotation(lookDir);
        }
    }

    void FixedUpdate()
    {
        if (fixedUpdate)
        {
            mainCam.transform.SetPositionAndRotation(lookDir);
        }
        dt = 0;
    }
}
