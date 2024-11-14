using UnityEngine;

public class SyrringeAnimationCallbacks : MonoBehaviour
{

    [SerializeField] SyrringeUser syrringeUser;
    void Start()
    {
        if (!syrringeUser) syrringeUser = GetComponentInParent<SyrringeUser>();
    }

    void Use()
    {
        if (syrringeUser)
        {
            syrringeUser.UseSyrringe();
        }
    }

    void UseFinished()
    {
        if (syrringeUser)
            syrringeUser.UseFinished();
    }
}
