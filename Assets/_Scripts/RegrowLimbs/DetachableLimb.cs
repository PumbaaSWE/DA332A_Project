
using UnityEngine;

public class DetachableLimb : MonoBehaviour
{
    [SerializeField] private GameObject disposableLimb;
    //[SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private Quaternion rotationOffset = Quaternion.identity;
    private bool detached;
    private DetachableLimb parent;
    private DetachableLimb child;
    private float timer;
    private float time;
    private void Awake()
    {
        if (transform.parent != null && transform.parent.TryGetComponent(out DetachableLimb detachable))
        {
            detachable.child = this;
            parent = detachable;
        }
        enabled = false;
    }

    public void Regrow(float t)
    {
        if (parent == null)
        {
            //we can regrow
            RegrowSelf(t);
        }
        else if (parent.detached)
        {
            parent.Regrow(t);
            //we cannot regrow
        }
        else
        {
            RegrowSelf(t);
        }
    }

    public void Detatch()
    {
        if (detached) return;
        detached = true;

        if (disposableLimb)
        {
            GameObject go = Instantiate(disposableLimb, transform.position, rotationOffset * transform.rotation);
            //if (go.TryGetComponent(out Rigidbody rb) && TryGetComponent(out Rigidbody originalRb))
            //{
            //    //commented off to check correct pos and rot

            //    //rb.velocity = originalRb.velocity;
            //    //rb.angularVelocity = originalRb.angularVelocity;
            //    //rb.AddForce(originalRb.GetAccumulatedForce());
            //    //rb.AddTorque(originalRb.GetAccumulatedTorque());
            //}
            Destroy(go, 30);
        }

        transform.localScale = Vector3.zero;
        if (child != null)
        {
            child.Detatch();
        }
    }

    private void RegrowSelf(float t)
    {
        enabled = true;
        time = t;
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / time;

        if (t > 1)
        {
            t = 1;
            enabled = false;
            detached = false;
            if (child != null)
            {
                child.Regrow(time);
            }

        }

        transform.localScale = Vector3.one * t;

    }

    public bool IsDetatched()
    {
        if (detached) return true;
        if (!detached && child == null) return false;
        return child.IsDetatched();
    }
}
