using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> MuzzleFlashEffects;
    [SerializeField] AudioSource GunShot;
    [SerializeField] Light lightFlash;
    [SerializeField] float fadeTime;
    [SerializeField] bool DisableOverlap;

    void Awake()
    {
        GetComponentInParent<WeaponHandler>().OnShoot.AddListener(DoFlash);
    }

    void OnEnable()
    {
        GetComponentInParent<WeaponHandler>().OnShoot.AddListener(DoFlash);
    }

    void OnDisable()
    {
        GetComponentInParent<WeaponHandler>().OnShoot.RemoveListener(DoFlash);
    }

    void OnDestroy()
    {
        GetComponentInParent<WeaponHandler>().OnShoot.RemoveListener(DoFlash);
    }

    public void DoFlash()
    {
        if (MuzzleFlashEffects.Count > 0)
        {
            if (DisableOverlap)
                GunShot.Play();

            else
                GunShot.PlayOneShot(GunShot.clip);

            MuzzleFlashEffects.ForEach(s => s.Play());
            //StopAllCoroutines();
            StartCoroutine(FlashFade());
        }
    }

    IEnumerator FlashFade()
    {
        lightFlash.enabled = true;

        yield return new WaitForSeconds(fadeTime);

        lightFlash.enabled = false;
    }
}