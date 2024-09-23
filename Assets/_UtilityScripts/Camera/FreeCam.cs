using UnityEngine;

public class FreeCam : MonoBehaviour
{
    public float speed = 100;
    public float speedMultiplier = 2;
    public Transform cam;
    public float mouseSensX = 100;
    public bool toogleLock = true;
    private bool mouseLock = false;

    private void Awake()
    {
        transform.localEulerAngles = transform.localEulerAngles.With(x: 0, z: 0);
        cam = Camera.main.transform;
        cam.localPosition = Vector3.zero;
        cam.localEulerAngles = cam.localEulerAngles.With(y: 0, z: 0);
    }

    //private void CorrectRoration()

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        float speed = Input.GetKey(KeyCode.LeftShift) ? this.speed + speedMultiplier : this.speed;



        if (Input.GetKey(KeyCode.W))
        {
            transform.position += speed * Time.deltaTime * cam.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= speed * Time.deltaTime * cam.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= speed * Time.deltaTime * cam.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += speed * Time.deltaTime * cam.right;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += speed * Time.deltaTime * cam.up;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position -= speed * Time.deltaTime * cam.up;
        }

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {

            mouseLock = !mouseLock;
            if (mouseLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse2) && !toogleLock)
        {
            Cursor.lockState = CursorLockMode.None;
            mouseLock = false;
        }

        if(Input.mouseScrollDelta != Vector2.zero)
        {
            int dir = (int)Mathf.Sign(Input.mouseScrollDelta.y);
            mouseSensX += dir * 10;
            mouseSensX = Mathf.Clamp(mouseSensX, 100, 10000);
        }

        if (Input.GetKey(KeyCode.Mouse1) || mouseLock)
        {
            //float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            if (mouseX != 0)
            {
                transform.Rotate(0, mouseX * mouseSensX * Time.deltaTime, 0);
            }
            if (mouseY != 0)
            {
                cam.Rotate(-mouseY * mouseSensX * Time.deltaTime, 0, 0);
            }
        }

        
    }
}
