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
    [SerializeField] float HipFireSpread, MinHipFireSpread, MaxHipFireSpread, HipFireGain, HipFireDecay;
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
    [SerializeField] bool UseAdsSpread = false;
    [SerializeField] bool Ads = false;
    [SerializeField] LayerMask ShootableLayers;
    [SerializeField] GameObject Decal;
    [SerializeField] FPSController Player;

    [SerializeField] FireMode CurrentMode;
    [SerializeField] FireMode[] AvailableModes;

    // Start is called before the first frame update
    void Start()
    {
        LoadedAmmo = MagazineSize + Convert.ToInt32(RoundInTheChamber);
        HipFireSpread = MinHipFireSpread;
        OriginalFov = GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView;
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
                //Debug.Log($"Hit object {hit.collider.gameObject.name} at {hit.point}");
                Instantiate(Decal, hit.point, new Quaternion());
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

            HipFireSpread = Mathf.Lerp(startHipFireAngle, startHipFireAngle + HipFireGain, timeElapsed / recoilDuration);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

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

    public void Reload(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            if (ReserveAmmo == 0 || Firing)
                return;

            int returnedAmmo = Mathf.Clamp(LoadedAmmo - Convert.ToInt32(RoundInTheChamber), 0, LoadedAmmo);
            LoadedAmmo -= returnedAmmo;
            ReserveAmmo += returnedAmmo;

            int ammoToLoad = Mathf.Clamp(ReserveAmmo, 0, MagazineSize);
            LoadedAmmo += ammoToLoad;
            ReserveAmmo -= ammoToLoad;

            Debug.Log($"Mag:{LoadedAmmo} | Reserve: {ReserveAmmo}");
        }
    }

    public void ToggleFireMode(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            CurrentMode = AvailableModes[(Array.IndexOf(AvailableModes, CurrentMode) + 1) % AvailableModes.Length];
        }
    }
}

public enum FireMode
{
    SemiAuto,
    BurstFire,
    FullAuto
}

//public enum AnimState
//{
//    Idle,
//    Firing,
//    Reloading,
//    Holstering,
//    PullingOut
//}