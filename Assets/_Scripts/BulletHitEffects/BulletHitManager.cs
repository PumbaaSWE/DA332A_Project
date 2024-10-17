using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

public class BulletHitEffectManager : PersistentSingleton<BulletHitEffectManager>
{
    [Serializable]
    public sealed class MateralHit
    {
        public Material material;
        public AudioClip clip;
        public ParticleSystem effect;
    }
    //public MateralHit fallback;
    private ObjectPool<ParticleSystem> fallbackPool;

    public BulletHitEffectManagerData data;


    private Dictionary<Material, (AudioClip clip, ObjectPool<ParticleSystem> pool)> pools;
    //private Dictionary<Material, AudioClip> clips;

    EventBinding<BulletHitEvent> eventBinding;
    void Start()
    {
        if (data == null)
        {

            enabled = false;
            return;
        }
        Debug.Assert(data.effect != null, "No fallback assigned");



        fallbackPool = new ObjectPool<ParticleSystem>(() => Instantiate(data.effect, transform), OnGet, OnRelease, p => Destroy(p.gameObject));
        pools = new();
        if (data.MateralHitList != null)
        {
            for (int i = 0; i < data.MateralHitList.Length; i++)
            {
                MateralHit hit = data.MateralHitList[i];
                ObjectPool<ParticleSystem> pool = new ObjectPool<ParticleSystem>(() => Instantiate(hit.effect, transform), OnGet, OnRelease, p => Destroy(p.gameObject));
                pools.Add(hit.material, (hit.clip, pool));
            }
        }
    }

    private void OnRelease(ParticleSystem particleSystem)
    {
        particleSystem.Stop();
        particleSystem.gameObject.SetActive(false);
    }

    private void OnGet(ParticleSystem particleSystem)
    {
        particleSystem.gameObject.SetActive(true);
    }

    public void OnEnable()
    {
        eventBinding ??= new(OnHitEvent);
        EventBus<BulletHitEvent>.Register(eventBinding);
    }

    private void OnDisable()
    {
        EventBus<BulletHitEvent>.Deregister(eventBinding);
    }

    public void OnHitEvent(BulletHitEvent hitEvent)
    {
        HitEffectType type = hitEvent.hit.collider.GetComponentInParent<HitEffectType>();

        float scale = hitEvent.cartridgeType == Cartridgetype.ShotgunShell ? 0.4f : 1;

        if (type != null)
        {
            SpawnHitEffect(type.material, hitEvent.hit.point, hitEvent.hit.normal , scale);
            return;
        }

        if (hitEvent.hit.collider.TryGetComponent(out Renderer renderer))
        {
            SpawnHitEffect(renderer.sharedMaterial, hitEvent.hit.point, hitEvent.hit.normal, scale);
        }
        else
        {
            renderer = hitEvent.hit.collider.GetComponentInParent<SkinnedMeshRenderer>();
            if (renderer != null)
            {
                SpawnHitEffect(renderer.sharedMaterial, hitEvent.hit.point, hitEvent.hit.normal, scale);
            }
            else
            {
                SpawnHitEffect(null, hitEvent.hit.point, hitEvent.hit.normal, scale);
            }
        }
    }

    public void SpawnHitEffect(Material material, Vector3 position, Vector3 normal, float scale = 1)
    {
        ObjectPool<ParticleSystem> pool;
        AudioClip clip;
        if (material != null && pools.TryGetValue(material, out (AudioClip clip, ObjectPool<ParticleSystem> pool) pair))
        {
            pool = pair.pool;
            clip = pair.clip;
            if (clip == null)
            {
                clip = data.clip;
            }
        }
        else
        {
            pool = fallbackPool;
            clip = data.clip;
        }

        if (clip != null)
        {
            //EventBus<SoundClipEvent>.Raise(new SoundClipEvent()
            //{
            //    clip = clip,
            //    position = position
            //});
        }

        ParticleSystem system = pool.Get();

        system.transform.localScale = Vector3.one * scale;
        system.transform.position = position;
        system.transform.forward = normal;
        system.Play();
        StartCoroutine(ReturnParticleSystem(pool, system, system.main.duration - Time.deltaTime));
    }

    public IEnumerator ReturnParticleSystem(ObjectPool<ParticleSystem> pool, ParticleSystem system, float t)
    {
        yield return new WaitForSeconds(t);
        pool.Release(system);
    }
}

public struct BulletHitEvent : IEvent
{
    public RaycastHit hit;
    public Cartridgetype cartridgeType;
    //public float damage;
}