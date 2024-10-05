using UnityEngine;

//[CreateAssetMenu(fileName = "SubtitleChannel", menuName = "ScriptableObjects/SubtitleChannel", order = 8)]
public class SubtitleChannel : EventChannel<SubtitleData>
{

}

public struct SubtitleData
{
    public string text;
    public float time;
    public Color color;
}