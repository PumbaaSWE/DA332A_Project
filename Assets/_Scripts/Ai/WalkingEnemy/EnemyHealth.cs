using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyHealth : MonoBehaviour, IDamageble
{
    public GameObject enemy;

    public float health = 1000;
    [SerializeField] float headShotDmgModifer = 1.2f;
    Animator animator;
    Ragdoll ragdoll;
    Regrow regrow;
    FSM_Walker fsm;

    [Range(0f, 1f)]
    public float chanceToHappen = 0.1f;


    private float leftLegHealth = 100f;
    private float rightLegHealth = 100f;
    private float leftArmHealth = 100f;
    private float rightArmHealth = 100f;
    private float headHealth = 100f;
    private float limbHealth = 100f;

    public Action<float> OnHealthChanged;
    public Action OnDeath;

    public bool dead;

    [SerializeField] private AudioSource dmgAudio;
    [SerializeField] private List<AudioClip> dmgClips;
    [SerializeField] private AudioClip deathClip;

    [SerializeField] GameObject headblodParticle;
    [SerializeField] GameObject rightArmblodParticle;
    [SerializeField] GameObject leftArmblodParticle;
    [SerializeField] GameObject rightLegblodParticle;
    [SerializeField] GameObject leftLegblodParticle;

    public GameObject damageEffectPrefab;

    public List<GameObject> drops;

    public List<DissolveEffect> dissolveEffects = new List<DissolveEffect>();

    public Material objectMaterial;
    public GameObject decalPrefab;
    public int headParticales = 10;
    Vector3 playerPos;
    float destroyTime = 5f;
    private void Awake()
    {
        fsm = GetComponent<FSM_Walker>();
         animator = GetComponent<Animator>();
        regrow = GetComponent<Regrow>();
        ragdoll = GetComponent<Ragdoll>();
       
    }

    private void Update()
    {
        if (health <= 0)
        {          
            Death();
         
        }

    }
  

    public void Death()
    {
        fsm.agentState = FSM_Walker.AgentState.Sleep;
        if (dead) return;//code after does not need to run every frame while dead?



        //ragdoll.TriggerRagdoll(new Vector3(0, 0.5f, 0), new Vector3(0, 0, 0));
        Destroy(enemy, destroyTime);
        ragdoll.state = Ragdoll.RagdollState.Ragdoll;
        regrow.canRegrow = false;

        dmgAudio.clip = deathClip;
        if (!dmgAudio.isPlaying && !dead)
        {
            dmgAudio.Play();
            dead = true;

        }
        foreach (var dis in dissolveEffects)
        {
            dis.death = true;
        }

        OnDeath?.Invoke();
        dead = true;
    }

    public void DropThing(Vector3 point, Quaternion rotation)
    {
      
        //if (UnityEngine.Random.value < chanceToHappen)
        {
            int randomIndex = UnityEngine.Random.Range(0, drops.Count);

            Instantiate(drops[randomIndex], point, rotation);
        }
    }
   
    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        OnHealthChanged?.Invoke(damage);

        //Vector3 localPoint = transform.InverseTransformPoint(point);

        //objectMaterial.SetVector("_ImpactPoint", new Vector4(localPoint.x, localPoint.y, localPoint.z, 1));
        //objectMaterial.SetColor("_ImpactColor", Color.red);
        //objectMaterial.SetFloat("_ImactRadius", 0.3f);



        //StartCoroutine(ResetImpactEffect());

        leftLegblodParticle.SetActive(false);
        rightLegblodParticle.SetActive(false);
        rightArmblodParticle.SetActive(false);
        leftArmblodParticle.SetActive(false);
        headblodParticle.SetActive(false);

        Quaternion rotation = Quaternion.LookRotation(-direction);

        Instantiate(damageEffectPrefab, point, rotation);
       

        health -= damage;
        Impact(direction, point);
        //Damage(damage);
        Detachable d = regrow.GetDetachable(point);
        if (d != null)
        {
            if (d.leftLeg)
            {
                leftLegHealth -= damage;
            }
            else if(d.rightLeg)
            {
                rightLegHealth -= damage;
            }
            else if(d.rightArm) 
            {
                rightArmHealth -= damage;
            }
            else if(d.leftArm)
            {
                leftArmHealth -= damage;
            }
            else if(d.head)
            {
                headHealth -= damage ;
                health -= (damage * headShotDmgModifer - damage);

            }
        }

        
       

        if (leftLegHealth <= 0)
        {
            leftLegblodParticle.SetActive(true);
            LoseLimbSound();
            regrow.Hit(point);
            if (d != null)
            {
                if (d.leftLeg)
                {
                    ragdoll.TriggerRagdoll(direction, point);
                }
            }
            if (regrow.canRegrow)
            {
                leftLegHealth = limbHealth;
            }

        }
        else if(rightLegHealth <= 0)
        {
            rightLegblodParticle.SetActive(true);
            LoseLimbSound();
            regrow.Hit(point);
            if (d != null)
            {
                if (d.leftLeg)
                {
                    ragdoll.TriggerRagdoll(direction, point);
                }
            }
            if (regrow.canRegrow)
            {
                rightLegHealth = limbHealth;
            }           
          
        }
        else if (rightArmHealth <= 0)
        {
            rightArmblodParticle.SetActive(true);
            LoseLimbSound();
            regrow.Hit(point);
            if (regrow.canRegrow)
            {
                rightArmHealth = limbHealth;
            }

            
        }
        else if (leftArmHealth <= 0)
        {
            leftArmblodParticle.SetActive(true);
            LoseLimbSound();
            regrow.Hit(point);
            if (regrow.canRegrow)
            {
                leftArmHealth = limbHealth;
            }
          
        }
        else if (headHealth <= 0)
        {
            headblodParticle.SetActive(true);
            LoseLimbSound();
            regrow.Hit(point);
            for (int i = 0; i < headParticales; i++)
            {
                DropThing(point, rotation);
            }
           
         
            if (regrow.canRegrow)
            {
                headHealth = limbHealth;
            }
        
        }

        if (health <= 0)
        {
            ragdoll.TriggerRagdoll(direction, point);
            regrow.Hit(point);
        }
        //dmgAudio.clip = dmgClips[1];
        //if (!dmgAudio.isPlaying)
        //{
        //    dmgAudio.Play();
        //}

        // PlayHitAnimation(direction);
        //Impact(direction, point);


    }
    private IEnumerator ResetImpactEffect()
    {
        yield return new WaitForSeconds(2.5f);
        objectMaterial.SetFloat("_ImpactRadius", 0.1f);
    }
    void LoseLimbSound()
    {
        dmgAudio.clip = dmgClips[0];
        if (!dmgAudio.isPlaying)
        {
            dmgAudio.Play();
        }
    }


    public void Impact(Vector3 from, Vector3 at)
    {
        Vector3 dir = transform.position - from;
        float height = at.y - transform.position.y;

        float fwd = -Vector3.Dot(dir, transform.forward);
        float right = -Vector3.Dot(dir, transform.right);
        float deg = 0.707f;
        if (fwd > deg)
        {

            if (height > 1.5f)
            {
                animator.Play("Standing React Small From Headshot", 1);
            }
            else
            {
                animator.Play("Standing React Large Gut", 1);

            }
        }
        if (fwd < -deg)
        {
            animator.Play("Standing React Small From Back", 1);
        }
        if (right > deg)
        {
            animator.Play("Standing React Large From Right", 1);
        }
        if (right < -deg)
        {
            animator.Play("Standing React Large From Left", 1);
        }

    }
}



