using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableObject), true), CanEditMultipleObjects]
public class ScriptableObjectEditor : Editor
{

    private MakeButtonManager buttonManager;

    void OnEnable()
    {
        buttonManager = new MakeButtonManager(target);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        buttonManager.Draw(targets);
    }
}
