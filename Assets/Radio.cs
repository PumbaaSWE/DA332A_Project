using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Health), typeof(Rigidbody))]
public class Radio : MonoBehaviour, IDamageble
{
    [Tooltip("How much is the radio Rigidbody affected by bullet hits")]
    [SerializeField] float forceMul = 1;
    [Tooltip("Particle System tp spawn when radio have no hp left")]
    [SerializeField] ParticleSystem destroyParticles;
    AudioSource source;
    Health health;
    Rigidbody rb;
 //   float damagedTime;
    ToggleButton toggle;
    
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        health = GetComponent<Health>();
        health.OnDeath += RadioDeath;
        rb = GetComponent<Rigidbody>();
        toggle = GetComponent<ToggleButton>();
    }

    private void RadioDeath(Health _)
    {
        if (destroyParticles)
        {
            ParticleSystem ps = Instantiate(destroyParticles, transform.position, transform.rotation);
            Destroy(ps.gameObject, ps.totalTime);
        }
        Destroy(gameObject);
    }

    public void Toggle()
    {
        SetPlaying(!source.isPlaying);
    }

    public void SetPlaying(bool playing)
    {
        if (playing)
        {
            source.Play();
        }
        else
        {
            source.Stop();
        }
    }

    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        //if (Time.time - damagedTime > 0.1f)
        //{

        //    damagedTime = Time.time;
        //}
        if(toggle)toggle.IsOn = false;
        SetPlaying(false);
        rb.AddForceAtPosition(direction * forceMul, point, ForceMode.Impulse);
        health.Damage(damage);
    }
}
