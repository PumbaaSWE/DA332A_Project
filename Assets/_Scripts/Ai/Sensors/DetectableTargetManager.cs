using System.Collections.Generic;
using UnityEngine;

public class DetectableTargetManager : MonoBehaviour
{
    public static DetectableTargetManager Instance { get; private set; }

    public List<DetectableTarget> AllTargets { get; private set; } = new List<DetectableTarget>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple DetectableTargetManager found. Destroying: " + gameObject.name);
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(DetectableTarget target)
    {
        if (!AllTargets.Contains(target))
        {
            AllTargets.Add(target);
        }
    }

    public void Deregister(DetectableTarget target)
    {
        if (AllTargets.Contains(target))
        {
            AllTargets.Remove(target);
        }
    }
}
