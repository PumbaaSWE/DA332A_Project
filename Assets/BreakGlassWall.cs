using UnityEngine;

public class BreakGlassWall : MonoBehaviour, IDamageble
{

    Rigidbody[] rbs;
    Collider[] colliders;
    BoxCollider hitbox;
    [SerializeField] float health = 100;
    [SerializeField] float maxForce = 10;
    [SerializeField] float forceMultiplier = 1;
    [SerializeField] float timeToDisable = 5;
    [SerializeField] GameObject wall;
    [SerializeField] GameObject wallBroken;
    bool dead;
    
    public void TakeDamage(Vector3 point, Vector3 direction, float damage)
    {
        if(dead)return;
        health -= damage;
        if(health <= 0)
        {
            if (wall) wall.SetActive(false);
            if (wallBroken) wallBroken.SetActive(true);
            ExploadeWall(point, direction);
            dead = true;
            hitbox.enabled = false;
        }
    }

    private void ExploadeWall(Vector3 point, Vector3 direction)
    {
        for (int i = 0; i < rbs.Length; i++)
        {
            Rigidbody rb = rbs[i];
            Vector3 force = Vector3.ClampMagnitude(rb.position - point, 4);

            force *=  16 / Mathf.Max(1,force.sqrMagnitude);

            force += direction;

            rb.AddForce(Vector3.ClampMagnitude(force * forceMultiplier, maxForce), ForceMode.Impulse);
        }
        Invoke(nameof(DisableColliders), timeToDisable);
    }

    private void DisableColliders()
    {
        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].isKinematic = true;
        }
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    void Awake()
    {
        if (wall)
        {
            wall.SetActive(true);
            hitbox = wall.GetComponent<BoxCollider>();
        }

        if (wallBroken)
        {
            wallBroken.SetActive(false);
            colliders = wallBroken.GetComponentsInChildren<Collider>(true);
            rbs = wallBroken.GetComponentsInChildren<Rigidbody>(true);
        }
    }
}
