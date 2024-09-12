using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PoolManager<T> : Singleton<PoolManager<T>> where T : Component //T is something Unity Object/ GameObject?
{

    protected ObjectPool<T> pool;
    [SerializeField] protected T defaultItem;
    [SerializeField] protected bool collectionCheck = true;
    [SerializeField] protected int size = 100;


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        if (!defaultItem) {
            GameObject gameObject = new GameObject(typeof(T).Name + " (auto created by PoolManager)");
            defaultItem = gameObject.AddComponent<T>();
            defaultItem.transform.SetParent(transform);
        }
        pool = new ObjectPool<T>(OnCreate, OnGet, OnRelease, ActionOnDestroy, collectionCheck, 100);
        OnAwake();
    }
    protected virtual void OnAwake() { }

    //OnDestroy was taken...
    protected virtual void ActionOnDestroy(T item)
    {
        Destroy(item);
    }

    protected virtual T OnCreate()
    {
        T item = Instantiate(defaultItem, transform);
        return item;
    }

    protected virtual void OnGet(T item)
    {
        item.gameObject.SetActive(true);
    }
    protected virtual void OnRelease(T item)
    {
        item.gameObject.SetActive(false);
    }

    public virtual void ReturnItem(T item, float time = 0)
    {
        if (time <= 0)
        {
            pool.Release(item);
        }
        else
        {
            StartCoroutine(ReleaseItem(item, time));
        }
    }

    protected IEnumerator ReleaseItem(T item, float t)
    {
        yield return new WaitForSeconds(t);
        pool.Release(item);
    }
}
