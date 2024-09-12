using System;
/// <summary>
/// <para>[MakeButton("Custom Name", bool allowInEditMode = true)]</para>
/// So far supports float, int, Vector2 and 3 as well as Unity.Object 
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class MakeButtonAttribute : Attribute
{
    public readonly string name;
    public readonly bool allowInEditMode;

    public MakeButtonAttribute(bool allowInEditMode = true)
    {
        name = string.Empty;
        this.allowInEditMode = allowInEditMode;
    }

    public MakeButtonAttribute(string name, bool allowInEditMode = true)
    {
        this.name = name;
        this.allowInEditMode = allowInEditMode;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class MakePlayButtonAttribute : Attribute
{
}