using UnityEngine;

public class SimpleAudioManager : PersistentSingleton<SimpleAudioManager>
{
    
 //   private readonly Stack<AudioSourceData> audioSources = new();

//    public AudioSourceData audioSourceDataPrefab;
    public int maxSources = 20;
    public int sourcesPlaying = 0;
    public AudioSourceData[] audioSourceDatas;// = new AudioSourceData[maxSources];
    public AudioDataIndex[] audioDataIndices;
    public int[] freeList;
    public int firstFree = 0;

    protected override void Awake()
    {
        base.Awake();
        audioSourceDatas = new AudioSourceData[maxSources];
        audioDataIndices = new AudioDataIndex[maxSources];
        freeList = new int[maxSources];
        for (int i = 0; i < maxSources; i++)
        {
            audioSourceDatas[i] = new GameObject("AudioSourceData_" + i).AddComponent<AudioSourceData>();
            audioSourceDatas[i].transform.parent = transform;
        }
    }

    //private AudioSourceData GetAudioSource()
    //{
    //    if(audioSources.Count == 0)
    //    {
    //        return Instantiate(audioSourceDataPrefab);
    //    }
    //    return audioSources.Pop();
    //}

    //private void ReturnAudioSource(AudioSourceData audioSource)
    //{
    //    audioSources.Push(audioSource);
    //}


    public void PlayClipAt(AudioClip clip, Vector3 position, int prio = 0)
    {
        //Debug.Log("PlayClipAt");
        //int bestIndex = 0;
        //int bestPriority = int.MaxValue;
        //float leastTime = float.MaxValue;
        //for (int i = 0; i < maxSources; i++)
        //{
        //    if (!audioSourceDatas[i].IsPlaying)
        //    {
        //        audioSourceDatas[i].Play(clip, position, prio);
        //        return;
        //    }
        //    if(bestPriority > audioSourceDatas[i].Priority)
        //    {
        //        if (leastTime > audioSourceDatas[i].TimeLeft)
        //        {
        //            bestIndex = i;
        //        }
        //    }
        //}
        //audioSourceDatas[bestIndex].Play(clip, position, prio);
        int i;
        if(firstFree >= 0)
        {
            i = firstFree;
            firstFree = freeList[firstFree]; //the free list points to the next free
        }
        else
        {
            i = GetIndex(prio);
        }


        if (i >= 0)
        {
            audioDataIndices[i].time = Time.time;
            audioDataIndices[i].priority = prio;
            audioDataIndices[i].length = clip.length;
            audioSourceDatas[i].Play(clip, position);
        }


    }

    //gets the first availible audio source or the one with lowest prio and with time left as tiebreaker;
    public int GetIndex(int prio)
    {
        float now = Time.time;
        int bestIndex = -1;
        int bestPriority = prio;
        float bestTime = float.MaxValue;
        for (int i = 0; i < maxSources; i++)
        {
            var data = audioDataIndices[i];
            
            if (now - data.time >= data.length)
            {
                return i;
            }
            if (data.priority <= bestPriority)
            {
                bestPriority = data.priority;
                if (data.length - (now - data.time) < bestTime)
                {
                    bestTime = data.length - (now - data.time);
                    bestIndex = i;
                }
            }
        }
        return bestIndex;

    }


    // Update is called once per frame
    void Update()
    {
        float now = Time.time;
        int lastFree = firstFree;
        for (int i = maxSources-1; i >= 0; --i)
        {
            var data = audioDataIndices[i];
            if (now - data.time >= data.length) //clip has ended
            {
                freeList[i] = lastFree;
                lastFree = i;
            }
        }
        firstFree = lastFree;
    }
}

public struct AudioDataIndex
{
    public float time;
    public float length;
    public int priority;
}

public struct AudioData
{
    public AudioSource source;
    public AudioClip clip;
    public float volume;
    public float pitch;
}

