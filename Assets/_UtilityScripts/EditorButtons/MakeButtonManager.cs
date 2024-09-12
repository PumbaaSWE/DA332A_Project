using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MakeButtonManager
{
    const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private readonly List<MakeButton> makeButtons = new();

    public MakeButtonManager(Object target)
    {
        var methods = target.GetType().GetMethods(bindingFlags);

        foreach (MethodInfo method in methods)
        {
            var buttonAttribute = method.GetCustomAttribute<MakeButtonAttribute>();

            if (buttonAttribute == null)
                continue;

            string name = buttonAttribute.name != string.Empty ? buttonAttribute.name : method.Name;
            makeButtons.Add(new MakeButton(name, method, buttonAttribute.allowInEditMode));
        }
    }

    public void Draw(Object[] targets)
    {
        foreach (var button in makeButtons)
        {
            button.Draw(targets);
        }
    }
}
