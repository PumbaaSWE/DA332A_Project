using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ControlsHUD : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Text text;
    private PlayerControls controls;
    InputAction[] inputs;
    int inputCount = 10;
    void Awake()
    {
        text = transform.GetChild(1).GetComponentInChildren<TMP_Text>();
        GetControls();
        if (controls != null)
        {
            ApplyText();
        }
    }
    void OnEnable()
    {
        GetControls();
        if (controls != null)
        {
            ApplyText();
        }
    }
    void GetControls()
    {
        controls = new PlayerControls();
        if (controls == null) { return; }
        inputs = new InputAction[inputCount];
        inputs[0] = controls.Player.Move;
        inputs[1] = controls.Player.Fire;
        inputs[2] = controls.Player.SecondaryUse;
        inputs[3] = controls.Player.Crouch;
        inputs[4] = controls.Player.Jump;
        inputs[5] = controls.Player.Sprint;
        inputs[6] = controls.Player.Interact;
        inputs[7] = controls.Player.F;
        inputs[8] = controls.Player.G;
        inputs[9] = controls.Player.Reload;
    }
    void ApplyText()
    {
        //text.text = string.Empty;
        //for (int i = 0; i < inputCount; i++)
        //{
        //    string format = string.Format("{0} - {1}\n", inputs[i].name, inputs[i].bindings[0].path.ToString());
        //    format = format.Replace("<Keyboard>/", string.Empty);
        //    format = format.Replace("<Gamepad>/", string.Empty);
        //    text.text += format;
        //}
        text.text = string.Empty;
        for (int i = 0; i < inputCount; i++)
        {
            if (inputs[i] == controls.Player.Move)
            {
                text.text += "Move - WASD\n";
                continue;
            }
            if(inputs[i] == controls.Player.Fire)
            {
                text.text += "Fire - LEFT MOUSE BUTTON\n";
                continue;
            }
            if (inputs[i] == controls.Player.SecondaryUse)
            {
                text.text += "Aim - RIGHT MOUSE BUTTON\n";
                continue;
            }
            if (inputs[i] == controls.Player.F)
            {
                text.text += "Throw Flare - F\n";
                continue;
            }
            if (inputs[i] == controls.Player.G)
            {
                text.text += "Flashlight - G\n";
                continue;
            }
            foreach (var binding in inputs[i].bindings)
            {
                // Check if the binding is associated with the keyboard
                if (binding.path.StartsWith("<Keyboard>")/* && !binding.isPartOfComposite*/)
                {
                    // Format and add to text
                    string format = string.Format("{0} - {1}\n", inputs[i].name, binding.path.Replace("<Keyboard>/", string.Empty).ToUpper());
                    text.text += format;
                }
            }
        }
    }
}
