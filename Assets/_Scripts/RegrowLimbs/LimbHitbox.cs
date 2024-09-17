using UnityEngine;

public class LimbHitbox : MonoBehaviour
{
    
    [SerializeField]private LimbHealth limbHealth;
    public LimbHealth LimbHealth
    {
        get { return limbHealth; }
        set { limbHealth = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReciveHit(float damage)
    {
        limbHealth.TakeDamage(damage);
    }
}
