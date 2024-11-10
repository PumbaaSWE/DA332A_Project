using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNode : MonoBehaviour
{
    [SerializeField] int[] playMusicIds;
    [SerializeField] Method method;

    [Header("Distance")]
    [SerializeField] float radius = 9f;
    [SerializeField] float smoothing = 1f;

    [Header("Trigger")]
    [SerializeField] float transitionTime = 1f;

    public int[] MusicIds => playMusicIds;

    float t;

    private void OnEnable()
    {
        MusicManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        MusicManager.Instance.Deregister(this);
    }

    public float Ping(Vector3 pos)
    {
        switch (method)
        {
            case Method.distance:

                float dist = Vector3.Distance(transform.position, pos);
                return Mathf.InverseLerp(radius + smoothing, radius, dist);

            case Method.trigger:
                return t;
        }

        return 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (method != Method.trigger)
            return;

        if (!other.TryGetComponent(out Player _))
            return;

        StopAllCoroutines();
        StartCoroutine(Lerp(t, 1));
    }

    private void OnTriggerExit(Collider other)
    {
        if (method != Method.trigger)
            return;

        if (!other.TryGetComponent(out Player _))
            return;

        StopAllCoroutines();
        StartCoroutine(Lerp(t, 0));
    }

    IEnumerator Lerp(float from, float to)
    {
        while (t != to)
        {
            t = Mathf.Clamp(t + Mathf.Sign(to - from) * transitionTime * Time.deltaTime, 0, 1);

            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
    }

    public enum Method
    {
        distance,
        trigger
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (method != Method.distance)
            return;

        UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, radius);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius + smoothing);
    }
#endif
}
