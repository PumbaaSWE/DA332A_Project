using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private Camera fpsCam;

    [SerializeField] private float defaultFov = 74;
    [SerializeField] private float defaultFpsFov = 74;

    [SerializeField] private float bobAmp = 0.01f;
    [SerializeField] private float bobFreq = 15f;

    public Camera MainCam => mainCam;
    public Camera FpsCam => fpsCam;
    public float DefaultFov => defaultFov;
    public float DefaultFpsFov => defaultFpsFov;

    public Vector3 LookDir => mainCam.transform.forward;
    public Vector3 EyePos => mainCam.transform.position;
    public Quaternion LookRot => mainCam.transform.rotation;
    public float MainCamZoomLevel { get => mainCam.fieldOfView / defaultFov; set => SetMainZoom(value); } 
    public float FpsCamZoomLevel { get => fpsCam.fieldOfView / defaultFpsFov; set => SetFpsZoom(value); }

    

    private float targetFov = 74;
    private float fovAngle = 74;
    private float fpsFovAngle = 74;
    private float targetSpeed = 1;
    private float targetFpsFov = 74;
    private float targetFpsSpeed = 1;

    // bob vars
    private float tv;
    private float th;
    private NonphysController nc;

    void Awake()
    {
        Debug.Assert(mainCam, "PlayerCamera - Assign mainCam in Inspector!");
        Debug.Assert(fpsCam, "PlayerCamera - Assign fpsCam in Inspector!");
        LockMouse();


        fovAngle = Mathf.Tan(defaultFov * 0.5f * Mathf.Deg2Rad);
        fpsFovAngle = Mathf.Tan(defaultFpsFov * 0.5f * Mathf.Deg2Rad);

        SetMainFov(defaultFov);
        SetFpsFov(defaultFpsFov);
    }

    public void SetMainFov(float fov)
    {
        mainCam.fieldOfView = fov;
    }

    public void SetFpsFov(float fov)
    {
        fpsCam.fieldOfView = fov;
    }

    public void ResetFovs()
    {
        mainCam.fieldOfView = defaultFov;
        fpsCam.fieldOfView = defaultFpsFov;
    }

    public void LerpMainFov(float targetFov, float time)
    {
        this.targetFov = Mathf.Clamp(targetFov, 30, 180);
        float error = Mathf.Abs(mainCam.fieldOfView - targetFov);
        targetSpeed = error / time;
    }

    public void MoveMainFov(float targetFov, float speed)
    {
        this.targetFov = Mathf.Clamp(targetFov, 30, 180);
        targetSpeed = speed;
    }

    public void SetMainZoom(float zoomLevel)
    {
        float fov = Mathf.Atan(fovAngle / zoomLevel) * 2 * Mathf.Rad2Deg;
        mainCam.fieldOfView = Mathf.Clamp(fov, 30, 180);
    }

    public void LerpMainZoom(float zoomLevel, float time)
    {
        float fov = Mathf.Atan(fovAngle / zoomLevel) * 2 * Mathf.Rad2Deg;
        LerpMainFov(fov, time);
    }

    public void MoveMainZoom(float zoomLevel, float speed)
    {
        float fov = Mathf.Atan(fovAngle / zoomLevel) * 2 * Mathf.Rad2Deg;
        MoveMainFov(fov, speed);
    }

    public void LerpFpsFov(float targetFov, float time)
    {
        targetFpsFov = Mathf.Clamp(targetFov, 30, 180);
        float error = Mathf.Abs(fpsCam.fieldOfView - targetFov);
        targetFpsSpeed = error / time;
    }

    public void MoveFpsFov(float targetFov, float speed)
    {
        targetFpsFov = Mathf.Clamp(targetFov, 30, 180);
        targetFpsSpeed = speed;
    }
    public void MoveFpsZoom(float zoomLevel, float speed)
    {
        float fov = Mathf.Atan(fpsFovAngle / zoomLevel) * 2 * Mathf.Rad2Deg;
        MoveFpsFov(fov, speed);
    }

    public void LerpFpsZoom(float zoomLevel, float time)
    {
        float fov = Mathf.Atan(fpsFovAngle / zoomLevel) * 2 * Mathf.Rad2Deg;
        LerpFpsFov(fov, time);
    }

    public void SetFpsZoom(float zoomLevel)
    {
        float fov = Mathf.Atan(fpsFovAngle / zoomLevel) * 2 * Mathf.Rad2Deg;
        fpsCam.fieldOfView = Mathf.Clamp(fov, 30, 180);
    }

    public void SetZoom(float zoomLevel)
    {
        SetFpsZoom(zoomLevel);
        SetMainZoom(zoomLevel);
    }

    public void MoveZoom(float zoomLevel, float speed)
    {
        MoveMainZoom(zoomLevel, speed);
        MoveFpsZoom(zoomLevel, speed);
    }

    public void LerpZoom(float zoomLevel, float time)
    {
        LerpMainZoom(zoomLevel, time);
        LerpFpsZoom(zoomLevel, time);
    }

    public void LockMouse(bool hide = true)
    {
        Cursor.lockState = hide ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !hide;
    }

    void Update()
    {   
        if(mainCam.fieldOfView != targetFov)
        {
            mainCam.fieldOfView = Mathf.MoveTowards(mainCam.fieldOfView, targetFov, targetSpeed * Time.deltaTime);
        }

        if (fpsCam.fieldOfView != targetFpsFov)
        {
            fpsCam.fieldOfView = Mathf.MoveTowards(fpsCam.fieldOfView, targetFpsFov, targetFpsSpeed * Time.deltaTime);
        }

        Bob(Time.deltaTime);
    }

    void Bob(float dt)
    {
        if (nc == null)
        {
            nc = GetComponent<NonphysController>();

            if (nc == null)
                return;
        }

        if (!nc.Grounded)
            return;

        float speed = Mathf.InverseLerp(0, nc.MaxSpeed, nc.Speed);

        if (speed > 0.01)
        {
            th += dt * speed;
            float bobh = Mathf.Cos(th * bobFreq / 2);
            fpsCam.transform.localPosition = fpsCam.transform.localPosition.WithX(bobh * bobAmp * 2 * speed);
        }


        tv += dt * speed;
        float bobv = Mathf.Sin(tv * bobFreq);

        fpsCam.transform.localPosition = fpsCam.transform.localPosition.WithY(bobv * bobAmp * speed);
    }
}
