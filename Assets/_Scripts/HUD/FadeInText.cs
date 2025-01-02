using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeInText : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Color startColor;
    Color originalColor;
    [SerializeField] float countDown = 3;
    [SerializeField] float duration = 2;
    float usingCountDown;
    float usingDuration;
    float durationMultiplier;
    void OnEnable()
    {
        usingCountDown = countDown;
        usingDuration = duration;
        durationMultiplier = 1 / duration;
        text = GetComponent<TMP_Text>();
        originalColor = text.color;
        startColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        //if (startColor == null) { startColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0); }
        text.color = startColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (usingCountDown < 0)
        {
            usingDuration -= Time.deltaTime;
            float x = 1 - (durationMultiplier * usingDuration);
            x = Mathf.Clamp(x, 0.0f, 1.0f);
            Color target = Color.Lerp(startColor, originalColor, x);
            text.color = target;
        }
        else
        {
            usingCountDown -= Time.deltaTime;
        }
    }
}
