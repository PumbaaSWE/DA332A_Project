using UnityEngine;

/// <summary>
/// If you override Awake call InitializeSingleton() there first!
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindAnyObjectByType<T>();
                if(instance == null)
                {
                    GameObject gameObject = new GameObject(typeof(T).Name + " (auto created)");
                    instance = gameObject.AddComponent<T>();
                }
            }
            return instance;
        }

    }


    protected virtual void Awake()
    {
        InitializeSingleton();
    }

    protected virtual void InitializeSingleton()
    {
        if(!Application.isPlaying) return;
        instance = this as T;
    }
}
