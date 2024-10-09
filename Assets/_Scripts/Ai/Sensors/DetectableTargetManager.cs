using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectableTargetManager :MonoBehaviour /*Singleton<DetectableTargetManager>*/
{
    //public static DetectableTargetManager Instance { get; private set; }

    public List<DetectableTarget> AllTargets { get; private set; } = new List<DetectableTarget>();

    //void Start()
    //{
    //    DontDestroyOnLoad(this.gameObject);
    //}

    public static DetectableTargetManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple DecteableTargetManager found. Destroying" + gameObject.name);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void Register(DetectableTarget target)
    {
        AllTargets.Add(target);
    }

    public void Deregister(DetectableTarget target)
    {
        AllTargets.Remove(target);
    }
}
