using UnityEngine;
using UnityEngine.Events;

public class PlayerDataHUDHolder : MonoBehaviour
{
    public PlayerDataSO playerData;
    PauseManager pauseManager;

    void Awake()
    {
        Init();
    }
    void Init()
    {
        pauseManager = (PauseManager)FindObjectOfType<PauseManager>();
        UnityEvent unityEvent = pauseManager.Triggered;
        unityEvent.AddListener(Triggered);
    }
    void Triggered()
    {
        if (!pauseManager) { return; }
        switch (pauseManager.Paused)
        {
            case true:
                OnDisable();
                break;
            case false:
                OnEnable();
                break;
        }
    }
    void OnEnable()
    {
        gameObject.SetActive(true);
    }
    void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
