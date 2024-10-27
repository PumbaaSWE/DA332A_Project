using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [Header("ReadOnly")]
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] NonphysController controller;
    public float MaxValue => maxStamina;
    public float Value => stamina;

    void Start()
    {
        if (!GetController())
        {
            return;
        }
        maxStamina = controller.MaxStamina;
    }
    void FixedUpdate()
    {
        if (!controller) { return; }
        stamina = controller.Stamina;
        stamina = Mathf.Clamp(stamina, 0.0f, maxStamina);
    }
    bool GetController()
    {
        controller = GetComponent<NonphysController>();
        return controller;
    }
}
