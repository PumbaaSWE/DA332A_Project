using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public float speed;
    public bool wantClimb;
    public bool transitioning;
    public Transform head;

    ClimbController ClimbController;
    NonphysController NonphysController;
    CapsuleCollider CapsuleCollider;
    WeaponHandler WeaponHandler;
    Firearm _equippedGun;

    Vector3 startPos;
    Vector3 endPos;

    float time;

    [MakeButton]
    public void Transition()
    {
        if (!enabled)
            return;

        if (transitioning)
            return;

        wantClimb = !wantClimb;
        time = 0;
        transitioning = true;

        startPos = transform.position;
        if (wantClimb)
        {
            NonphysController.enabled = false;
            endPos = transform.position.WithY(transform.position.y + ClimbController.Radius);

            // disable weapon
            if (WeaponHandler != null)
            {
                if (WeaponHandler.HasGun())
                {
                    _equippedGun = WeaponHandler.EquippedGun;
                    _equippedGun.gameObject.SetActive(false);
                    WeaponHandler.EquippedGun = null;
                }

                WeaponHandler.SetCanPickup(false);
                WeaponHandler.enabled = false;
            }
        }
        else
        {
            ClimbController.enabled = false;
            endPos = transform.position.WithY(transform.position.y - ClimbController.Radius);
        }


    }

    void Start()
    {
        ClimbController = GetComponent<ClimbController>();
        NonphysController = GetComponent<NonphysController>();
        CapsuleCollider = GetComponent<CapsuleCollider>();
        WeaponHandler = GetComponent<WeaponHandler>();
    }

    void Update()
    {
        if (!transitioning)
            return;

        time += Time.deltaTime;

        if (time > speed)
            time = speed;

        // from climb to phys
        float t = time / speed;
        transform.position = Vector3.Lerp(startPos, endPos, t);

        if (wantClimb)
            t = (speed - time) / speed;

        CapsuleCollider.center = Vector3.Lerp(Vector3.zero, Vector3.up * NonphysController.Height / 2f, t);
        CapsuleCollider.height = Mathf.Lerp(ClimbController.Radius, NonphysController.Height, t);
        CapsuleCollider.radius = Mathf.Lerp(ClimbController.Radius, NonphysController.Radius, t);
        head.localPosition = Vector3.Lerp(Vector3.up * ClimbController.CamOffset, Vector3.up * (NonphysController.Height - NonphysController.CamOffset), t);

        if (!wantClimb)
            transform.MatchUp(Vector3.Slerp(transform.up, Vector3.up, t));

        if (time == speed)
        {
            transitioning = false;

            if (wantClimb)
            {
                ClimbController.enabled = true;
                Vector2 rot = NonphysController.Rotation();
                ClimbController.SetRotation(-rot.x, rot.y);
                ClimbController.SetVelocity(Vector3.zero);
            }
            else
            {
                NonphysController.enabled = true;
                Vector2 rot = ClimbController.Rotation();
                NonphysController.SetRotation(-rot.x, rot.y);
                NonphysController.SetVelocity(Vector3.zero);

                // enable weapon
                if (WeaponHandler != null)
                {
                    if (WeaponHandler.HasGun())
                    {
                        WeaponHandler.EquippedGun = _equippedGun;
                        _equippedGun.gameObject.SetActive(true);
                    }
                    
                    WeaponHandler.SetCanPickup(true);
                    WeaponHandler.enabled = true;
                }
            }
        }
    }
}
