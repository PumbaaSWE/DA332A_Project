using System.Collections;
using UnityEngine;

public class SlideObject : MonoBehaviour
{
    [SerializeField] private GameObject slidingObject;
    //[SerializeField] private Vector3 slideDelta;
    [SerializeField] private Vector3 slideStart = Vector3.zero;
    [SerializeField] private Vector3 slideEnd = Vector3.zero;
    [SerializeField] private float slideTime = 1;

    [SerializeField][Range(0, 1)] private float slidePosition = 0;
    [SerializeField] private AnimationCurve slideCurve;

    private void Start()
    {
        SetSlidePos(slidePosition);
    }

    [MakeButton]
    public void SetStartPos()
    {
        slideStart = slidingObject.transform.localPosition;
    }
    [MakeButton]
    public void SetEndPos()
    {
        slideEnd = slidingObject.transform.localPosition;
    }

    public void SetSlidePos(float percentNormalized)
    {
        float t = slideCurve != null ? slideCurve.Evaluate(percentNormalized) : percentNormalized;
        slidingObject.transform.localPosition = Vector3.Lerp(slideStart, slideEnd, t);
    }

    public void SlideToEnd()
    {
        StartCoroutine(SlideToEndCoroutine());
    }

    private IEnumerator SlideToEndCoroutine()
    {
        float step = 1.0f / slideTime;
        while (slidePosition < 1)
        {
            slidePosition += Time.deltaTime * step;
            SetSlidePos(slidePosition);
            yield return null;
        }
        //Set(1);
    }

    public void SlideToStart()
    {
        StartCoroutine(SlideToStartCoroutine());
    }

    private IEnumerator SlideToStartCoroutine()
    {
        float step = 1.0f / slideTime;
        while (slidePosition > 0)
        {
            slidePosition -= Time.deltaTime * step;
            SetSlidePos(slidePosition);
            yield return null;
        }
        //Set(0);
    }

    private void OnValidate()
    {
        if(slidingObject)
            SetSlidePos(slidePosition);
    }
}
