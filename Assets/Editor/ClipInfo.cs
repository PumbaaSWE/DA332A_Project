using UnityEditor;
using UnityEngine;

// Editor window for listing all float curves in an animation clip
public class ClipInfo : EditorWindow
{
    private AnimationClip clip;
    private string find;
    private string replace;
    private Vector2 scrollPosition;

    [MenuItem("Window/Clip Info")]
    static void Init()
    {
        GetWindow(typeof(ClipInfo));
    }

    public void OnGUI()
    {
        clip = EditorGUILayout.ObjectField("Clip", clip, typeof(AnimationClip), false) as AnimationClip;
        find = EditorGUILayout.TextField("Find:", find);
        replace = EditorGUILayout.TextField("Replace:", replace);

        if (GUILayout.Button("Find and Replace"))
        {
            var curves = AnimationUtility.GetCurveBindings(clip);
            for (int i = 0; i < curves.Length; i++)
            {
                var binding = curves[i];
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

                if (binding.path.Contains(find))
                {
                    AnimationUtility.SetEditorCurve(clip, binding, null);
                    string newPath = binding.path.Replace(find, replace);
                    binding.path = newPath;
                    AnimationUtility.SetEditorCurve(clip, binding, curve);
                }
            }
        }

        EditorGUILayout.LabelField("Curves:");
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        if (clip != null)
        {
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                EditorGUILayout.LabelField(binding.path + "/" + binding.propertyName + ", Keys: " + curve.keys.Length);
                //clip.
            }
        }
        GUILayout.EndScrollView();
    }
}
