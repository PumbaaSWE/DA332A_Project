using System;
using UnityEngine;
using UnityEngine.Events;

public class Limb : MonoBehaviour
{
    [SerializeField] private float timeToBeginRegrow = 5;
    [SerializeField] private float regrowTime = 5;
    [SerializeField] private LimbHealth limbHealth;

    public enum LimbState { Whole, Severed, Regrowing };
    LimbState state = LimbState.Whole;
    public LimbState State => state;

    public bool IsWhole => state == LimbState.Whole;

    private float time, timer;

    public UnityEvent<Limb> LimbSeveredEvent;
    public UnityEvent<Limb> LimbRegownEvent;

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        switch (state)
        {
            case LimbState.Whole:
                break;
            case LimbState.Severed:
                HandleSevered(dt);
                break;
            case LimbState.Regrowing:
                HandleRegrowing(dt);
                break;
            default:
                break;
        }
    }

    private void HandleSevered(float dt)
    {
        timer -= dt;
        if(timer <= 0)
        {
            RegrowLimb(regrowTime);
            if(limbHealth)
                limbHealth.HealFull(regrowTime);
        }
    }

    private void HandleRegrowing(float dt)
    {
        timer += dt;

        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer / time);

        if (timer >= time)
        {
            state = LimbState.Whole;
            transform.localScale = Vector3.one;
            LimbRegownEvent?.Invoke(this);
        }
    }

    public void SeverLimb()
    {
        if (state == LimbState.Severed) return;
        state = LimbState.Severed;
        transform.localScale = Vector3.zero;
        LimbSeveredEvent?.Invoke(this);
        timer = timeToBeginRegrow;
    }

    [MakeButton("Regrow", false)]
    public void RegrowLimb(float time)
    {
        if (state != LimbState.Severed) return;
        state = LimbState.Regrowing;

        this.time = time;
        timer = 0;
    }
}
