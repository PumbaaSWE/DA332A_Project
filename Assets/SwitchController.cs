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

    ClimbController climb;
    NonphysController nonphys;
    CapsuleCollider cc;

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
            nonphys.enabled = false;
            endPos = transform.position.WithY(transform.position.y + climb.Radius);
        }
        else
        {
            climb.enabled = false;
            endPos = transform.position.WithY(transform.position.y - climb.Radius);
        }


    }

    void Start()
    {
        climb = GetComponent<ClimbController>();
        nonphys = GetComponent<NonphysController>();
        cc = GetComponent<CapsuleCollider>();
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

        cc.center = Vector3.Lerp(Vector3.zero, Vector3.up * nonphys.Height / 2f, t);
        cc.height = Mathf.Lerp(climb.Radius, nonphys.Height, t);
        cc.radius = Mathf.Lerp(climb.Radius, nonphys.Radius, t);
        head.localPosition = Vector3.Lerp(Vector3.up * climb.CamOffset, Vector3.up * (nonphys.Height - nonphys.CamOffset), t);

        if (!wantClimb)
            transform.MatchUp(Vector3.Slerp(transform.up, Vector3.up, t));

        if (time == speed)
        {
            transitioning = false;

            if (wantClimb)
            {
                climb.enabled = true;
                Vector2 rot = nonphys.Rotation();
                climb.SetRotation(-rot.x, rot.y);
                climb.SetVelocity(Vector3.zero);
            }
            else
            {
                nonphys.enabled = true;
                Vector2 rot = climb.Rotation();
                nonphys.SetRotation(-rot.x, rot.y);
                nonphys.SetVelocity(Vector3.zero);
            }
        }
    }
}
