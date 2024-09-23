using UnityEngine;
using UnityEngine.Events;

public abstract class Controller : MonoBehaviour
{
    protected Vector2 move;
    protected Vector2 look;

    public Vector2 Move { get => move; }
    public Vector2 Look { get => look; }


    public Button A = new();
    public Button B = new();
    public Button X = new();
    public Button Y = new();

    public class Button
    {
        bool isPressed = false;
        public UnityEvent OnPressDown = new();
        public UnityEvent OnPressUp = new();


        public bool IsPressed
        {
            get => isPressed;

            set
            {
                if (isPressed == value)
                    return;

                if (!isPressed && value)
                    OnPressDown.Invoke();

                if (isPressed && !value)
                    OnPressUp.Invoke();

                isPressed = value;
            }
        }
    }
}