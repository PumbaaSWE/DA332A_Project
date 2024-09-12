using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;

public class MakeButton
{
    private static readonly Dictionary<Type, Func<string, object, object>> @switch = new Dictionary<Type, Func<string, object, object>>
        {
            { typeof(float),    (s,v) => EditorGUILayout.FloatField(s,    v != null ? (float)v    : 0) },
            { typeof(int),      (s,v) => EditorGUILayout.IntField(s,      v != null ? (int)v      : 0) },
            { typeof(Vector2),  (s,v) => EditorGUILayout.Vector2Field(s,  v != null ? (Vector2)v  : Vector2.zero) },
            { typeof(Vector3),  (s,v) => EditorGUILayout.Vector3Field(s,  v != null ? (Vector3)v  : Vector3.zero) },
            { typeof(string),    (s,v) => EditorGUILayout.TextField(s,    v != null ? (string)v    : string.Empty) },
        };

    public readonly string displayName;
    public readonly MethodInfo method;
    public readonly object[] parameters = null;
    public readonly ParameterInfo[] parameterInfos = null;
    public readonly bool allowInEditMode;

    public MakeButton(string name, MethodInfo method, bool allowInEditMode)
    {
        this.method = method;
        displayName = name;
        parameterInfos = method.GetParameters();
        if (parameterInfos != null)
        {
            parameters = new object[parameterInfos.Length];
        }
        this.allowInEditMode = allowInEditMode;
    }


    public void DrawParams()
    {
        for (int i = 0; i < parameters.Length; i++)
        {
            Type type = parameterInfos[i].ParameterType;
            string paramName = parameterInfos[i].Name;
            if (@switch.ContainsKey(type))
            {
                parameters[i] = @switch[type](paramName, parameters[i]);
            }
            else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                parameters[i] = EditorGUILayout.ObjectField(paramName, (UnityEngine.Object)parameters[i], type, true);
            }
            else
            {
                Debug.LogWarning("[MakeButton] Used on unsuported method : " + method.Name + " parameter type " + type + " not supported!");
            }
        }
    }

    public void Draw(IEnumerable<object> targets)
    {
        bool disabled = !(allowInEditMode || Application.isPlaying);
        EditorGUI.BeginDisabledGroup(disabled);

        if (parameterInfos != null)
        {
            DrawParams();
        }

        if (!GUILayout.Button(displayName)) return;

        foreach (object target in targets)
        {
            method.Invoke(target, parameters);
        }

        EditorGUI.EndDisabledGroup();
    }


}