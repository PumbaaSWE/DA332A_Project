using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.InputSystem.InputAction;
using Random = UnityEngine.Random;

public class Firearm : MonoBehaviour
{
    public Transform CameraView;
    public Transform ShotOrigin;
    public int LoadedAmmo, MagazineSize, ReserveAmmo, MaxReserveAmmo;
    public int RPM;
    int CurrentBurst;

    public float VerticalRecoil, MinHorizontalRecoil, MaxHorizontalRecoil;
    public float MaxRange = 100;

    [SerializeField] bool RoundInTheChamber = true;
    [SerializeField] bool AutoReload = false;
    bool CanFire = true;
    bool Firing = false;
    bool ADS = false;
    [SerializeField] LayerMask ShootableLayers;
    [SerializeField] GameObject Decal;
    [SerializeField] FPSController Player;

    [SerializeField] FireMode CurrentMode;
    [SerializeField] FireMode[] AvailableModes;

    // Start is called before the first frame update
    void Start()
    {
        LoadedAmmo = MagazineSize + Convert.ToInt32(RoundInTheChamber);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire(CallbackContext context)
    {
        if (context.started)
        {
            if (LoadedAmmo > 0 && CanFire)
            {
                Firing = true;
                StartCoroutine(Fire());
            }
        }

        else if (context.canceled)
        {
            Firing = false;
            CurrentBurst = 0;
        }
    }

    IEnumerator Fire()
    {
        if (CanFire && Firing)
        {
            RaycastHit hit;
            if (Physics.Raycast(ShotOrigin.position, ShotOrigin.forward, out hit, MaxRange))
            {
                Debug.DrawLine(ShotOrigin.position, hit.point, Color.red);
                Debug.Log($"Hit object {hit.collider.gameObject.name} at {hit.point}");
                Instantiate(Decal, hit.point, new Quaternion());
                Player.Rotate(VerticalRecoil,Random.Range(MinHorizontalRecoil,MaxHorizontalRecoil));
            }

            else
                Debug.DrawRay(ShotOrigin.position, ShotOrigin.forward, Color.red);

            LoadedAmmo--;
            Debug.Log($"Mag:{LoadedAmmo} | Reserve: {ReserveAmmo}");

            CanFire = false;
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

            StartCoroutine(Fire());
        }
    }

    public void AimDownSights(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            
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