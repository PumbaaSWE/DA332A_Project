using UnityEngine;

public class LimbHitbox : MonoBehaviour, IDamageble
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

    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        limbHealth.TakeDamage(damage);
    }
}
