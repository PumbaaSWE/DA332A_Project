using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] ParticleSystem muzzleFlashEffect;
    [SerializeField] Light lightFlash;
    [SerializeField] float fadeTime;

    public void Awake()
    {
        Firearm firearm = GetComponentInParent<Firearm>();
        if (firearm)
        {
            firearm.OnFire += DoFlash;
        } 
    }

    private void OnDestroy()
    {
        Firearm firearm = GetComponentInParent<Firearm>();
        if (firearm)
        {
            firearm.OnFire -= DoFlash;
        }
    }

    public void DoFlash()
    {
        if (muzzleFlashEffect) muzzleFlashEffect.Play();
    }

    private void Update()
    {
        
    }
}
