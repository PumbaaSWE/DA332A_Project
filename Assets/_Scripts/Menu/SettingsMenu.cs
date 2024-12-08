using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TMP_InputField sensitivityField;

    NonphysController nonphysController;
    ClimbController climbController;

    void Awake()
    {
        StartCoroutine(FindPlayer());
        StartCoroutine(WaitForPlayer(() => SetSens(nonphysController.MouseSensitivity)));
    }

    // Keep trying to find player in open scenes
    IEnumerator FindPlayer()
    {
        Player p = FindAnyObjectByType<Player>();

        while (p == null)
        {
            p = FindAnyObjectByType<Player>();
            //Debug.Log("Lookig for player..");
            yield return null;
        }

        nonphysController = p.GetComponent<NonphysController>();
        climbController = p.GetComponent<ClimbController>();

        //Debug.Log("Player found!");
    }

    // Wait for other coroutine to find player, then execute action
    IEnumerator WaitForPlayer(Action callback)
    {
        while (!nonphysController || !climbController)
        {
            yield return null;
        }

        callback.Invoke();
    }

    // Change slider, label and controll values
    private void SetSens(float value)
    {
        // Set controller values
        nonphysController.MouseSensitivity = value;
        climbController.MouseSensitivity = value;

        // Set menu values
        sensitivitySlider.value = value;
        sensitivityField.text = value.ToString("0.###");
    }

    // Called from slider.
    public void SetSensitivity(float value)
    {
        StartCoroutine(WaitForPlayer(() => SetSens(value)));
    }

    // Called from input field. If string doesn't parse to float, don't change sens.
    public void SetSensitivity(string value)
    {
        try
        {
            float single = Convert.ToSingle(value);
            SetSensitivity(single);
        }
        catch
        {
            StartCoroutine(WaitForPlayer(() => SetSens(nonphysController.MouseSensitivity)));
        }
    }
}
