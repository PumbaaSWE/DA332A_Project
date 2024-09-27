using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HearingManager;
public class EmittSoundAi : MonoBehaviour
{
    public float time;
    private void Start()
    {
        StartCoroutine(EmitSoundAfterDelay(time)); 
    }

    private IEnumerator EmitSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 
        HearingManager.Instance.OnSoundEmitted(gameObject, transform.position, EHeardSoundCategory.EWorldSound, .8f);
    }
}
