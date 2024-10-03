using System;
using UnityEngine;

public class BlackBoardRequrement : InteractRequrement
{
    [SerializeField] private BlackboardSetting[] blackboardSettings;

    public override bool Check(Transform _)
    {
        if(blackboardSettings == null) return true;

        for (int i = 0; i < blackboardSettings.Length; i++)
        {
            bool b = Blackboard.Instance.Get<bool>(blackboardSettings[i].entry);
            if (b != blackboardSettings[i].shouldBe)
            {
                return false;
            }
        }

        return true;
    }
}

[Serializable]
public struct BlackboardSetting
{
    public string entry;
    public bool shouldBe;
}
