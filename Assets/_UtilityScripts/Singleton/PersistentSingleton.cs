using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;
    public bool AutoUnparentOnAwake = true;

    public static T Instance
    {
        get
        {
            //if (applicationIsQuitting)
            //{
            //    Debug.Log("PersistentSingleton is quitting?");
            //    return null;
            //}
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();
                if (instance == null)
                {
                    GameObject gameObject = new GameObject(typeof(T).Name + " (auto created)");
                    instance = gameObject.AddComponent<T>();
                }
            }
            return instance;
        }

    }
//    protected bool duplicate = false;

    protected virtual void Awake()
    {
        InitializeSingleton();
    }

    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying) return;
        if (AutoUnparentOnAwake)
        {
            transform.SetParent(null);
        }

        if (instance == null)
        {
            instance = this as T;
//            duplicate = false;
            DontDestroyOnLoad(gameObject);
        }else if(instance != this)
        {
 //           duplicate = true;
            Destroy(gameObject);
        }

    }

    //protected static bool applicationIsQuitting = false;
    ///// <summary>
    ///// When Unity quits, it destroys objects in a random order.
    ///// In principle, a Singleton is only destroyed when application quits.
    ///// If any script calls Instance after it have been destroyed,
    /////   it will create a buggy ghost object that will stay on the Editor scene
    /////   even after stopping playing the Application. Really bad!
    ///// So, this was made to be sure we're not creating that buggy ghost object.
    ///// </summary>
    //protected virtual void OnDestroy()
    //{
    //    if (!duplicate)
    //    {
    //        Debug.Log("PersistentSingleton OnDestroy?");
    //        applicationIsQuitting = true;
    //    }
    //}
}
