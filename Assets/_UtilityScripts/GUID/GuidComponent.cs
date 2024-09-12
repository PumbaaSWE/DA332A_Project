using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[DisallowMultipleComponent, ExecuteInEditMode]
public class GuidComponent : MonoBehaviour
{
    [SerializeField] SerializableGuid guid;

    public bool IsGuidAssigned => guid != SerializableGuid.Empty;
    public SerializableGuid Guid => GetGuid(); //guid; //safe check?

    public SerializableGuid GetGuid()
    {
        if (guid == default)
        {
            CreateGuid();
        }
        return guid;
    }

    public void SetGuid(SerializableGuid guid)
    {
        GuidManager.Remove(this.guid); //check not empty?
        this.guid = guid;
        if (!GuidManager.Add(this))
        {
            //this guid is already registred but we try to set it, duplication is bad
            Debug.LogWarning("Are you duplicating GUIDs?");
        }
    }

    void Awake()
    {
        CreateGuid();
    }


    private void CreateGuid()
    {
        if(guid == default)
        {
#if UNITY_EDITOR
            if (IsAssetOnDisk())
            {
                return;
            }
            Undo.RecordObject(this, "Added GUID");
#endif
            guid = SerializableGuid.NewGuid();
#if UNITY_EDITOR
            // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
            // force a save of the modified prefab instance properties
            if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                //SerializedObject serializedObject = new SerializedObject(this);
                //SerializedProperty serializedProperty = serializedObject.FindProperty("guid");
                //PrefabUtility.RevertPropertyOverride(serializedProperty, InteractionMode.AutomatedAction);
            }

            if (PrefabUtility.IsPartOfAnyPrefab(this))
            {
                Debug.Log("this is part of a prefab");
               //
            }

            //if(PrefabUtility.GetPropertyModifications(this).Any(a => a.value == guid.ToHexString()))
            //{
            //    //
            //    Debug.Log("Modifications made " + guid.ToHexString());
            //}
#endif
        }
        //else if(guid == SerializableGuid.Empty)
        //{
        //    guid = SerializableGuid.NewGuid();
        //}

        if (guid != SerializableGuid.Empty)
        {
            if (!GuidManager.Add(this))
            {
                guid = SerializableGuid.Empty;
                CreateGuid();
            }
        }
    }




#if UNITY_EDITOR
    private bool IsEditingInPrefabMode()
    {
        if (EditorUtility.IsPersistent(this))
        {
            // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset
            return true;
        }
        else
        {
            // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
            var mainStage = StageUtility.GetMainStageHandle();
            var currentStage = StageUtility.GetStageHandle(gameObject);
            if (currentStage != mainStage)
            {
                var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                if (prefabStage != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsAssetOnDisk()
    {
        return PrefabUtility.IsPartOfPrefabAsset(this) || IsEditingInPrefabMode();
    }
#endif


    void OnValidate()
    {
#if UNITY_EDITOR
        // similar to on Serialize, but gets called on Copying a Component or Applying a Prefab
        // at a time that lets us detect what we are
        if (IsAssetOnDisk())
        {
            guid = SerializableGuid.Empty;
        }
        else
#endif
        {
            CreateGuid();
        }
    }

    public void OnDestroy()
    {
        GuidManager.Remove(guid);
    }
}
