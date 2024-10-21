using UnityEngine;
using TMPro;
public class DebugClock : MonoBehaviour
{
    [SerializeField] TMP_Text fixedUpdateClock;
    [SerializeField] TMP_Text updateClock;
    float time = 0;
    float fixedTime = 0;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        updateClock.text = $"Update Time : {(int)time}";
    }

    void FixedUpdate()
    {
        fixedTime += Time.deltaTime;
        fixedUpdateClock.text = $"Fixed Update Time : {(int)fixedTime}";
    }
}
