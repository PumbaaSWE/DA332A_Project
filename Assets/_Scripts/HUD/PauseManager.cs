using static UnityEngine.InputSystem.InputAction;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseObjectPrefab;
    [SerializeField] GameObject pauseObject;
    PauseModule pauseModule;
    public bool Paused { get { return pauseModule.Paused; } }
    public UnityEvent Triggered;
    void InitPauseModule()
    {
        LoadPauseObject();
        if(!pauseModule) { return; }
        AssignParent();
        pauseModule.Triggered.AddListener(ManualTrigger);
        pauseObject.transform.localPosition = new Vector3(0, 0, 0);
    }
    void ManualTrigger()
    {
        if (Triggered != null)
        {
            Triggered.Invoke();
        }
    }
    public void PauseButton(InputAction.CallbackContext context)
    {
        if (!pauseModule)
        {
            //get module
            InitPauseModule();
            if (!pauseModule)
            {
                return;
            }
        }
        if (context.phase == InputActionPhase.Performed)
        {
            pauseModule.OnClick();
            if(Triggered != null)
            {
                Triggered.Invoke();
            }
        }
    }
    void FindPauseModule()
    {
        pauseModule = pauseObject.GetComponent<PauseModule>();
    }
    void AssignParent()
    {
        pauseObject.transform.parent = transform;
        pauseObject.transform.localPosition = new Vector3(0, 0, 0);
    }
    //[MakeButton]
    public void LoadPauseObject()
    {
        pauseObject = Instantiate(pauseObjectPrefab, Vector3.zero, Quaternion.identity);
        FindPauseModule();
        //if (Application.isPlaying)
        //{

        //}
    }
    //[MakeButton]
    //public void UnloadPauseScene()
    //{
    //    if (Application.isPlaying)
    //    {
    //        Destroy(pauseObject);
    //        pauseModule = null;
    //    }
    //}
}
