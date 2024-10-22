using UnityEngine;

public class FlareBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    public void PickUp(Transform obj)
    {
        if (obj.TryGetComponent(out FlareThrower flareThrower)) {
            flareThrower.NumFlares += 5;
            Destroy(gameObject);
        }
    }
}
