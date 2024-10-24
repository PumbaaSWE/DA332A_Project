using UnityEngine;

[CreateAssetMenu(fileName = "New EquipmentData", menuName = "ScriptableObjects/EquipmentData", order = 100)]
public class EquipmentData : ScriptableObject
{

    [SerializeField]private Equipment onPickUpPrefab;
    [SerializeField]private Equipment onDropPrefab;
    [SerializeField]private int equipmentId = 0;
    [SerializeField]private int slot = 0;
    [SerializeField]private string raiseState = "";
    [SerializeField]private float lowerTime = .5f;
    [SerializeField]private float raiseTime = .5f;
    [SerializeField]private bool animationEventLower = false;
    [SerializeField]private bool animationEventRaise = false;

    public Equipment OnPickUpPrefab => onPickUpPrefab;
    public Equipment OnDropPrefab => onDropPrefab;
    public int EquipmentId => equipmentId;
    public int Slot => slot;
    public float LowerTime => lowerTime;
    public float RaiseTime => raiseTime;
    public bool AnimationEventLower => animationEventLower;
    public bool AnimationEventRaise => animationEventRaise;





    public int RaiseStateHash { get; private set; }

    void OnEnable()
    {
        RaiseStateHash = Animator.StringToHash(raiseState);
    }
}
