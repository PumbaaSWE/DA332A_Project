using System;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Voiceline", menuName = "ScriptableObjects/VoicelineData", order = 7)]
public class VoicelineData : ScriptableObject
{
    public AudioClip audioClip;
    public string[] subtitles;
    public float[] subtitlesTimeStamps;
    [Tooltip("Manual time set, default is same as aucioclip")]
    public float time = 0; //set specified time voiceline will play, if time is 0, the time will be set to the audioclips duration instead
    public Color color = Color.white;

    public Precondition[] preconditions;
    public VoicelineEffect[] effects;

    private void OnValidate()
    {
        //if (effects != null)
        //{
        //    VoicelineEffect[] newEffects = new VoicelineEffect[effects.Length];
        //    Array.Copy(effects, newEffects, effects.Length);
        //    Array.Sort(newEffects, (x, y) => { return y.eventTimeNormalized.CompareTo(x.eventTimeNormalized); });
        //    effects = newEffects;
        //}
    }

    public bool CheckPreconditions()
    {
        if(preconditions == null) return true;
        for (int i = 0; i < preconditions.Length; i++)
        {
            if(!preconditions[i].Check()) return false;
        }
        return true;
    }

    //Keep the subtitle reading speed at a maximum of 21 characters / second. 
    //A subtitle should not be on the screen for more than 6 seconds (otherwise the reader starts re-reading the subtitle) or for less than 1.5 seconds (otherwise the reader will not have enough time to read it).
    //When a subtitle is longer than 42 characters, break it into two lines.
    [MakeButton]
    public void ComputeTime()
    {
        for(int i = 0; i < subtitles[i].Length; i++)
        {
            time = Mathf.Clamp(subtitles[i].Length / 21.0f, 1.5f, 6.0f);
            if (subtitles[i].Length > 42)
            {
                int breakLineIndex = subtitles[i].LastIndexOf(' ', 42);
                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine(subtitles[i][..breakLineIndex]);
                stringBuilder.AppendLine(subtitles[i][breakLineIndex..]);
                subtitles[i] = stringBuilder.ToString();
            }
        }
    }
}

[Serializable]
public struct Precondition
{
    public string blackboardEntry;
    //public Unity
    //public delegate bool PreconditionPredicate();
    //public event PreconditionPredicate Predicate;

    public bool Check()
    {
        return Blackboard.GetBool(blackboardEntry);
    }
}

[Serializable]
public struct VoicelineEffect
{
    public string blackboardEntry;
    public float eventTimeNormalized;
    public void Activate()
    {
        Blackboard.SetBool(blackboardEntry, true);
    }

}
