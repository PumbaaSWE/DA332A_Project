using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct GuidReference
{
    private GameObject cachedObject;
    private bool hasCache;

    [SerializeField] private SerializableGuid guid;
    public SerializableGuid Guid => guid;   

    public event Action<GameObject> OnGuidAdded;
    public event Action OnGuidRemoved;

    private readonly Action<GameObject> addDelegate;
    private readonly Action removeDelegate;


#if UNITY_EDITOR
#pragma warning disable 0414
    // decorate with some extra info in Editor so we can inform a user of what that GUID means
    [SerializeField]
    private string cachedName;
    [SerializeField]
    private SceneAsset cachedScene;
#pragma warning restore 0414
#endif

    public GameObject GameObject
    {
        get {
            if (hasCache)
            {
                return cachedObject; 
            }
            cachedObject = GuidManager.Resolve(guid, addDelegate, removeDelegate);
            hasCache = true;
            return cachedObject;
        }
    }

    public GuidReference(GuidComponent target)
    {
        guid = target.Guid;
        cachedObject = null;
        hasCache = false;
        OnGuidAdded = delegate (GameObject go) { };
        OnGuidRemoved = delegate () { };

        cachedName = string.Empty;
        cachedScene = null;

        //structs are stupid?
        addDelegate = null;
        removeDelegate = null;

        addDelegate = GuidAdded;
        removeDelegate = GuidRemoved;
    }

    private void GuidAdded(GameObject go)
    {
        cachedObject = go;
        OnGuidAdded(go);
    }

    private void GuidRemoved()
    {
        cachedObject = null;
        hasCache = false;
        OnGuidRemoved();
    }

}
