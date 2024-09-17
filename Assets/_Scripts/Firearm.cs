using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firearm : MonoBehaviour
{
    public int CurrentMagazine, ReserveAmmo, MaxReserveAmmo;
    public int FireRate;

    //public float VerticalRecoil;
    //public float MaxHorizontalRecoil;

    //[SerializeField] FireMode CurrentMode;
    //[SerializeField] []FireMode AvailableModes;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {
        if (CurrentMagazine > 0)
        {
            
        }
    }

    public void AimDownSights()
    {

    }

    public void Reload()
    {

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