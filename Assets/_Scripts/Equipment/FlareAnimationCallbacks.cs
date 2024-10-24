using UnityEngine;

public class FlareAnimationCallbacks : MonoBehaviour
{

    [SerializeField] FlareThrower flareThrower;
    void Start()
    {
        if (!flareThrower) flareThrower = GetComponentInParent<FlareThrower>();
    }

    void Throw()
    {
        if (flareThrower)
        {
            flareThrower.ThrowFlare();
        }
    }

    void ThrowFinished()
    {
        if (flareThrower)
            flareThrower.ThrowFinished();
    }
}
