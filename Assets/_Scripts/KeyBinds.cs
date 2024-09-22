using UnityEngine;


[CreateAssetMenu(fileName = "NewKeyBind", menuName = "ScriptableObjects/KeyBinds", order = 1)]
public class KeyBinds : ScriptableObject
{
    public float sens = 1000;
    public float sensX = 1;
    public float sensY = 1;

    public KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode interact = KeyCode.E;
    public KeyCode jump = KeyCode.Space;
    public KeyCode crouch = KeyCode.LeftControl;
    public KeyCode crouchToggle = KeyCode.C;
    public KeyCode sprint = KeyCode.LeftShift;



}