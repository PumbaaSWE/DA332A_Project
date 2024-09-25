using System;

public static class TooltipUtil
{
    public static event Action<string> OnTooltip;
    public static event Action<string, float> OnTimedTooltip;

    public static void Display(string tip)
    {
        OnTooltip?.Invoke(tip);
    }

    public static void Display(string tip, float time)
    {
        OnTimedTooltip?.Invoke(tip, time);
    }
}
