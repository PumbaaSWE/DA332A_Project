using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image image;
    [SerializeField] Color startColor;
    Color originalColor;
    [SerializeField] float countDown = 3;
    [SerializeField] float duration = 2;
    float durationMultiplier;
    void OnEnable()
    {
        durationMultiplier = 1 / duration;
        image = GetComponent<Image>();
        originalColor = image.color;
        if (startColor == null) { startColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0); }
        image.color = startColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (countDown < 0)
        {
            duration -= Time.deltaTime;
            float x = 1 - (durationMultiplier * duration);
            x = Mathf.Clamp(x, 0.0f, 1.0f);
            Color target = Color.Lerp(startColor, originalColor, x);
            image.color = target;
        }
        else
        {
            countDown -= Time.deltaTime;
        }
    }
}
