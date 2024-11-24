using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

public class InGameScreen : MonoBehaviour
{
    [SerializeField] private Image noSignal;
    [SerializeField] private RawImage Image1;
    [SerializeField] private RawImage Image2;

    [SerializeField] private InGameScreenData data;


    private RawImage active;
    private RawImage swap;

    Rect screen;
    RectTransform activeRT;
    RectTransform swapRT;

    private Vector2 up;
    private Vector2 right;

    float screenAspect;
    float time = 0;
    int currSprite = 0;

    void Start()
    {
        active = Image1;
        swap = Image2;
        swap.gameObject.SetActive(false);
        activeRT = active.GetComponent<RectTransform>();
        swapRT = swap.GetComponent<RectTransform>();
        screen = GetComponent<RectTransform>().rect;
        up = new Vector2(0, screen.height);
        right = new Vector2(screen.width, 0);

        screenAspect = screen.width / screen.height;

        SetData(data);
    }

    public void SetData(InGameScreenData newData)
    {
        if (newData == null)
        {
            enabled = false;
            NoSignal(true);
            return;
        }
        NoSignal(false);
        data = newData;
    }

    public void NoSignal(bool signal)
    {
        noSignal.color = signal ? Color.blue : Color.black;
        noSignal.transform.GetChild(0).gameObject.SetActive(signal);
        active.gameObject.SetActive(!signal);
    }

    // Update is called once per frame
    void Update()
    {
        Display(Time.deltaTime);
    }

    public virtual void Display(float dt)
    {
        time += dt;
        if (time > data.timeToSwap)
        {
            time = -data.swapTime;
            currSprite = (currSprite + 1) % data.sprites.Length;
            SwapTexture(data.sprites[currSprite], data.swapTime);
        }
    }

    public void SwapTexture(Texture img, float time)
    {
        //Debug.Log("Swapping");
        swap.texture = img;
        StartCoroutine(DoSwap(time));
    }

    public void SwapImage(RawImage img, float time)
    {
        //Debug.Log("Swapping");
        swap = img;
        StartCoroutine(DoSwap(time));
    }

    private IEnumerator DoSwap(float time)
    {

        float aspect = swap.texture.width / swap.texture.height;

        float w;
        float h;
        if (screenAspect > aspect)
        {
            w = swap.texture.width * (screen.height / swap.texture.height);
            h = screen.height;
        }
        else
        {
            w = screen.width;
            h = swap.texture.height * (screen.width / swap.texture.width);
        }

        swap.rectTransform.sizeDelta = new Vector2(w, h);


        swap.gameObject.SetActive(true);
        Vector2 target = up;
        Vector2 current = activeRT.anchoredPosition;
        float t = 0;
        while (t < 1 && time > float.Epsilon)
        {
            t += Time.deltaTime / time;

            activeRT.anchoredPosition = Vector2.Lerp(current, target, t);
            swapRT.anchoredPosition = activeRT.anchoredPosition - up;

            yield return null;
        }
        activeRT.anchoredPosition = -up;
        (activeRT, swapRT) = (swapRT, activeRT);
        (swap, active) = (active, swap);
        swap.gameObject.SetActive(false);
    }

    //float Smoothstep(float a, float b, float t)
    //{
    //    // Scale, and clamp x to 0..1 range
    //    t = Mathf.Clamp01((t - a) / (b - a));
    //    return t * t * (3.0f - 2.0f * t);
    //}

    //Vector2 Smoothstep(Vector2 a, Vector2 b, float t)
    //{
    //    float x = Smoothstep(a.x, b.x, t);
    //    float y = Smoothstep(a.y, b.y, t);
    //    return new Vector2(x, y);
    //}

    [MakeButton]
    void StickToWall()
    {
        Transform t = transform.parent;
        if(Physics.Raycast(t.position, -t.forward, out RaycastHit hit, 10))
        {
            t.forward = hit.normal;
            t.position = hit.point+(hit.normal * 0.0005f);
        }
    }

}
