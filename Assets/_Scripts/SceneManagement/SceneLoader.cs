using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SceneGroupManager)), DisallowMultipleComponent]
public class SceneLoader : MonoBehaviour
{
    public Image loadingBar;
    public float fillSpeed = 0.5f;
    public Canvas loadingCanvas;
    public Camera loadingCamera;
    public SceneGroup[] sceneGroups; //SceneGroupData

    private bool isLoading = false;
    private float targetRrogress;

    SceneGroupManager sceneGroupManager;

    private void Awake()
    {
        sceneGroupManager = GetComponent<SceneGroupManager>();
        sceneGroupManager.OnSceneGroupLoaded += OnLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSceneGroup(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLoading) return;

        float currentFill = loadingBar.fillAmount;
        float progressDelta = Mathf.Abs(currentFill - targetRrogress);
        loadingBar.fillAmount = Mathf.Lerp(currentFill, targetRrogress, progressDelta * Time.deltaTime * fillSpeed);
    }


    public void LoadSceneGroup(int i, bool showLoadingScreen = true)
    {
        if(i < 0 || i >= sceneGroups.Length)
        {
            
            return;
        }

        LoadingProgress progress = new();
        progress.OnProgress += t => targetRrogress = Mathf.Max(t, targetRrogress); //dont go backwards...

        if(showLoadingScreen)
        {
            EnableLoadingScreen(true);
        }
        sceneGroupManager.LoadScenes(sceneGroups[0], progress);
    }

    private void OnLoaded()
    {
        EnableLoadingScreen(false);
    }

    private void EnableLoadingScreen(bool enabled)
    {
        isLoading = enabled;
        loadingCanvas.gameObject.SetActive(enabled);
        loadingCamera.gameObject.SetActive(enabled);

    }
}

public class LoadingProgress : IProgress<float>
{
    public event Action<float> OnProgress;
    public void Report(float value)
    {
        OnProgress?.Invoke(value);
    }
}
