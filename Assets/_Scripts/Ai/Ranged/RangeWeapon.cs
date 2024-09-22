using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : MonoBehaviour
{
 
    public enum WeaponType { AK, Shotgun }

    [Header("Weapon Settings")]
    public WeaponType weaponType;       
    public int ammoCount;
    public GameObject projectilePrefab;
    public Transform firePoint;
    [Header("Weapon AK")]
    public int akMaxAmmo;                 
    public float akFireRate;               
    public float akReloadTime;
    public float akSpread = 0.05f;
    [Header("Weapon ShootGun")]
    public int SMaxAmmo;
    public float SFireRate;
    public float SReloadTime;
    public float shotgunSpread = 0.2f;


    private bool isReloading = false;    
    private float nextFireTime = 0f;
    private float fireRate;
    private float power = 300;
    private float reloadTime;
    private int maxAmmo;
    private float spread;

    private void Awake()
    {
        WeponType();
    }

   
    public void HandleWeaponAI(bool shoot, Vector3 target)
    {
        if (shoot && Time.time >= nextFireTime && !isReloading)
        {
            FireWeaponAtTarget(target);
        }
    }

    private void FireWeaponAtTarget(Vector3 target)
    {
        nextFireTime = Time.time + 1f / fireRate;

        if (ammoCount > 0)
        {
            ammoCount--;
        
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null && target != null)
            {
                Vector3 direction = (target - firePoint.position).normalized;
                direction = ApplySpread(direction);
                rb.AddForce(direction * power, ForceMode.VelocityChange);
            }
            Destroy(projectile, 2f);
            if (ammoCount <= 0)
            {
                StartCoroutine(Reload());
            }
        }
    }
    private Vector3 ApplySpread(Vector3 direction)
    {
        
        direction.x += Random.Range(-spread, spread);
        direction.y += Random.Range(-spread, spread);
        direction.z += Random.Range(-spread, spread);

        return direction.normalized; 
    }
    IEnumerator Reload()
    {
    
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        ammoCount = maxAmmo;
        isReloading = false;
  
    }

    void WeponType()
    {
        if (weaponType == WeaponType.AK)
        {
            fireRate = akFireRate;
            reloadTime = akReloadTime;
            maxAmmo = akMaxAmmo;
            spread = akSpread;
        }
        else if (weaponType == WeaponType.Shotgun)
        {
            fireRate = SFireRate;
            reloadTime = SReloadTime;
            maxAmmo = SMaxAmmo;
            spread = shotgunSpread;
        }
    }
}
