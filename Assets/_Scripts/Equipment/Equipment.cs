using UnityEngine;

public class Equipment : MonoBehaviour
{

    [SerializeField] protected EquipmentData _equipmentData;
    public EquipmentData EquipmentData => _equipmentData;
    protected Animator animator;
    protected PlayerControls input;

    #region Overrides
    public override bool Equals(object other)
    {
        if (other is Equipment otherEquip)
        {
            return _equipmentData == otherEquip._equipmentData;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return _equipmentData.GetHashCode();
    }

    public static bool operator ==(Equipment lhs, Equipment rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
            {
                return true;
            }
            return false;
        }
        return lhs.Equals(rhs);
    }
    public static bool operator !=(Equipment lhs, Equipment rhs)
    {
        return !(lhs == rhs);
    }
    #endregion

    public void Init(Animator animator)
    {
        this.animator = animator;
        input ??= new PlayerControls();
        OnInit();
    }

    public virtual void OnInit()
    {

    }

    public void EnableInput()
    {
        input.Enable();
    }

    public void DisableInput()
    {
        input.Disable();
    }

    public virtual void BeginLower()
    {
    }

    public virtual void CompletedLower()
    {
    }

    public virtual void Reselect()
    {
    }

    public virtual void BeginRaise()
    {
    }

    public virtual void CompletedRaise()
    {
    }
}
