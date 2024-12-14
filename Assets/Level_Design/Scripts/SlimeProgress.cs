
using UnityEngine;
using UnityEngine.Events;


public class SlimeProgress : MonoBehaviour
{
    [SerializeField] int maxSlime = 10;
    [SerializeField] int slime = 0;


    public UnityEvent onMaxCollected;
    public UnityEvent onAlomstDone;
    public void AddSlime()
    {
        slime++;
    }

    void Update()
    {
        if (maxSlime >= slime)
        {
            onMaxCollected?.Invoke();
        }
        if ((maxSlime / 2) == slime)
        {
            onAlomstDone?.Invoke();
        }
        
    }
}
