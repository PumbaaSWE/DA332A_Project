using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TimedTrigger : MonoBehaviour
{
    public UnityEvent OnTrigger;
    public float TriggerPerMinute = 10f;
    public bool TriggerActive = false;
    private float oldTriggerPerMinute;
    private float calculatedTime;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        calculatedTime = (60.0f / TriggerPerMinute);
        timer = 0;
    }
    void CalculateTriggerPerMinute()
    {
        oldTriggerPerMinute = TriggerPerMinute;
        calculatedTime = (60.0f / TriggerPerMinute);
    }

    // Update is called once per frame
    void Update()
    {
        if (TriggerActive && timer >= calculatedTime)
        {
            timer = 0;
            Trigger();
        }
        else
        {
            timer += Time.deltaTime;
        }
        if(oldTriggerPerMinute != TriggerPerMinute)
        {
            CalculateTriggerPerMinute();
        }
    }

    void Trigger()
    {
        if(OnTrigger != null)
        {
            OnTrigger.Invoke();
        }
    }
}
