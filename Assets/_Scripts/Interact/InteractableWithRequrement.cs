using UnityEngine;
using UnityEngine.Events;

public class InteractableWithRequrement : MonoBehaviour, IInteractable
{
    [SerializeField] private string canPassTooltip;
    [SerializeField] private string onPassTooltip;
    public UnityEvent<Transform> OnPassEvent;
    public UnityEvent onPassEvent;
    public UnityEvent<Transform> OnFailEvent;
    public UnityEvent onFailEvent;
    [SerializeField] InteractRequrement[] requrements;

    private string tooltip;
    public string Tooltip => tooltip;

    public string InteractedTooltip => tooltip;

    public bool CanInteract => true;

    public bool ShowInteractMessage => true;

    public int InteractedDisplayPriority => 1;

    public float InteractedTipDisplayTime => 3;

    public void Interact(Transform interactor)
    {
        if (CheckRequirements(interactor))
        {
            tooltip = onPassTooltip;
            OnPassEvent?.Invoke(interactor);
            onPassEvent?.Invoke();
        }
        else
        {
            OnFailEvent?.Invoke(interactor);
            onFailEvent?.Invoke();
        }
    }

    public void SpeculateInteract(Transform interactor)
    {
        if (CheckRequirements(interactor))
        {
            tooltip = canPassTooltip;
        }
    }

    void Start()
    {
        
    }

    private bool CheckRequirements(Transform interactor)
    {
        if (requrements == null) return true;
        bool passed = true;
        for (int i = 0; i < requrements.Length; i++)
        {
            if (!requrements[i].Check(interactor))
            {
                tooltip = requrements[i].OnFailTooltip;
                passed = false;
                return passed;
            }
        }
        return passed;
    }

    [MakeButton]
    private void CollectRequirements()
    {
        requrements = GetComponents<InteractRequrement>();
    }

}