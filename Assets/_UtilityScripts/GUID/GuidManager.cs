using UnityEngine;
using System;
using System.Collections.Generic;

public class GuidManager
{
    // for each GUID we need to know the Game Object it references
    // and an event to store all the callbacks that need to know when it is destroyed
    private struct GuidInfo
    {
        public GameObject go;

        public event Action<GameObject> OnAdd;
        public event Action OnRemove;

        public GuidInfo(GuidComponent comp)
        {
            go = comp.gameObject;
            OnRemove = null;
            OnAdd = null;
        }

        public void HandleAddCallback()
        {
            OnAdd?.Invoke(go);
        }

        public void HandleRemoveCallback()
        {
            OnRemove?.Invoke();
        }
    }

    private static GuidManager instance;
    public static GuidManager Instance { 
        get
        {
            instance ??= new GuidManager();
            return instance;
        }
    }

    private readonly Dictionary<SerializableGuid, GuidInfo> guids;
    private GuidManager()
    {
        guids = new Dictionary<SerializableGuid, GuidInfo>();
    }

    public static bool Add(GuidComponent guidComponent)
    {
        return Instance.InternalAdd(guidComponent);
    }

    public static void Remove(SerializableGuid guid)
    {
        Instance.InternalRemove(guid);
    }

    public static GameObject Resolve(SerializableGuid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
    {
        return Instance.InternalResolve(guid, onAddCallback, onRemoveCallback);
    }

    public static GameObject Resolve(SerializableGuid guid, Action onDestroyCallback)
    {
        return Instance.InternalResolve(guid, null, onDestroyCallback);
    }

    public static GameObject Resolve(SerializableGuid guid)
    {
        return Instance.InternalResolve(guid, null, null);
    }

    private bool InternalAdd(GuidComponent guidComponent)
    {
        SerializableGuid guid = guidComponent.Guid;
        GuidInfo info = new(guidComponent);


        if (!guids.ContainsKey(guid))
        {
            guids.Add(guid, info);
            return true;
        }

        GuidInfo existingInfo = guids[guid];
        if (existingInfo.go != null && existingInfo.go != guidComponent.gameObject)
        {
            // normally, a duplicate GUID is a big problem, means you won't necessarily be referencing what you expect
            if (Application.isPlaying)
            {
                Debug.AssertFormat(false, guidComponent, "Guid Collision Detected between {0} and {1}.\nAssigning new Guid. Consider tracking runtime instances using a direct reference or other method.", (guids[guid].go != null ? guids[guid].go.name : "NULL"), (guidComponent != null ? guidComponent.name : "NULL"));
            }
            else
            {
                // however, at editor time, copying an object with a GUID will duplicate the GUID resulting in a collision and repair.
                // we warn about this just for pedantry reasons, and so you can detect if you are unexpectedly copying these components
                Debug.LogWarningFormat(guidComponent, "Guid Collision Detected while creating {0}.\nAssigning new Guid.", (guidComponent != null ? guidComponent.name : "NULL"));
            }
            return false;
        }
        // if we already tried to find this GUID, but haven't set the game object to anything specific, copy any OnAdd callbacks then call them
        existingInfo.go = info.go;
        existingInfo.HandleAddCallback();
        guids[guid] = existingInfo;
        return true;
    }



    private void InternalRemove(SerializableGuid guid)
    {
        if (guids.TryGetValue(guid, out GuidInfo info))
        {
            info.HandleRemoveCallback();
        }
        guids.Remove(guid);
    }

    // nice easy api to find a GUID, and if it works, register an on destroy callback
    // this should be used to register functions to cleanup any data you cache on finding
    // your target. Otherwise, you might keep components in memory by referencing them
    private GameObject InternalResolve(SerializableGuid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
    {
        //we sould really not add an Empty guid here?? there is no check for this...
        
        if (guids.TryGetValue(guid, out GuidInfo info))
        {
            if (onAddCallback != null)
            {
                info.OnAdd += onAddCallback;
            }

            if (onRemoveCallback != null)
            {
                info.OnRemove += onRemoveCallback;
            }
            guids[guid] = info;
            return info.go;
        }

        if (onAddCallback != null)
        {
            info.OnAdd += onAddCallback;
        }

        if (onRemoveCallback != null)
        {
            info.OnRemove += onRemoveCallback;
        }

        guids.Add(guid, info);

        return null;
    }

    public static void DebugPrint()
    {
        Debug.Log("***GuidManager contains " + Instance.guids.Count + " entries***");
        foreach (var item in Instance.guids)
        {
            if(item.Value.go == null)
            {
                Debug.Log($"GUID: {item.Key} has go == null");
            }
            else
            {
                Debug.Log($"GUID: {item.Key}\nGameObject name: {item.Value.go.name} Located in scene: {item.Value.go.scene.name}");
            }
        }
        Debug.Log("***GuidManager end debug print***");
    }
}