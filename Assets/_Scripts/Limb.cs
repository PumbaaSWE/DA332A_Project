using UnityEngine;
using UnityEngine.Events;

public class Limb : MonoBehaviour
{
    public enum LimbState { Whole, Severed, Regrowing };
    LimbState state = LimbState.Whole;
    public LimbState State => state;

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
                break;
            case LimbState.Regrowing:
                HandleRegrowing(dt);
                break;
            default:
                break;
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
