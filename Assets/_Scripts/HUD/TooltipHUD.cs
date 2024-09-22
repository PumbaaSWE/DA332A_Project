using UnityEngine;
using TMPro;

public class TooltipHUD : MonoBehaviour
{

    [SerializeField]  private TMP_Text textArea;

    float timer;

    Interactor interactor;

    private void OnEnable()
    {
        Tooltip.OnTimedTooltip += ShowText;
        Tooltip.OnTooltip += ShowText;

        interactor = FindAnyObjectByType<Interactor>();
        if (interactor)
        {
            interactor.OnCanInteract += ShowText;
        }
    }

    private void OnDisable()
    {
        Tooltip.OnTimedTooltip -= ShowText;
        Tooltip.OnTooltip -= ShowText;
        if (interactor)
        {
            interactor.OnCanInteract -= ShowText;
        }
    }

    public void ShowText(string text)
    {
        ShowText(text, 0);
    }

    public void ShowText(string text, float time)
    {
        textArea.text = text;
        timer = time;
        textArea.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < 0)
        {
            textArea.enabled = false;
        }
        timer -= Time.deltaTime;
    }
}
