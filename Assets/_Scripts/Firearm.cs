using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.InputSystem.InputAction;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using Random = UnityEngine.Random;

public class Firearm : MonoBehaviour
{
    public Transform CameraView;
    public Transform ShotOrigin;
    public int LoadedAmmo, MagazineSize, ReserveAmmo, MaxReserveAmmo;
    public int RPM;
    int CurrentBurst;

    [SerializeField] float VerticalRecoil, MinHorizontalRecoil, MaxHorizontalRecoil;
    public float AdsZoom, OriginalFov;
    [SerializeField] float Damage;
    [SerializeField] float MaxRange = 100;
    [SerializeField] int ProjectilesPerShot = 1;
    public float HipFireSpread;
    [SerializeField] float MinHipFireSpread, MaxHipFireSpread, HipFireGain, HipFireDecay;
    [SerializeField] float AdsSpread;
    [SerializeField] float RecoilDuration;
    public Vector2 RecoilImpulse;
    [SerializeField] float ImpulseDuration;

    [SerializeField] bool RoundInTheChamber = true;
    [SerializeField] bool AutoReload = false;
    bool CanFire = true;
    bool Firing = false;
    /// <summary>
    /// True: Ammo consumption per shot depends on how many projectiles are shot
    /// False: 1 Shot is allways consumed per shot, no matter how many projectiles
    /// </summary>
    [SerializeField] bool ProportionalAmmoConsumption = false;
    [SerializeField] bool UseLocalAmmoPool;
    /// <summary>
    /// True: Reload magazine one cartridge at a time
    /// False: Reload whole magazine at once
    /// </summary>
    [SerializeField] bool IndividualReloading;
    [SerializeField] bool UseAdsSpread = false;
    [SerializeField] bool Ads = false;
    [SerializeField] LayerMask ShootableLayers;
    [SerializeField] GameObject Decal;
    [SerializeField] FPSController Player;

    [SerializeField] FireMode CurrentMode;
    [SerializeField] FireMode[] AvailableModes;
    [SerializeField] Cartridgetype AmmoType;
    [SerializeField] WeaponHandler Handler;
    Animator Animator;
    Action SwitchAction;

    // Start is called before the first frame update
    void Start()
    {
        LoadedAmmo = MagazineSize + Convert.ToInt32(RoundInTheChamber);
        HipFireSpread = MinHipFireSpread;
        OriginalFov = GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView;
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HipFireSpread > MinHipFireSpread && !Firing)
        {
            HipFireSpread = Mathf.Clamp(HipFireSpread - HipFireDecay * Time.deltaTime, MinHipFireSpread, MaxHipFireSpread);
        }

        //Debug.Log($"Hipfire Angle {HipFireAngle}");
    }

    public void Shoot(CallbackContext context)
    {
        if (context.started)
        {
            if (LoadedAmmo > 0 && CanFire)
            {
                Firing = true;
                StartCoroutine(Shoot());
            }
        }

        else if (context.canceled)
        {
            Firing = false;
            CurrentBurst = 0;
        }
    }

    IEnumerator Shoot()
    {
        if (CanFire && Firing)
        {
            Fire();
            PerformAnimation(Animation.Firing);

            //Debug.Log($"Mag:{LoadedAmmo} | Reserve: {ReserveAmmo}");

            CanFire = false;
            //Player.Rotate(VerticalRecoil, Random.Range(MinHorizontalRecoil, MaxHorizontalRecoil));
            StopCoroutine(Recoil());
            StartCoroutine(Recoil());
            yield return new WaitForSeconds(60f / (float)(RPM));
            CanFire = true;

            if (Firing)
                switch (CurrentMode)
                {
                    case FireMode.SemiAuto:
                        Firing = false;
                        break;
                    case FireMode.BurstFire:
                        Firing = (++CurrentBurst < 3 && LoadedAmmo > 0);
                        break;
                    case FireMode.FullAuto:
                        Firing = LoadedAmmo > 0;
                        break;
                }

            StartCoroutine(Shoot());
        }
    }

    void Fire()
    {
        int projectilesToFire = ProjectilesPerShot;

        if (ProportionalAmmoConsumption && projectilesToFire < LoadedAmmo)
            projectilesToFire = LoadedAmmo;

        for (int x = 0; x < projectilesToFire; x++)
        {
            Vector3 shotDirection = ShotOrigin.forward;

            if (!Ads)
            {
                Vector2 randomPoint = Random.insideUnitCircle;
                shotDirection = Quaternion.Euler(randomPoint.x * HipFireSpread, randomPoint.y * HipFireSpread, 0) * shotDirection;
            }

            else if (UseAdsSpread)
            {
                Vector2 randomPoint = Random.insideUnitCircle;
                shotDirection = Quaternion.Euler(randomPoint.x * AdsSpread, randomPoint.y * AdsSpread, 0) * shotDirection;
            }

            RaycastHit hit;
            if (Physics.Raycast(ShotOrigin.position, shotDirection, out hit, MaxRange))
            {
                Debug.DrawLine(ShotOrigin.position, hit.point, Color.red, 10f);

                if (hit.collider.TryGetComponent(out IDamageble target))
                    target.TakeDamage(hit.point, shotDirection, Damage);

                else //bonus...
                {
                    target = hit.collider.GetComponentInParent<IDamageble>();
                    if (target != null)
                        target.TakeDamage(hit.point, shotDirection, Damage);
                }

                //Debug.Log($"Hit object {hit.collider.gameObject.name} at {hit.point}");
                GameObject go = Instantiate(Decal, hit.point, new Quaternion());
                Destroy(go, 1.0f);
            }

            else
                Debug.DrawRay(ShotOrigin.position, shotDirection, Color.red, 10f);
        }

        if (ProportionalAmmoConsumption)
            LoadedAmmo -= projectilesToFire;

        else
            LoadedAmmo--;
    }

    IEnumerator Recoil()
    {
        float recoilDuration = RecoilDuration;
        float timeElapsed = 0f;

        float xRecoil = VerticalRecoil;
        float yRecoil = Random.Range(MinHorizontalRecoil, MaxHorizontalRecoil);
        float previousXRecoil = 0;
        float previousYRecoil = 0;
        float startHipFireAngle = HipFireSpread;

        while (timeElapsed < recoilDuration)
        {
            Vector2 recoil = new(Mathf.Lerp(0, xRecoil, timeElapsed / recoilDuration), Mathf.Lerp(0, yRecoil, timeElapsed / recoilDuration));

            Player.Rotate(recoil.x - previousXRecoil, recoil.y - previousYRecoil);

            previousXRecoil = recoil.x;
            previousYRecoil = recoil.y;

            //if (!Ads)
                HipFireSpread = Mathf.Lerp(startHipFireAngle, startHipFireAngle + HipFireGain, timeElapsed / recoilDuration);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        //if (!Ads)
            HipFireSpread = Mathf.Clamp(startHipFireAngle + HipFireGain, MinHipFireSpread, MaxHipFireSpread);

        Player.Rotate(xRecoil - previousXRecoil, yRecoil - previousYRecoil);
    }

    public void AimDownSights(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            Ads = true;
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView = OriginalFov / AdsZoom;
        }

        else if (context.canceled)
        {
            Ads = false;
            GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView = OriginalFov;
        }
    }

    public void Reload()
    {
        if (UseLocalAmmoPool)
        {
            if (ReserveAmmo == 0 || Firing)
                return;

            int returnedAmmo = Mathf.Clamp(LoadedAmmo - Convert.ToInt32(RoundInTheChamber), 0, LoadedAmmo);
            LoadedAmmo -= returnedAmmo;
            ReserveAmmo += returnedAmmo;

            int ammoToLoad = Mathf.Clamp(ReserveAmmo, 0, MagazineSize);
            LoadedAmmo += ammoToLoad;
            ReserveAmmo -= ammoToLoad;
        }

        else
        {
            if (!Handler.AmmoLeft(AmmoType) || Firing)
                return;

            int returnedAmmo = Mathf.Clamp(LoadedAmmo - Convert.ToInt32(RoundInTheChamber), 0, LoadedAmmo);
            LoadedAmmo -= returnedAmmo;
            Handler.AddAmmo(AmmoType, returnedAmmo);

            LoadedAmmo += Handler.TakeAmmo(AmmoType, MagazineSize);
        }
    }

    public void Reload(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed && !Firing && Handler.AmmoLeft(AmmoType))
        {
            if (LoadedAmmo == 0)
                PerformAnimation(Animation.ReloadingEmpty);

            else
                PerformAnimation(Animation.Reloading);

            //Reload();
        }
    }

    public void ToggleFireMode(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            CurrentMode = AvailableModes[(Array.IndexOf(AvailableModes, CurrentMode) + 1) % AvailableModes.Length];
        }
    }

    public void Equip()
    {
        //Debug.Log("Pulling out");
        // Play animation of pulling up gun
        gameObject.SetActive(true);
        PerformAnimation(Animation.PullingOut);
    }

    public void Unequip(Action equip)
    {
        //Debug.Log("Holstering");
        // Play animation of putting away gun
        PerformAnimation(Animation.Holstering);
        SwitchAction = equip;
    }

    void Switch()
    {
        //Debug.Log("Switching weapons");
        SwitchAction.Invoke();
        gameObject.SetActive(false);
    }

    public void Pickup()
    {
        // Pickup Gun into inventory
    }

    public void Drop()
    {
        // Drop gun onto floor
    }

    void PerformAnimation(Animation animation)
    {
        switch (animation)
        {
            case Animation.Firing:
                Animator.SetTrigger("Shoot");
                break;
            case Animation.Reloading:
                Animator.SetTrigger("Reload");
                break;
            case Animation.ReloadingEmpty:
                Animator.SetTrigger("Reload Empty");
                break;
            case Animation.Holstering:
                Animator.SetTrigger("Holster");
                break;
        }
    }
}

public enum FireMode
{
    SemiAuto,
    BurstFire,
    FullAuto
}

public enum Animation
{
    Idle,
    Firing,
    Reloading,
    ReloadingEmpty,
    Holstering,
    PullingOut
}