#if UNITY_EDITOR

using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

public static class UnityEventExtra
{
    public static void AddButDontDupe(UnityEvent<Limb> e, UnityAction<Limb> a)
    {
        int length = e.GetPersistentEventCount();
        string name = a.Method.Name;


        if (length == 0)
        {
            UnityEventTools.AddPersistentListener(e, a);
            return;
        }

        for (int i = 0; i < length; i++)
        {
            if (name.Equals(e.GetPersistentMethodName(i)))
            {
                //we have dupe
                Debug.Log("Listener Method added already: " + e.GetPersistentMethodName(i) + " is " + name);
            }
            else
            {
                UnityEventTools.AddPersistentListener(e, a);
            }
        }


    }
}
#endif
