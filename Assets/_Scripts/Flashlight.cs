using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

//this is temporary
public class Flashlight : MonoBehaviour
{
    private Light m_light; 
    [SerializeField] PlayerInput playerInput;
    InputAction action;
    string key = "<nope>";
    bool lightHasBeenOn = false;

    Quaternion rotation;
    [SerializeField] float lerpFactor = 10;
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        action = playerInput.actions.FindAction("G");
        //playerInput.actions.
        //InputBinding binding 
        key = "[" + action.bindings.First().ToDisplayString() + "]";
        m_light = GetComponent<Light>();
        //StartCoroutine(TooltipTest());
        m_light.enabled = false;
        if(!audioSource)audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator TooltipTest()
    {
        yield return new WaitForSeconds(5);
        if(!lightHasBeenOn){
            TooltipUtil.Display("Press " + key + " for flashlight.", 5);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (action.triggered)
        {
            m_light.enabled = !m_light.enabled;
            if (audioSource) audioSource.Play();
            lightHasBeenOn = true;
            Blackboard.Instance.Set("lightHasBeenOn", true);
        }

        // Interpolate rotation
        transform.rotation = rotation = Quaternion.Lerp(rotation, transform.parent.rotation, lerpFactor * Time.deltaTime);
    }
}
