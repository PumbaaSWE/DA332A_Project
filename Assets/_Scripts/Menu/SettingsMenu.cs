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

    NonphysController nc;
    ClimbController cc;

    void Awake()
    {
        StartCoroutine(FindPlayer());
        StartCoroutine(TryGetAndSetSensitivity());
    }

    IEnumerator FindPlayer()
    {
        Player p = FindAnyObjectByType<Player>();

        while (p == null)
        {
            p = FindAnyObjectByType<Player>();
            Debug.Log("Lookig for player..");
            yield return null;
        }

        nc = p.GetComponent<NonphysController>();
        cc = p.GetComponent<ClimbController>();

        Debug.Log("Player found!");
    }

    IEnumerator TryGetAndSetSensitivity()
    {
        while (!nc || !cc)
        {
            yield return null;
        }

        SetSens(nc.MouseSensitivity);
    }

    IEnumerator TrySetSensitivity(float value)
    {
        while (!nc || !cc)
        {
            yield return null;
        }

        SetSens(value);
    }

    private void SetSens(float value)
    {
        // set controller values
        nc.MouseSensitivity = value;
        cc.MouseSensitivity = value;

        // set menu values
        sensitivitySlider.value = value;
        sensitivityField.text = value.ToString("0.###");
    }

    public void SetSensitivity(float value)
    {
        StartCoroutine(TrySetSensitivity(value));
    }

    public void SetSensitivity(string value)
    {
        try
        {
            float single = Convert.ToSingle(value);
            SetSensitivity(single);
        }
        catch
        {
            StartCoroutine(TryGetAndSetSensitivity());
        }
    }
}
