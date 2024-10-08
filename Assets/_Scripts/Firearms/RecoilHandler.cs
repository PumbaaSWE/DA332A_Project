using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RecoilHandler : MonoBehaviour
{
    [SerializeField] MovementController Player;
    [SerializeField] WeaponHandler WHandler;
    public Vector2 RecoilImpulse;
    //public float ImpulseDuration = 0.5f;
    float CurrentImpulseTime;
    bool ImpulseActive = false;

    // Start is called before the first frame update
    void Start()
    {
        WHandler = GetComponent<WeaponHandler>();
        Player = GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (WHandler == null)
            return;

        if (WHandler.IsFiring())
        {
            float x = Mathf.Min(Player.LookDelta.x, 0);
            float y = Mathf.Min(Player.LookDelta.y, 0);

            if (RecoilImpulse.y < 0)
                y = Mathf.Max(Player.LookDelta.y, 0);

            AddImpluse(x, y, false);
        }

        RecoilImpulse.x = Mathf.Max(RecoilImpulse.x, 0);

        //if (!WHandler.IsFiring() && !ImpulseActive)
        //    StartCoroutine(Impulse());

        //else if (WHandler.IsFiring() && ImpulseActive)
        //    StopCoroutine(Impulse());

        //if (!WHandler.IsFiring() && CurrentImpulseTime < WHandler.EquippedGun.ImpulseDuration)
        //{
        //    // Lerp how much to rotate the player from impulse
        //    Vector2 impulse = new(Mathf.Lerp(0, RecoilImpulse.x, CurrentImpulseTime / WHandler.EquippedGun.ImpulseDuration), Mathf.Lerp(0, RecoilImpulse.y, CurrentImpulseTime / WHandler.EquippedGun.ImpulseDuration));

        //    // Roate player by the given 
        //    Player.Rotate(-(impulse.x - previousImpulse.x), -(impulse.y - previousImpulse.y));

        //    previousImpulse = impulse;

        //    CurrentImpulseTime += Time.deltaTime;
        //}
    }

    public void AddImpluse(Vector2 impulse, bool reset = true)
    {
        RecoilImpulse += impulse;

        if (reset)
        {
            CurrentImpulseTime = 0;
            StopAllCoroutines();
        }
    }

    public void AddImpluse(float x, float y, bool reset = true)
    {
        AddImpluse(new(x, y), reset);
    }

    IEnumerator Impulse()
    {
        //Debug.Log("Applying impulse");
        ImpulseActive = true;
        float currentImpulseTime = 0;
        float impulseDuration = WHandler.EquippedGun.ImpulseDuration;
        Vector2 previousImpulse = Vector2.zero;

        while (currentImpulseTime < impulseDuration /*&& !WHandler.IsFiring()*/)
        {
            // Lerp how much to rotate the player from impulse
            Vector2 impulse = new(Mathf.Lerp(0, RecoilImpulse.x + previousImpulse.x, currentImpulseTime / impulseDuration), Mathf.Lerp(0, RecoilImpulse.y + previousImpulse.y, currentImpulseTime / impulseDuration));

            // Roate player by the given 
            Player.Rotate(-(impulse.x - previousImpulse.x), -(impulse.y - previousImpulse.y));

            RecoilImpulse -= (impulse - previousImpulse);
            previousImpulse = impulse;

            currentImpulseTime += Time.deltaTime;

            yield return null;
        }

        //if (!WHandler.IsFiring())
            RecoilImpulse = Vector2.zero;

        ImpulseActive = false;
        //Debug.Log("Impulse done");
    }

    public void StartImpulse()
    {
        //Debug.Log("Starting impulse");
        //StopAllCoroutines();
        StartCoroutine(Impulse());
    }
}
