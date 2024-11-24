
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DeathBehaviour : MonoBehaviour
{
    [SerializeField] PlayerDataSO playerData;
    [SerializeField] float minTime = 10;
    [SerializeField] float minDist = 10;
    protected Transform player;
    
    private void OnEnable()
    {
        GetComponent<Health>().OnDeath += OnDeath;
        if (playerData)
        {
            playerData.NotifyOnPlayerChanged(SetPlayer);
        }
    }


    private void OnDisable()
    {
        GetComponent<Health>().OnDeath -= OnDeath;
        if (playerData)
        {
            playerData.UnsubscribeOnPlayerChanged(SetPlayer);
        }
    }
    protected void SetPlayer(Transform transform)
    {
        player = transform;
    }

    protected void OnDeath(Health obj)
    {
        StartCoroutine(SinkCoroutine());
    }

    private IEnumerator SinkCoroutine()
    {
        yield return new WaitForSeconds(minTime);
        //Debug.Log("daed ready to destroy");
        bool stay = true;
        while (stay)
        {
            if (!player) break;
            Vector3 delta = transform.position - player.position;
            if(delta.sqrMagnitude > minDist * minDist && Vector3.Dot(player.forward, delta) < 0)
            {
                stay = false;
            }
            yield return null;
        }


        //float t = 2;
        //while (t > 0)
        //{
        //    float dt = Time.deltaTime;
        //    t -= dt;
        //    transform.position += Vector3.down * 0.5f * dt;
        //    yield return null;
        //}
        Destroy(gameObject);
    }
}
