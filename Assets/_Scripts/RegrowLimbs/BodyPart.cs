using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(DetachableLimb))]
public class BodyPart : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float timeBeforeRegrow = 5;
    [SerializeField] private float regrowTime = 1;

    private DetachableLimb detachableLimb;
    private Health healthScript;
    private bool doRegow = true;

    public event Action<bool> OnDetach;
    public event Action OnRegrown;
    public float Health
    {
        get { return health; }
        set { health = Mathf.Min(maxHealth, value); }
    }

    public float TimeBeforeRegrow
    {
        get { return timeBeforeRegrow; }
        set { timeBeforeRegrow = value; }
    }
    public float RegrowTime
    {
        get { return regrowTime; }
        set { regrowTime = value; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public bool IsDetached => detachableLimb.IsDetatched();

    void Awake()
    {
        health = maxHealth;
        doRegow = true;
        healthScript = GetComponentInParent<Health>();
        if (healthScript)
        {
            healthScript.OnDeath += OnDeath;
        }
        var hbs = GetComponentsInChildren<Hitbox>();
        foreach (var hb in hbs)
        {
            hb.OnHit += Hb_OnHit;
        }
        detachableLimb = GetComponent<DetachableLimb>();

    }

    private void OnDeath(Health obj)
    {
        StopAllCoroutines();
        doRegow = false;
    }

    private void OnDisable()
    {

    }

    private void Hb_OnHit(Vector3 point, Vector3 dir, float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            detachableLimb.Detatch();
            if(doRegow)StartCoroutine(RegrowIn(timeBeforeRegrow));
            OnDetach?.Invoke(doRegow);
        }
    }

    private IEnumerator RegrowIn(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        //if()
        detachableLimb.Regrow(regrowTime);
        float healed = 0;
        float healAmountPerSec  = maxHealth / regrowTime;
        while (healed < maxHealth)
        {
            float healAmount = healAmountPerSec * Time.deltaTime;
            health += healAmount;
            healed += healAmount;
            yield return null;
        }
        health = Mathf.Min(maxHealth, health);
        OnRegrown?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
