using UnityEngine;

public class MouseKeyboardController : Controller
{
    [SerializeField] KeyCode key_move_up = KeyCode.W;
    [SerializeField] KeyCode key_move_left = KeyCode.A;
    [SerializeField] KeyCode key_move_down = KeyCode.S;
    [SerializeField] KeyCode key_move_right = KeyCode.D;

    [SerializeField] bool invertMouse = false;
    [SerializeField] float mouseSensitivity = 100;

    [SerializeField] KeyCode key_A = KeyCode.Mouse0;
    [SerializeField] KeyCode key_B = KeyCode.Mouse1;
    [SerializeField] KeyCode key_X = KeyCode.Q;
    [SerializeField] KeyCode key_Y = KeyCode.E;


    // Update is called once per frame
    void Update()
    {
        move = KeysToVector(key_move_left, key_move_right, key_move_down, key_move_up);

        float horizontal = Input.GetAxisRaw("Mouse X");
        float vertical = Input.GetAxisRaw("Mouse Y");
        //if(invertMouse) vertical = -vertical;
        look = new Vector2(horizontal, invertMouse ? -vertical : vertical) * mouseSensitivity;

        A.IsPressed = Input.GetKey(key_A);
        B.IsPressed = Input.GetKey(key_B);
        X.IsPressed = Input.GetKey(key_X);
        Y.IsPressed = Input.GetKey(key_Y);
    }

    private Vector2 KeysToVector(KeyCode left, KeyCode right, KeyCode down, KeyCode up)
    {
        Vector2 stick = Vector2.zero;

        if (Input.GetKey(left)) stick.x -= 1;
        if (Input.GetKey(right)) stick.x += 1;
        if (Input.GetKey(down)) stick.y -= 1;
        if (Input.GetKey(up)) stick.y += 1;

        if (stick != Vector2.zero)
            stick.Normalize();

        return stick;
    }
}
