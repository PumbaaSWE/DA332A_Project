using System.Collections.Generic;
using UnityEngine;

public class MusicManager : PersistentSingleton<MusicManager>
{
    AudioClip[] audioClips;
    AudioSource[] audioSources;
    List<MusicNode> nodes;
    Player player;

    protected override void Awake()
    {
        base.Awake();

        nodes = new();

        audioClips = Resources.LoadAll<AudioClip>("Loops");

        audioSources = new AudioSource[audioClips.Length];
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
            audioSources[i].clip = audioClips[i];
            audioSources[i].loop = true;
            audioSources[i].spatialize = false;
            audioSources[i].Play();
        }
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();

            if (player == null)
                return;
        }

        if (audioSources.Length == 0)
            return;

        foreach(var source in audioSources)
        {
            //source.transform.position = player.HeadPos;
            source.volume = 0;
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
