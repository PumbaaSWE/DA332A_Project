using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlasher : MonoBehaviour
{
    [SerializeField] Light FlashLight;
    [SerializeField] AudioSource GunShot;
    [SerializeField] ParticleSystem MuzzleFlash;
    [SerializeField] float FlashDuration;
    [SerializeField] bool DisableOverlap;

    void Awake()
    {
        gameObject.GetComponentInParent<WeaponHandler>().OnShoot.AddListener(EmitFlash);
    }

    public void EmitFlash()
    {
        if (DisableOverlap)
            GunShot.Play();

        else
            GunShot.PlayOneShot(GunShot.clip);

        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        MuzzleFlash.Play();
        FlashLight.enabled = true;

        yield return new WaitForSeconds(FlashDuration);

        FlashLight.enabled = false;
    }

    private void OnDestroy()
    {
        gameObject.GetComponentInParent<WeaponHandler>().OnShoot.RemoveListener(EmitFlash);
    }
}
