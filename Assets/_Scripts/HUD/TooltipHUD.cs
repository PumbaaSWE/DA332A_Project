using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipHUD : MonoBehaviour
{

    [SerializeField] private TMP_Text textArea;
    [SerializeField] private PlayerDataSO playerData;

    int priority = 0;

    float timer;

    Queue<string> texts;
    Queue<float> textTime;

    Interactor interactor;

    private void OnEnable()
    {
        TooltipUtil.OnTimedTooltip += ShowText;
        TooltipUtil.OnTooltip += ShowText;

        playerData.NotifyOnPlayerChanged(PlayerData_OnPlayerChanged);

        interactor = FindAnyObjectByType<Interactor>();
        if (interactor)
        {
            interactor.OnCanInteract += ShowText;
            interactor.OnCanInteractTimed += ShowText;
            interactor.OnInteracted += ShowText;
            interactor.OnInteractedPriority += ShowText;
        }
    }

    private void PlayerData_OnPlayerChanged(Transform obj)
    {
        if (obj)
        {
            if (interactor)
            {
                interactor.OnCanInteract -= ShowText;
                interactor.OnCanInteractTimed -= ShowText;
                interactor.OnInteracted -= ShowText;
                interactor.OnInteractedPriority -= ShowText;
            }
            interactor = obj.GetComponent<Interactor>();
            if (interactor)
            {
                interactor.OnCanInteract += ShowText;
                interactor.OnCanInteractTimed += ShowText;
                interactor.OnInteracted += ShowText;
                interactor.OnInteractedPriority += ShowText;
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
            interactor.OnCanInteractTimed -= ShowText;
            interactor.OnInteracted -= ShowText;
            interactor.OnInteractedPriority -= ShowText;
        }
    }

    private void QueueText(string text, float time)
    {
        if (texts == null || textTime == null)
        {
            texts = new Queue<string>();
            textTime = new Queue<float>();
        }
        texts.Enqueue(text);
        textTime.Enqueue(time);
    }

    public void ShowText(string text)
    {
        ShowText(text, 0, 0);
    }

    public void ShowText(string text, float time)
    {
        ShowText(text, time, 0);
    }

    public void ShowText(string text, int priority)
    {
        ShowText(text, 0, priority);
    }

    public void ShowText(string text, float time, int priority)
    {
        if (this.priority <= priority)
        {
            textArea.text = text;
            timer = time;
            textArea.enabled = true;
            this.priority = priority;
        }
        else
        {
            if (time > 0)
            {
                QueueText(text, time);
            }
        }
       // Debug.Log("HUD:" + text);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0)
        {
            if (texts != null)
            {
                if (texts.Count != 0 && texts.Count == textTime.Count)
                {
                    string text = texts.Dequeue();
                    float time = textTime.Dequeue();
                    ShowText(text, time, priority);
                }
                else
                {
                    priority = 0;
                    textArea.enabled = false;
                }
            }
            else
            {
                priority = 0;
                textArea.enabled = false;
            }
        }
        timer -= Time.deltaTime;
    }
}