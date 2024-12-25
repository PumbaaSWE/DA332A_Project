using UnityEngine;
using UnityEngine.Events;

public class PlayerTriggerZone : MonoBehaviour
{
    [SerializeField] GameObject gameObjectToEnable;
    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerExit;
    public UnityEvent OnStart;


    private void Start()
    {
        //we should do Some fancy if player start within the thing...
        OnStart?.Invoke();
        if(gameObjectToEnable) gameObjectToEnable.SetActive(false);
    }


    public void OnEnter()
    {
        OnPlayerEnter?.Invoke();
        if (gameObjectToEnable) gameObjectToEnable.SetActive(true);
    }

    public void OnExit()
    {
        OnPlayerEnter?.Invoke();
        if (gameObjectToEnable) gameObjectToEnable.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player _)) OnEnter();
            
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player _))
            OnPlayerExit?.Invoke();
    }
}
