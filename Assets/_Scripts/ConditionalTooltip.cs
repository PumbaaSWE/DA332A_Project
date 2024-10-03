using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalTooltip : MonoBehaviour
{
    [SerializeField] string message;
    [SerializeField] string blackboardCondition;
    [SerializeField] float waitTime;
    [SerializeField] float displayTime;

    public void Execute()
    {
        StartCoroutine(DisplayTooltip());
    }

    IEnumerator DisplayTooltip()
    {
        yield return new WaitForSeconds(waitTime);

        if (!Blackboard.Instance.Get<bool>(blackboardCondition))
        {
            TooltipUtil.Display(message, displayTime);
            StartCoroutine(RemoveTooltip());
        }
    }

    IEnumerator RemoveTooltip()
    {
        float timer = 0;
        while (timer < displayTime)
        {
            if (Blackboard.Instance.Get<bool>(blackboardCondition))
            {
                TooltipUtil.Display("");
                yield return null;
            }

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
}
