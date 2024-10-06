using UnityEngine;

public class PlayAudioClip : MonoBehaviour
{
    
    public AudioClip clip;
    
    // Start is called before the first frame update
    public void Play()
    {
        if (clip)
        {
            //Debug.Log("clip");
            SimpleAudioManager.Instance.PlayClipAt(clip, transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
