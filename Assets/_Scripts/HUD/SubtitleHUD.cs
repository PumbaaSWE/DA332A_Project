using TMPro;
using UnityEngine;

public class SubtitleHUD : MonoBehaviour
{
    [SerializeField] SubtitleChannel subtitleChannel;
    [SerializeField] TMP_Text text;
    float timer;

    //TODO: Add queing..??

    void OnEnable()
    {
        if(subtitleChannel)
            subtitleChannel.OnChannelEvent += SubtitleChannel_OnChannelEvent;
        if (!text) text = GetComponent<TMP_Text>();
    }

    void OnDisable()
    {
        if (subtitleChannel)
            subtitleChannel.OnChannelEvent -= SubtitleChannel_OnChannelEvent;
    }

    private void SubtitleChannel_OnChannelEvent(SubtitleData ctx)
    {
        text.color = ctx.color;
        text.text = ctx.text;
        timer = ctx.time;
        text.enabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        if(timer <= 0)
        {
            //text.text = "";
            text.enabled = false;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
}
