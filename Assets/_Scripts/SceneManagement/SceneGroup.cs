using System;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSceneGroup", menuName = "ScriptableObjects/SceneGroup", order = 100)]
public class SceneGroup : ScriptableObject
{
    public SceneData[] scenes;
    public int Count => scenes.Length;

    public string FindSceneNameByType(SceneType sceneType)
    {
        return scenes.FirstOrDefault(scene => sceneType == scene.type)?.Name;
    }
}

[Serializable]
public class SceneData
{
    public SceneField scene;
    public string Name => scene.SceneName;
    public SceneType type;
}

[Serializable]
public enum SceneType { ActiveScene, UI, HUD, Menu, Misc, Tool }