using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : PersistentSingleton<MusicManager>
{
    AudioClip[] audioClips;
    AudioSource[] audioSources;
    List<MusicNode> nodes;
    Player player;
    [SerializeField] AudioMixer mixer;

    protected override void Awake()
    {
        base.Awake();

        nodes = new();

        audioClips = Resources.LoadAll<AudioClip>("Loops");

        audioSources = new AudioSource[audioClips.Length];

        //AudioMixer.FindObjectsOfTypeIncludingAssets
        //AudioMixer mixer = FindAnyObjectByType<AudioMixer>();
        //Debug.Assert(mixer, "MusicManager - cannot find AudioMixer :(");
        AudioMixerGroup musicMixer = mixer ? mixer.FindMatchingGroups("Music")[0] : null;
        //Debug.Assert(musicMixer, "MusicManager - cannot find AudioMixerGroup called Music");
        //Addressables


        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].clip = audioClips[i];
            audioSources[i].loop = true;
            audioSources[i].spatialize = false;
            audioSources[i].Play();
            audioSources[i].outputAudioMixerGroup = musicMixer;
        }
    }

    private void LateUpdate()
    {
        if (audioSources.Length == 0)
            return;

        foreach (var source in audioSources)
        {
            //source.transform.position = player.HeadPos;
            source.volume = 0;
        }

        if (player == null)
        {
            player = FindAnyObjectByType<Player>();

            if (player == null)
                return;
        }

        

        foreach(var node in nodes)
        {
            float ping = node.Ping(player.HeadPos);

            foreach(var id in node.MusicIds)
            {
                if (id >= 0 && id < audioSources.Length)
                    audioSources[id].volume = Mathf.Max(audioSources[id].volume, ping);
            }
        }
    }

    public void Register(MusicNode node)
    {
        if (!nodes.Contains(node))
            nodes.Add(node);
    }

    public void Deregister(MusicNode node)
    {
        nodes.Remove(node);
    }
}
