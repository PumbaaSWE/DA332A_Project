using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActivateGO : MonoBehaviour
{
    [SerializeField] GameObject go;
    [SerializeField] float timer;

    bool startTimer = false;
    public void Update()
    {
        if (timer < 0)
        {
            go.SetActive(true);
            this.enabled = false;
        }
        if (startTimer)
        {
            timer -= Time.deltaTime;
        }

    }

    public void StartTimer()
    {
        startTimer = true;
    }
}
