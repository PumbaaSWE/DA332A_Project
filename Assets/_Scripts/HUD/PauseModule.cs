using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseModule : MonoBehaviour
{
    Transform pauseMenu;
    Transform settingsMenu;
    Transform controlsMenu;
    [SerializeField] bool paused = false;
    public bool Paused { get { return paused; } }
    public UnityEvent Triggered;
    void Awake()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        pauseMenu = gameObject.transform.GetChild(0);
        settingsMenu = gameObject.transform.GetChild(1);
        controlsMenu = gameObject.transform.GetChild(2);
        Init();
    }
    void Init()
    {
        Transform button = pauseMenu.transform.Find("Buttons/ResumeButton");
        UnityEvent unityEvent = button.GetComponent<Button>().onClick;
        unityEvent.AddListener(OnClick);

        button = pauseMenu.transform.Find("Buttons/SettingsButton");
        unityEvent = button.GetComponent<Button>().onClick;
        unityEvent.AddListener(EnableSettings);

        button = pauseMenu.transform.Find("Buttons/ControlsButton");
        unityEvent = button.GetComponent<Button>().onClick;
        unityEvent.AddListener(EnableControls);

        button = pauseMenu.transform.Find("Buttons/QuitButton");
        unityEvent = button.GetComponent<Button>().onClick;
        unityEvent.AddListener(Quit);
    }
    //// Update is called once per frame
    //void Update()
    //{
        
    //}
    public void OnClick()
    {
        switch (paused)
        {
            case true:
                paused = false;
                Resume();
                break;
            case false:
                paused = true;
                Pause();
                break;
        }
        if(Triggered != null)
        {
            Triggered.Invoke();
        }
    }
    void Pause()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameObject.SetActive(true);
        EnablePauseMenu();
    }
    void Resume()
    {
        DisableAll();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }
    void EnablePauseMenu()
    {
        pauseMenu.gameObject.SetActive(true);
        DisableSettings();
        DisableControls();
    }
    void DisablePauseMenu()
    {
        pauseMenu.gameObject.SetActive(false);
    }
    void EnableSettings()
    {
        settingsMenu.gameObject.SetActive(true);
        DisablePauseMenu();
    }
    void DisableSettings()
    {
        settingsMenu.gameObject.SetActive(false);
    }
    void EnableControls()
    {
        controlsMenu.gameObject.SetActive(true);
        DisablePauseMenu();
    }
    void DisableControls()
    {
        controlsMenu.gameObject.SetActive(false);
    }
    void DisableAll()
    {
        DisablePauseMenu();
        DisableSettings();
        DisableControls();
    }
    void Quit()
    {
        //Enviroment quit, or back to menu
        //Resume TimeScale
    }
}
