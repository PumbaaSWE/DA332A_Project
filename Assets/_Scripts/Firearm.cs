using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.InputSystem.InputAction;

public class Firearm : MonoBehaviour
{
    public Transform ShotOrigin;
    public int LoadedAmmo, MagazineSize, ReserveAmmo, MaxReserveAmmo;
    public int FireRate;

    public float VerticalRecoil, MaxHorizontalRecoilLeft, MaxHorizontalRecoilRight;
    public float MaxRange = 100;

    [SerializeField] bool RoundInTheChamber = true;
    [SerializeField] bool AutoReload = false;
    bool CanFire = true;
    bool Firing = false;
    [SerializeField] LayerMask ShootableLayers;
    [SerializeField] GameObject Decal;

    //[SerializeField] FireMode CurrentMode;
    //[SerializeField] []FireMode AvailableModes;

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
            if (LoadedAmmo > 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(ShotOrigin.position, ShotOrigin.forward, out hit, MaxRange))
                {
                    Debug.DrawLine(ShotOrigin.position, hit.point, Color.red);
                    Debug.Log($"Hit object {hit.collider.gameObject.name} at {hit.point}");
                    Instantiate(Decal, hit.point, new Quaternion());
                }

                else
                    Debug.DrawRay(ShotOrigin.position, ShotOrigin.forward, Color.red);

                LoadedAmmo--;
            }

            Debug.Log($"Mag:{LoadedAmmo} | Reserve: {ReserveAmmo}");
        }
    }

    public void AimDownSights(CallbackContext context)
    {

    }

    public void Reload(CallbackContext context)
    {
        if (context.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            if (ReserveAmmo == 0)
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
}

//public enum FireMode
//{
//    SemiAuto,
//    BurstFire,
//    FullAuto,
//    Manual
//}

//public enum AnimState
//{
//    Idle,
//    Firing,
//    Reloading,
//    Holstering,
//    PullingOut
//}