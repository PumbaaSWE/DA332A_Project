using System;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Voiceline", menuName = "ScriptableObjects/VoicelineData", order = 7)]
public class VoicelineData : ScriptableObject
{
    public AudioClip audioClip;
    public string subtitle;
    public float time = 5; //minimum display time
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
        time = Mathf.Clamp(subtitle.Length / 21.0f, 1.5f, 6.0f);
        if (subtitle.Length > 42)
        {
            int breakLineIndex = subtitle.LastIndexOf(' ', 42);
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine(subtitle[..breakLineIndex]);
            stringBuilder.AppendLine(subtitle[breakLineIndex..]);
            subtitle = stringBuilder.ToString();
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
