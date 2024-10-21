using static UnityEngine.InputSystem.InputAction;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseObjectPrefab;
    [SerializeField] GameObject pauseObject;
    PauseModule pauseModule;
    public bool Paused { get { return pauseModule.Paused; } }
    public UnityEvent Triggered;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void InitPauseModule()
    {
        LoadPauseObject();
        FindPauseModule();
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
    [MakeButton]
    public void LoadPauseObject()
    {
        if (Application.isPlaying)
        {
            pauseObject = Instantiate(pauseObjectPrefab, Vector3.zero, Quaternion.identity);
            FindPauseModule();
        }
    }
    [MakeButton]
    public void UnloadPauseScene()
    {
        if (Application.isPlaying)
        {
            Destroy(pauseObject);
            pauseModule = null;
        }
    }
}
