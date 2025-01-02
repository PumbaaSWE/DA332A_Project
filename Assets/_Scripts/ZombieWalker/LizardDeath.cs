using System;
using System.Collections;
using UnityEngine;

public class LizardDeath : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] Renderer _renderer;
    [SerializeField] float dissolveSpeed = 1;
    [SerializeField] float dissolveDelay = 1;

    Material material;

    private void Awake()
    {
        material = _renderer.material;
        if (!material.HasFloat("_DissolveAmount"))
        {
            material = null;
        }
    }

    private void OnEnable()
    {
        GetComponent<Health>().OnDeath += OnDeath;
        
    }


    private void OnDisable()
    {
        GetComponent<Health>().OnDeath -= OnDeath;
    }

    private void OnDeath(Health obj)
    {
        GetComponent<SimpleAgent>().enabled = false;
        GetComponent<WallClimber>().enabled = false;
        GetComponent<GroundSensor>().enabled = false;
        GetComponent<MoveTowardsController>().enabled = false;
        if(rb)
            rb.isKinematic = false;
        if(animator)
            animator.enabled = false;
        if(material)
            StartCoroutine(Dissolve());

        EventBus<DeathEvent>.Raise(new DeathEvent(transform.position));
    }

    private IEnumerator Dissolve()
    {
        yield return new WaitForSeconds(dissolveDelay);
        float t = 0;
        while (t < 1)
        {
            float dt = Time.deltaTime;
            t += dissolveSpeed * dt;
            material.SetFloat("_DissolveAmount", t);
            yield return null;
        }
    }
}
