
using TMPro;
using UnityEngine;

public class FpsHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private float hudRefreshRate = 1f;

    private float timer;

    private void Start()
    {
        fpsText.enabled = false;
    }

    private void Update()
    {
        if (Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            fpsText.text = "FPS: " + fps;
            timer = Time.unscaledTime + hudRefreshRate;
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            fpsText.enabled = !fpsText.enabled;
        }
    }
}
