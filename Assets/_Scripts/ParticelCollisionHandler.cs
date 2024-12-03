using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionHandler : MonoBehaviour
{
    public GameObject waterPuddlePrefab;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem ps = other.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            int numCollisionEvents = ps.GetCollisionEvents(gameObject, collisionEvents);

            for (int i = 0; i < numCollisionEvents; i++)
            {
                Vector3 collisionPoint = collisionEvents[i].intersection;

                Instantiate(waterPuddlePrefab, collisionPoint, Quaternion.identity);
            }
        }
    }
}