
using UnityEngine;
using UnityEngine.Events;

public class LimbHealth : MonoBehaviour
{
    

    [SerializeField] private float health;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private LimbHitbox[] hitboxes;

    public float Health => health;


    public UnityEvent LimbDeathEvent;
    public UnityEvent<float> LimbHealthEvent;


    private void Awake()
    {
        health = maxHealth;    
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        if(health <= 0)
        {
            return;
        }

        health -= amount;
        LimbHealthEvent?.Invoke(amount);

        if (health <= 0)
        {
            health = 0;
            LimbDeathEvent?.Invoke();
        }
    }

    [ContextMenu("Find Hitboxes In Children")]
    private void FindHitboxesInChildren()
    {
        hitboxes = GetComponentsInChildren<LimbHitbox>();
        Debug.Assert(hitboxes != null, "LimbHealth - No hitboxes found in children!");
        for (int i = 0; i < hitboxes.Length; i++)
        {
            //todo make hitboxes report here...
            hitboxes[i].LimbHealth = this;
        }
    }

}
