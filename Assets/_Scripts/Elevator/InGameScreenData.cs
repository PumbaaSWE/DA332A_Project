using UnityEngine;

[CreateAssetMenu(fileName = "InGameScreenData", menuName = "ScriptableObjects/InGameScreenData", order = 90)]
public class InGameScreenData : ScriptableObject
{
    public float swapTime = 1.5f;
    public float timeToSwap = 10f;
    public Texture[] sprites;
}