using UnityEngine;

public abstract class InteractRequrement : MonoBehaviour
{
    [SerializeField] protected string tooltipOnFail = "Failed";
    public string OnFailTooltip => tooltipOnFail;

    public abstract bool Check(Transform interactor);
    
}
