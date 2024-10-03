using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FallingShelf : MonoBehaviour
{
    [SerializeField] Vector3 localEndPos;
    [SerializeField] Vector3 localEndRot;
    [SerializeField] float length;
    [SerializeField] AnimationCurve posCurve;
    [SerializeField] AnimationCurve rotCurve;
    [SerializeField] UnityEvent OnDone;

    float t;
    Vector3 localStartPos;
    Vector3 localStartRot;

    private void OnEnable()
    {
        localStartPos = transform.localPosition;
        localStartRot = transform.localRotation.eulerAngles;
    }

    void Update()
    {
        t += Time.deltaTime;

        if (t >= length)
        {
            t = length;
            enabled = false;
            OnDone.Invoke();
        }

        float tPos = posCurve.Evaluate(t / length);
        transform.localPosition = Vector3.Lerp(localStartPos, localEndPos, tPos);

        float tRot = rotCurve.Evaluate(t / length);
        transform.localRotation = Quaternion.Lerp(Quaternion.Euler(localStartRot), Quaternion.Euler(localEndRot), tRot);
    }

    [MakeButton]
    void SetEndPos()
    {
        localEndPos = transform.localPosition;
    }

    [MakeButton]
    void SetEndRot()
    {
        localEndRot = transform.localRotation.eulerAngles;
    }
}
