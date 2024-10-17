using UnityEngine;
[CreateAssetMenu(fileName = "NewBulletHitEffectManagerData", menuName = "WeaponStuff/BulletHitEffectManagerData", order = 30)]
public class BulletHitEffectManagerData : ScriptableObject
{
    [Header("Fallback")]
    public AudioClip clip;
    public ParticleSystem effect;
    [Header("List")]
    public BulletHitEffectManager.MateralHit[] MateralHitList;
}