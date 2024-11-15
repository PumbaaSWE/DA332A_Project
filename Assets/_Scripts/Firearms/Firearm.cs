using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using Random = UnityEngine.Random;

public class Firearm : MonoBehaviour
{
    public string Name = "Gun Display Name";
    [Header("Shooting")]
    [SerializeField] float Damage;
    public int RPM;
    [SerializeField] float MaxRange = 100;
    [SerializeField] int ProjectilesPerShot = 1;
    bool CanFire = true;
    public bool Firing = false;

    [Header("Recoil")]
    [SerializeField] float VerticalRecoil;
    [SerializeField] float MinHorizontalRecoil, MaxHorizontalRecoil;
    [SerializeField] float RecoilDuration;
    public float ImpulseDuration = 0.3f;

    [Header("Ammo & Reloading")]
    public int LoadedAmmo;
    public int MagazineSize;
    public int ReserveAmmo, MaxReserveAmmo;
    int CurrentBurst;
    public bool RoundInTheChamber = true;
    [SerializeField] bool AutoReload = false;
    /// <summary>
    /// True: Ammo consumption per shot depends on how many projectiles are shot
    /// False: 1 Shot is allways consumed per shot, no matter how many projectiles
    /// </summary>
    [SerializeField] bool ProportionalAmmoConsumption = false;
    [SerializeField] bool UseLocalAmmoPool = false;
    bool IsReloading;

    [Header("Hipfire & ADS")]
    public float HipFireSpread;
    [SerializeField] float MinHipFireSpread, MaxHipFireSpread, HipFireGain, HipFireDecay;
    [SerializeField] float AdsSpread;
    [SerializeField] float AdsTime = 0.5f;
    public float AdsZoom;
    float OriginalFov;
    [SerializeField] bool UseAdsSpread = false;
    public bool Ads = false;
    public bool CanAds = true;
    /// <summary>
    /// How far the gun is being aim down the sights. 0 = not ads | 1 = fully ads
    /// </summary>
    public float AdsProcentage = 0;

    [Header("Other")]
    [SerializeField] LayerMask ShootableLayers;
    [SerializeField] GameObject Decal;
    [SerializeField] MovementController Player;

    public FireMode CurrentMode;
    [SerializeField] FireMode[] AvailableModes;
    public Cartridgetype AmmoType;
    [SerializeField] WeaponHandler WHandler;
    [SerializeField] RecoilHandler RHandler;
    Animator Animator;
    Action SwitchAction;
    Transform CameraView;
    public GameObject DropPrefab;
    public int Id;
    Animation CurrentAnimation;
    PlayerCamera Camera;
    NonphysController nc;
    [SerializeField] ParticleSystem CaseEjectorParticleSystem;
    [SerializeField] CaseEjector CaseEjector;

    public event Action OnFire;

    // Start is called before the first frame update
    void Start()
    {
        //LoadedAmmo = MagazineSize + Convert.ToInt32(RoundInTheChamber);
        HipFireSpread = MinHipFireSpread;
        OriginalFov = GameObject.Find("Main Camera").GetComponent<Camera>().fieldOfView;
        Animator = GetComponentInParent<Animator>();
        Camera = GetComponentInParent<PlayerCamera>();
        nc = GetComponentInParent<NonphysController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HipFireSpread > MinHipFireSpread && !Firing)
        {
            HipFireSpread = Mathf.Clamp(HipFireSpread - HipFireDecay * Time.deltaTime, MinHipFireSpread, MaxHipFireSpread);
        }

        bool CouldAds = CanAds;
        if (nc.IsSprinting || (!nc.Grounded && nc.VerticalVelocity > 0))
            CanAds = false;

        if (Ads && CanAds)
        {
            float t = Mathf.InverseLerp(0, AdsTime, Time.deltaTime);
            AdsProcentage = Mathf.Clamp(AdsProcentage + t, 0, 1);
        }

        else
        {
            float t = Mathf.InverseLerp(0, AdsTime, Time.deltaTime);
            AdsProcentage = Mathf.Clamp(AdsProcentage - t, 0, 1);
        }

        if (AdsProcentage > 0)
        {
            //Camera.main.fieldOfView = Mathf.Lerp(OriginalFov, OriginalFov / AdsZoom, AdsProcentage);
            Camera.SetZoom(Mathf.Lerp(1f, AdsZoom, AdsProcentage));

            //if (UseAdsSpread)
            //    HipFireSpread = Mathf.Lerp(OriginalFov, OriginalFov / AdsZoom, AdsProcentage);
        }

        Animator.SetInteger("RoundsLoaded", LoadedAmmo);
        Animator.SetInteger("FireMode", (int)CurrentMode);
        Animator.SetFloat("ADS", AdsProcentage);

        //Debug.Log($"Hipfire Angle {HipFireAngle}");
        CanAds = CouldAds;
    }

    public void Shoot(CallbackContext context)
    {
        if (context.started)
        {
            if (LoadedAmmo > 0 && CanFire/* && !nc.IsSprinting*/)
            {
                Firing = true;
                CanAds = true;
                IsReloading = false;
                StartCoroutine(Shoot());
            }
            
            else if (!IsReloading && LoadedAmmo == 0 && AutoReload && WHandler.AmmoLeft(AmmoType))
            {
                PerformAnimation(Animation.Reloading);
                CanAds = false;
                IsReloading = true;
                Firing = false;
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
            yield return new WaitForSeconds(60f / RPM);
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

            if (LoadedAmmo == 0 && AutoReload && WHandler.AmmoLeft(AmmoType))
            {
                PerformAnimation(Animation.Reloading);
                CanAds = false;
                IsReloading = true;
                Firing = false;
            }

            StartCoroutine(Shoot());
        }
    }

    void Fire()
    {
        int projectilesToFire = ProjectilesPerShot;

        if (ProportionalAmmoConsumption && projectilesToFire < LoadedAmmo)
            projectilesToFire = LoadedAmmo;
        OnFire?.Invoke();
        for (int x = 0; x < projectilesToFire; x++)
        {
            Vector3 shotDirection = CameraView.forward;
            Vector2 randomPoint = Random.insideUnitCircle;

            float spread;

            if (UseAdsSpread)
                spread = Mathf.Lerp(HipFireSpread, AdsSpread, AdsProcentage);

            else
                spread = Mathf.Lerp(HipFireSpread, 0, AdsProcentage);

            shotDirection = Quaternion.Euler(randomPoint.x * spread, randomPoint.y * spread, 0) * shotDirection;

            RaycastHit hit;
            if (Physics.Raycast(CameraView.position, shotDirection, out hit, MaxRange, ShootableLayers))
            {
                //Debug.DrawLine(CameraView.position, hit.point, Color.red, 10f);

                if (hit.collider.TryGetComponent(out IDamageble target))
                    target.TakeDamage(hit.point, shotDirection, Damage);

                else
                {
                    target = hit.collider.GetComponentInParent<IDamageble>();
                    if (target != null)
                        target.TakeDamage(hit.point, shotDirection, Damage);
                }

                EventBus<BulletHitEvent>.Raise(new BulletHitEvent()
                {
                    hit = hit,
                    cartridgeType = AmmoType
                    //reserved
                    //reserved
                });

                //Debug.Log($"Hit object {hit.collider.gameObject.name} at {hit.point}");
                //GameObject go = Instantiate(Decal, hit.point, new Quaternion());
                //Destroy(go, 1.0f);
            }

            //else
            //    Debug.DrawRay(CameraView.position, shotDirection, Color.red, 10f);
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

        RHandler.AddImpluse(new(xRecoil, yRecoil));

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

        //if (!Firing)
            RHandler.StartImpulse();
    }

    public void AimDownSights(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
            Ads = true;

        else if (context.canceled)
            Ads = false;
    }

    public void Reload()
    {
        if (UseLocalAmmoPool)
        {
            //if (ReserveAmmo == 0 || Firing)
            //    return;

            int returnedAmmo = Mathf.Clamp(LoadedAmmo - Convert.ToInt32(RoundInTheChamber), 0, LoadedAmmo);
            LoadedAmmo -= returnedAmmo;
            ReserveAmmo += returnedAmmo;

            int ammoToLoad = Mathf.Clamp(ReserveAmmo, 0, MagazineSize);
            LoadedAmmo += ammoToLoad;
            ReserveAmmo -= ammoToLoad;
        }

        else
        {
            //if (!WHandler.AmmoLeft(AmmoType) || Firing)
            //    return;

            int returnedAmmo = Mathf.Max(LoadedAmmo - Convert.ToInt32(RoundInTheChamber), 0);
            WHandler.AddAmmo(AmmoType, returnedAmmo);

            LoadedAmmo -= returnedAmmo;
            LoadedAmmo += WHandler.TakeAmmo(AmmoType, MagazineSize);
        }

        CanAds = true;
        IsReloading = false;
        WHandler.OnReloadEnd.Invoke();
    }

    public void Reload(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed && !Firing && WHandler.AmmoLeft(AmmoType) && !IsReloading)
        {
            //if (LoadedAmmo == 0)
            //{
            //    PerformAnimation(Animation.ReloadingEmpty);
            //    CanAds = false;
            //    IsReloading = true;
            //}

            if (LoadedAmmo < MagazineSize + Convert.ToInt32(RoundInTheChamber))
            {
                PerformAnimation(Animation.Reloading);
                CanAds = false;
                IsReloading = true;
            }
        }
    }

    public void LoadShells(int shellsToLoad)
    {
        if (UseLocalAmmoPool)
        {
            if (ReserveAmmo == 0 || Firing)
                return;

            int ammoToLoad = Mathf.Clamp(ReserveAmmo, 0, shellsToLoad);
            LoadedAmmo += ammoToLoad;
            ReserveAmmo -= ammoToLoad;
        }

        else
        {
            if (!WHandler.AmmoLeft(AmmoType) || Firing)
                return;

            LoadedAmmo += WHandler.TakeAmmo(AmmoType, shellsToLoad);
        }

        if (LoadedAmmo == MagazineSize + Convert.ToInt32(RoundInTheChamber))
        {
            Animator.SetTrigger("ReloadFinished");
            IsReloading = false;
            CanAds = true;
            CanFire = true;
            WHandler.OnReloadEnd.Invoke();
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
        //PerformAnimation(Animation.PullingOut);
        IsReloading = false;
        CanFire = true;
        CanAds = true;
    }

    public void Unequip(Action equip)
    {
        //Debug.Log("Holstering");
        // Play animation of putting away gun
        PerformAnimation(Animation.Holstering);
        SwitchAction = equip;
        CanFire = false;
        IsReloading = false;
        CanAds = false;
    }

    public void Switch()
    {
        //Debug.Log("Switching weapons");
        CanFire = false;
        CanAds = false;
        SwitchAction.Invoke();
        gameObject.SetActive(false);
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
                Animator.SetTrigger("ReloadEmpty");
                break;
            case Animation.Holstering:
                Animator.SetTrigger("Holster");
                break;
        }
    }

    public void Set(WeaponHandler wHandler, RecoilHandler rHandler, MovementController player)
    {
        WHandler = wHandler;
        RHandler = rHandler;
        CameraView = wHandler.GetComponentInChildren<Camera>().transform;
        Player = player;
    }

    public void SetCanFire(int canFire)
    {
        CanFire = Convert.ToBoolean(canFire);
    }

    public void SetCanAds(int canAds)
    {
        CanAds = Convert.ToBoolean(canAds);
    }

    public void EjectCasing()
    {
        //CaseEjectorParticleSystem.Emit(1);
        CaseEjector.Eject();
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