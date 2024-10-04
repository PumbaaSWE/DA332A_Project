using TMPro;
using UnityEngine;

public class SubtitleHUD : MonoBehaviour
{
    [SerializeField] SubtitleChannel subtitleChannel;
    [SerializeField] TMP_Text text;
    float timer;

    //TODO: Add queing..??

    void Start()
    {
        if(subtitleChannel)
            subtitleChannel.OnChannelEvent += SubtitleChannel_OnChannelEvent;
        if (!text) text = GetComponent<TMP_Text>();
    }

    private void SubtitleChannel_OnChannelEvent(SubtitleData ctx)
    {
        text.color = ctx.color;
        text.text = ctx.text;
        timer = ctx.time;
        gameObject.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            //text.text = "";
            gameObject.SetActive(false);
        }
    }
}
