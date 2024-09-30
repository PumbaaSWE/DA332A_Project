using UnityEngine;
using TMPro;

public class TooltipHUD : MonoBehaviour
{

    [SerializeField]  private TMP_Text textArea;
    [SerializeField] private PlayerDataSO playerData;
     float timer;

    Interactor interactor;

    private void OnEnable()
    {
        TooltipUtil.OnTimedTooltip += ShowText;
        TooltipUtil.OnTooltip += ShowText;

        playerData.NotifyOnPlayerChanged(PlayerData_OnPlayerChanged);
    }

    private void PlayerData_OnPlayerChanged(Transform obj)
    {
        if (obj)
        {
            if (interactor)
            {
                interactor.OnCanInteract -= ShowText;
            }
            interactor = obj.GetComponent<Interactor>();
            if (interactor)
            {
                interactor.OnCanInteract += ShowText;
            } 
        }
    }

    private void OnDisable()
    {
        TooltipUtil.OnTimedTooltip -= ShowText;
        TooltipUtil.OnTooltip -= ShowText;
        playerData.UnsubscribeOnPlayerChanged(PlayerData_OnPlayerChanged);
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
