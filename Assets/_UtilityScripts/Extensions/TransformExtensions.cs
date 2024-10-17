using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static void ForEveryChild(this Transform parent, System.Action<Transform> action)
    {
        for (var i = parent.childCount - 1; i >= 0; i--)
        {
            action(parent.GetChild(i));
        }
    }

    public static void EnableChildren(this Transform parent)
    {
        parent.ForEveryChild(child => child.gameObject.SetActive(true));
    }

    public static void DisableChildren(this Transform parent)
    {
        parent.ForEveryChild(child => child.gameObject.SetActive(false));
    }

    public static IEnumerable<Transform> Children(this Transform parent)
    {
        foreach (Transform child in parent)
        {
            yield return child;
        }
    }

    public static void Reset(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    public static void ResetLocalPositionAndRotation(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public static void SetPositionAndRotation(this Transform transform, Transform toCopy)
    {
        transform.SetPositionAndRotation(toCopy.position, toCopy.rotation);
    }

    public static void SetLocalPositionAndRotation(this Transform transform, Transform toCopy)
    {
        transform.SetLocalPositionAndRotation(toCopy.localPosition, toCopy.localRotation);
    }

    public static Transform AddChild(this Transform transform, string name)
    {
        GameObject gameObj = new GameObject(name);
        gameObj.transform.parent = transform;
        gameObj.transform.Reset();
        return gameObj.transform;
    }

    private static Transform FindChildRecursively(this Transform parent, string name)
    {
        Transform target = parent.Find(name);
        if (target != null) return target;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            target = child.FindChildRecursively(name);
            if (target != null) return target;
        }

        return null;
    }

}
