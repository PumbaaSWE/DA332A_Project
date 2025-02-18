using UnityEngine;

public class Flare : MonoBehaviour
{

    Light m_light;
    [SerializeField] Rigidbody _rigidbody;

    public float timeToLive;
    public float fadeTime = 2;
    public float flickerSpeed = 1;
    public float lightRangeMax = 10;
    public float lightRangeMin = 8;

    public float lightIntensityMax = 1.5f;
    public float lightIntensityMin = .5f;

    public Color lightColor1 = Color.red;
    public Color lightColor2 = Color.yellow;

    // Start is called before the first frame update
    void Awake()
    {
        m_light = GetComponentInChildren<Light>();
    }

    public void Throw(Vector3 force, Vector3 torque)
    {
        _rigidbody.AddForce(force, ForceMode.Impulse);
        _rigidbody.AddTorque(torque, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        float m = Mathf.Min(timeToLive / fadeTime, 1);

        float t = Mathf.PerlinNoise1D(Time.time * flickerSpeed);
        m_light.range = (t * (lightRangeMax - lightRangeMin)) + lightRangeMin;
        m_light.intensity = (lightIntensityMin + (t * (lightIntensityMax - lightIntensityMin))) * m;
        m_light.color = Color.Lerp(lightColor1, lightColor2, t);

        timeToLive -= Time.deltaTime;
        if(timeToLive < 0)
        {
            Destroy(gameObject);
        }
    }
}
