using UnityEngine;
using UnityEngine.Events;

public class ToggleButton : MonoBehaviour, IInteractable
{
    public string toolTipOn = "Press {0} to turn off";
    public string toolTipOff = "Press {0} to turn on";
    private string tooltip = "";
    [SerializeField] private bool isOn;


    public bool IsOn
    {
        get { return isOn; }
        set { isOn = value;
            tooltip = isOn ? toolTipOn : toolTipOff;
        }
    }

    public UnityEvent<Transform> OnTurnOn;
    public UnityEvent<Transform> OnTurnOff;
    public string Tooltip => tooltip;

    public string InteractedTooltip => "";

    public bool CanInteract => true;

    public bool ShowInteractMessage => true;

    public int InteractedDisplayPriority => 0;

    public float InteractedTipDisplayTime => 0;

    public void Interact(Transform interactor)
    {
        IsOn = !IsOn;
        if (isOn)
        {
            OnTurnOn?.Invoke(interactor);
        }
        else
        {
            OnTurnOff?.Invoke(interactor);
        }
    }

    public void SpeculateInteract(Transform interactor)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        tooltip = isOn ? toolTipOn : toolTipOff;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
